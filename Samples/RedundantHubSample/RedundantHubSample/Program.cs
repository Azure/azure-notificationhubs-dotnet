// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Configuration;

namespace RedundantHubSample
{
    class Program
    {
        private const string FcmSampleNotificationContent = "{\"data\":{\"message\":\"Notification Hub test notification from SDK sample\"}}";
        private const string FcmSampleSilentNotificationContent = "{ \"message\":{\"data\":{ \"Nick\": \"Mario\", \"body\": \"great match!\", \"Room\": \"PortugalVSDenmark\" } }}";

        static async Task Main(string[] args)
        {
            // Getting connection key from the new resource
            var config = LoadConfiguration(args);
            var nhClient = new RedundantNotificationHubClient(config.PrimaryConnectionString, config.BackupConnectionString, config.HubName);
            await CreateRedundantInstallations(nhClient);
        }

        private static async Task CreateRedundantInstallations(RedundantNotificationHubClient nhClient)
        {
            var fcmDeviceId = Guid.NewGuid().ToString();
            var fcmInstallation = new Installation
            {
                InstallationId = "test-redundancy-install-id",
                Platform = NotificationPlatform.Fcm,
                PushChannel = fcmDeviceId,
                PushChannelExpired = false,
                Tags = new[] { "fcm" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(fcmInstallation);

            var installtions = await nhClient.GetInstallaitonsAsync(fcmInstallation.InstallationId);
            if (installtions.Length == 2)
            {
                Console.WriteLine("Installtions created in both namespaces");
            }

            var outcomeFcm = await nhClient.SendFcmNativeNotificationAsync(FcmSampleNotificationContent, fcmInstallation.InstallationId);
            var details = await GetPushDetailsAndPrintOutcome(nhClient, outcomeFcm);
            PrintPushOutcome(details, nhClient.UsePrimaryForSend);

            // Send notifications to installation in backup namespace
            nhClient.UsePrimaryForSend = false;
            var outcomeFcmFromBackUp = await nhClient.SendFcmNativeNotificationAsync(FcmSampleNotificationContent, fcmInstallation.InstallationId);
            var backupDetails = await GetPushDetailsAndPrintOutcome(nhClient, outcomeFcmFromBackUp);
            PrintPushOutcome(backupDetails, nhClient.UsePrimaryForSend);
        }

        private static SampleConfiguration LoadConfiguration(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("config.json", true);
            configurationBuilder.AddCommandLine(args);
            var configRoot = configurationBuilder.Build();
            var sampleConfig = new SampleConfiguration();
            configRoot.Bind(sampleConfig);
            return sampleConfig;
        }

        private static async Task<NotificationDetails> GetPushDetailsAndPrintOutcome(RedundantNotificationHubClient nhClient, NotificationOutcome notificationOutcome)
        {
            // The Notification ID is only available for Standard SKUs. For Basic and Free SKUs the API to get notification outcome details can not be called.
            if (string.IsNullOrEmpty(notificationOutcome.NotificationId))
            {
                Console.WriteLine($"Fcm has no outcome due to it is only available for Standard SKU pricing tier.");
                return null;
            }

            return await WaitForThePushStatusAsync(nhClient, notificationOutcome);
        }

        private static async Task<NotificationDetails> WaitForThePushStatusAsync(RedundantNotificationHubClient nhClient, NotificationOutcome notificationOutcome)
        {
            var notificationId = notificationOutcome.NotificationId;
            var state = NotificationOutcomeState.Enqueued;
            var count = 0;
            NotificationDetails outcomeDetails = null;
            while ((state == NotificationOutcomeState.Enqueued || state == NotificationOutcomeState.Processing) && ++count < 10)
            {
                try
                {
                    Console.WriteLine($"Status: {state}");
                    outcomeDetails = await nhClient.GetNotificationOutcomeDetailsAsync(notificationId);
                    state = outcomeDetails.State;
                }
                catch (MessagingEntityNotFoundException)
                {
                    // It's possible for the notification to not yet be enqueued, so we may have to swallow an exception
                    // until it's ready to give us a new state.
                }
                Thread.Sleep(1000);
            }
            return outcomeDetails;
        }

        private static void PrintPushOutcome(NotificationDetails details, bool isPrimary)
        {
            if (details.FcmOutcomeCounts != null)
            {
                Console.WriteLine($"Notification outcome for {(isPrimary ? "Primary" : "Backup")}: " + string.Join(",", details.FcmOutcomeCounts.Select(kv => $"{kv.Key}:{kv.Value}")));
            }
            else
            {
                Console.WriteLine($"No outcomes for {(isPrimary ? "Primary" : "Backup")}");
            }
            Console.WriteLine($"{(isPrimary ? "Primary" : "Backup")} error details URL: {details.PnsErrorDetailsUri}");
        }
    }

    public class RedundantNotificationHubClient
    {
        private readonly INotificationHubClient _primaryNotificationHubClient;
        private readonly INotificationHubClient _backupNotificationHubClient;

        public RedundantNotificationHubClient(string primaryConnectionString, string backupConnectionString, string hubName)
        {
            _primaryNotificationHubClient = NotificationHubClient.CreateClientFromConnectionString(primaryConnectionString, hubName);
            _backupNotificationHubClient = NotificationHubClient.CreateClientFromConnectionString(backupConnectionString, hubName);
        }

        public bool UsePrimaryForSend { get; set; } = true;

        public Task CreateOrUpdateInstallationAsync(Installation installation, CancellationToken cancellationToken = default)
            => Task.WhenAll(
                _primaryNotificationHubClient.CreateOrUpdateInstallationAsync(installation, cancellationToken),
                _backupNotificationHubClient.CreateOrUpdateInstallationAsync(installation, cancellationToken));

        public async Task<Installation[]> GetInstallaitonsAsync(string installationId)
        {
            return new Installation[] {await _primaryNotificationHubClient.GetInstallationAsync(installationId), await _backupNotificationHubClient.GetInstallationAsync(installationId)};
        }

        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken = default)
            => UsePrimaryForSend ?
                _primaryNotificationHubClient.SendFcmNativeNotificationAsync(jsonPayload, tagExpression, cancellationToken) :
                _backupNotificationHubClient.SendFcmNativeNotificationAsync(jsonPayload, tagExpression, cancellationToken);


        public Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId, CancellationToken cancellationToken = default)
            => UsePrimaryForSend ?
                _primaryNotificationHubClient.GetNotificationOutcomeDetailsAsync(notificationId, cancellationToken) :
                _backupNotificationHubClient.GetNotificationOutcomeDetailsAsync(notificationId, cancellationToken);
    }
}
