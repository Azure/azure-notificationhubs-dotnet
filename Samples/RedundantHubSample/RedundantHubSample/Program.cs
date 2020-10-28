// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System;
using System.Collections.Generic;
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

        static async Task Main(string[] args)
        {
            // Getting connection key from the new resource
            var config = LoadConfiguration(args);
            var nhClient = new RedundantNotificationHubClient(config.PrimaryConnectionString, config.BackupConnectionString, config.HubName);
            await CreateRedundantInstallations(nhClient);
        }

        private static async Task CreateRedundantInstallations(RedundantNotificationHubClient nhClient)
        {
            var deviceId = Guid.NewGuid().ToString();
            var installation = new Installation
            {
                InstallationId = "test-redundancy-install-id",
                Platform = NotificationPlatform.Fcm,
                PushChannel = deviceId,
                PushChannelExpired = false,
                Tags = new[] { "fcm" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(installation);

            await nhClient.GetInstallationAsync(installation.InstallationId);
            

            var outcomeFcm = await nhClient.SendFcmNativeNotificationAsync(FcmSampleNotificationContent, installation.InstallationId);
            var details = await GetPushDetailsAndPrintOutcome(nhClient, outcomeFcm);
            PrintPushOutcome(details, true);

            // Send notifications to installation in backup namespace
            nhClient.DefaultNamespace = DefaultNamespace.Backup;
            var outcomeFcmFromBackUp = await nhClient.SendFcmNativeNotificationAsync(FcmSampleNotificationContent, installation.InstallationId);
            var backupDetails = await GetPushDetailsAndPrintOutcome(nhClient, outcomeFcmFromBackUp);
            PrintPushOutcome(backupDetails, false);
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
            if (string.IsNullOrEmpty(notificationOutcome.NotificationId))
            {
                Console.WriteLine($"Fcm has no outcome due to it is only available for Standard SKU pricing tier.");
                return null;
            }

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
}
