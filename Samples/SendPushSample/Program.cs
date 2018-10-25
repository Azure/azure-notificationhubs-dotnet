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

namespace SendPushSample
{
    class Program
    {
        private const string GcmSampleNotificationContent = "{\"data\":{\"message\":\"Notification Hub test notification from SDK sample\"}}";
        private const string GcmSampleSilentNotificationContent = "{ \"message\":{\"data\":{ \"Nick\": \"Mario\", \"body\": \"great match!\", \"Room\": \"PortugalVSDenmark\" } }}";
        private const string AppleSampleNotificationContent = "{\"aps\":{\"alert\":\"Notification Hub test notification from SDK sample\"}}";
        private const string AppleSampleSilentNotificationContent = "{\"aps\":{\"content-available\":1}, \"foo\": 2 }";

        static async Task Main(string[] args)
        {
            // Getting connection key from the new resource
            var config = LoadConfiguration(args);
            var nhClient = NotificationHubClient.CreateClientFromConnectionString(config.PrimaryConnectionString, config.HubName);

            // Register some fake devices
            var gcmDeviceId = Guid.NewGuid().ToString();
            var gcmInstallation = new Installation
            {
                InstallationId = "fake-gcm-install-id",
                Platform = NotificationPlatform.Gcm,
                PushChannel = gcmDeviceId,
                PushChannelExpired = false,
                Tags = new [] { "gcm" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(gcmInstallation);
            
            var appleDeviceId = "00fc13adff785122b4ad28809a3420982341241421348097878e577c991de8f0";
            var apnsInstallation = new Installation
            {
                InstallationId = "fake-apns-install-id",
                Platform = NotificationPlatform.Apns,
                PushChannel = appleDeviceId,
                PushChannelExpired = false,
                Tags = new [] { "apns" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(apnsInstallation);

            switch ((SampleConfiguration.Operation)Enum.Parse(typeof(SampleConfiguration.Operation), config.SendType))
            {
                case SampleConfiguration.Operation.Broadcast:
                    // Send notifications to all users
                    var outcomeGcm = await nhClient.SendGcmNativeNotificationAsync(GcmSampleNotificationContent);
                    var outcomeSilentGcm = await nhClient.SendGcmNativeNotificationAsync(GcmSampleSilentNotificationContent);
                    var outcomeApns = await nhClient.SendAppleNativeNotificationAsync(AppleSampleNotificationContent);
                    var outcomeSilentApns = await nhClient.SendAppleNativeNotificationAsync(AppleSampleSilentNotificationContent);

                    // Gather send outcome
                    var gcmOutcomeDetails = await WaitForThePushStatusAsync("GCM", nhClient, outcomeGcm);
                    var gcmSilentOutcomeDetails = await WaitForThePushStatusAsync("GCM", nhClient, outcomeSilentGcm);
                    var apnsOutcomeDetails = await WaitForThePushStatusAsync("APNS", nhClient, outcomeApns);
                    var apnsSilentOutcomeDetails = await WaitForThePushStatusAsync("APNS", nhClient, outcomeSilentApns);
                    PrintPushOutcome("GCM", gcmOutcomeDetails, gcmOutcomeDetails.GcmOutcomeCounts);
                    PrintPushOutcome("GCM Silent ", gcmSilentOutcomeDetails, gcmSilentOutcomeDetails.GcmOutcomeCounts);
                    PrintPushOutcome("APNS", apnsOutcomeDetails, apnsOutcomeDetails.ApnsOutcomeCounts);
                    PrintPushOutcome("APNS Silent", apnsSilentOutcomeDetails, apnsSilentOutcomeDetails.ApnsOutcomeCounts);
                    break;
                case SampleConfiguration.Operation.SendByTag:
                    // Send notifications by tag
                    var outcomeGcmByTag = await nhClient.SendGcmNativeNotificationAsync(GcmSampleNotificationContent, config.Tag ?? "gcm");
                    var outcomeApnsByTag = await nhClient.SendAppleNativeNotificationAsync(AppleSampleNotificationContent, config.Tag ?? "apns");
                    // Gather send outcome
                    var gcmTagOutcomeDetails = await WaitForThePushStatusAsync("GCM Tags", nhClient, outcomeGcmByTag);
                    var apnsTagOutcomeDetails = await WaitForThePushStatusAsync("APNS Tags", nhClient, outcomeApnsByTag);
                    PrintPushOutcome("GCM Tags", gcmTagOutcomeDetails, gcmTagOutcomeDetails.GcmOutcomeCounts);
                    PrintPushOutcome("APNS Tags", apnsTagOutcomeDetails, apnsTagOutcomeDetails.ApnsOutcomeCounts);
                    break;
                case SampleConfiguration.Operation.SendByDevice:
                    // Send notifications by deviceId
                    var outcomeGcmByDeviceId = await nhClient.SendDirectNotificationAsync(CreateGcmNotification(), config.GcmDeviceId ?? gcmDeviceId);
                    var outcomeApnsByDeviceId = await nhClient.SendDirectNotificationAsync(CreateApnsNotification(), config.AppleDeviceId ?? appleDeviceId);
                    // Gather send outcome
                    var gcmDirectSendOutcomeDetails = await WaitForThePushStatusAsync("GCM direct", nhClient, outcomeGcmByDeviceId);
                    var apnsDirectSendOutcomeDetails = await WaitForThePushStatusAsync("APNS direct", nhClient, outcomeApnsByDeviceId);
                    PrintPushOutcome("GCM Direct", gcmDirectSendOutcomeDetails, gcmDirectSendOutcomeDetails.ApnsOutcomeCounts);
                    PrintPushOutcome("APNS Direct", apnsDirectSendOutcomeDetails, apnsDirectSendOutcomeDetails.ApnsOutcomeCounts);
                    break;
                default:
                    Console.WriteLine("Invalid Sendtype");
                    break;
            }
        }

        private static Notification CreateGcmNotification()
        {
            return new GcmNotification(GcmSampleNotificationContent);
        }

        private static Notification CreateApnsNotification()
        {
            return new AppleNotification(AppleSampleNotificationContent);
        }

        private static async Task<NotificationDetails> WaitForThePushStatusAsync(string pnsType, NotificationHubClient nhClient, NotificationOutcome notificationOutcome) 
        {
            var notificationId = notificationOutcome.NotificationId;
            var state = NotificationOutcomeState.Enqueued;
            var count = 0;
            NotificationDetails outcomeDetails = null;
            while ((state == NotificationOutcomeState.Enqueued || state == NotificationOutcomeState.Processing) && ++count < 10)
            {
                try
                {
                    Console.WriteLine($"{pnsType} status: {state}");
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

        private static void PrintPushOutcome(string pnsType, NotificationDetails details, NotificationOutcomeCollection collection)
        {
            if (collection != null) 
            {
                Console.WriteLine($"{pnsType} outcome: " + string.Join(",", collection.Select(kv => $"{kv.Key}:{kv.Value}")));    
            }
            else 
            {
                Console.WriteLine($"{pnsType} no outcomes.");
            }
            Console.WriteLine($"{pnsType} error details URL: {details.PnsErrorDetailsUri}");
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
    }
}
