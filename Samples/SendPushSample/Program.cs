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
        private const string FcmSampleNotificationContent = "{\"data\":{\"message\":\"Notification Hub test notification from SDK sample\"}}";
        private const string FcmSampleSilentNotificationContent = "{ \"message\":{\"data\":{ \"Nick\": \"Mario\", \"body\": \"great match!\", \"Room\": \"PortugalVSDenmark\" } }}";
        private const string AppleSampleNotificationContent = "{\"aps\":{\"alert\":\"Notification Hub test notification from SDK sample\"}}";
        private const string AppleSampleSilentNotificationContent = "{\"aps\":{\"content-available\":1}, \"foo\": 2 }";
        private const string WnsSampleNotification = "<?xml version=\"1.0\" encoding=\"utf-8\"?><toast><visual><binding template=\"ToastText01\"><text id=\"1\">Notification Hub test notification from SDK sample</text></binding></visual></toast>";

        static async Task Main(string[] args)
        {
            // Getting connection key from the new resource
            var config = LoadConfiguration(args);
            var nhClient = NotificationHubClient.CreateClientFromConnectionString(config.PrimaryConnectionString, config.HubName);

            // Register some fake devices
            var fcmDeviceId = Guid.NewGuid().ToString();
            var fcmInstallation = new Installation
            {
                InstallationId = "fake-fcm-install-id",
                Platform = NotificationPlatform.Fcm,
                PushChannel = fcmDeviceId,
                PushChannelExpired = false,
                Tags = new [] { "fcm" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(fcmInstallation);
            
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
                    var outcomeFcm = await nhClient.SendFcmNativeNotificationAsync(FcmSampleNotificationContent);
                    var outcomeSilentFcm = await nhClient.SendFcmNativeNotificationAsync(FcmSampleSilentNotificationContent);
                    var outcomeApns = await nhClient.SendAppleNativeNotificationAsync(AppleSampleNotificationContent);
                    var outcomeSilentApns = await nhClient.SendAppleNativeNotificationAsync(AppleSampleSilentNotificationContent);
                    var outcomeWns = await nhClient.SendWindowsNativeNotificationAsync(WnsSampleNotification);

                    // Gather send outcome
                    var fcmOutcomeDetails = await WaitForThePushStatusAsync("FCM", nhClient, outcomeFcm);
                    var fcmSilentOutcomeDetails = await WaitForThePushStatusAsync("FCM", nhClient, outcomeSilentFcm);
                    var apnsOutcomeDetails = await WaitForThePushStatusAsync("APNS", nhClient, outcomeApns);
                    var apnsSilentOutcomeDetails = await WaitForThePushStatusAsync("APNS", nhClient, outcomeSilentApns);
                    var wnsOutcomeDetails = await WaitForThePushStatusAsync("WNS", nhClient, outcomeWns);
                    PrintPushOutcome("FCM", fcmOutcomeDetails, fcmOutcomeDetails.FcmOutcomeCounts);
                    PrintPushOutcome("FCM Silent ", fcmSilentOutcomeDetails, fcmSilentOutcomeDetails.FcmOutcomeCounts);
                    PrintPushOutcome("APNS", apnsOutcomeDetails, apnsOutcomeDetails.ApnsOutcomeCounts);
                    PrintPushOutcome("APNS Silent", apnsSilentOutcomeDetails, apnsSilentOutcomeDetails.ApnsOutcomeCounts);
                    PrintPushOutcome("WNS", wnsOutcomeDetails, wnsOutcomeDetails.WnsOutcomeCounts);
                    break;
                case SampleConfiguration.Operation.SendByTag:
                    // Send notifications by tag
                    var outcomeFcmByTag = await nhClient.SendFcmNativeNotificationAsync(FcmSampleNotificationContent, config.Tag ?? "fcm");
                    var outcomeApnsByTag = await nhClient.SendAppleNativeNotificationAsync(AppleSampleNotificationContent, config.Tag ?? "apns");
                    // Gather send outcome
                    var fcmTagOutcomeDetails = await WaitForThePushStatusAsync("FCM Tags", nhClient, outcomeFcmByTag);
                    var apnsTagOutcomeDetails = await WaitForThePushStatusAsync("APNS Tags", nhClient, outcomeApnsByTag);
                    PrintPushOutcome("FCM Tags", fcmTagOutcomeDetails, fcmTagOutcomeDetails.FcmOutcomeCounts);
                    PrintPushOutcome("APNS Tags", apnsTagOutcomeDetails, apnsTagOutcomeDetails.ApnsOutcomeCounts);
                    break;
                case SampleConfiguration.Operation.SendByDevice:
                    // Send notifications by deviceId
                    var outcomeFcmByDeviceId = await nhClient.SendDirectNotificationAsync(CreateFcmNotification(), config.FcmDeviceId ?? fcmDeviceId);
                    var outcomeApnsByDeviceId = await nhClient.SendDirectNotificationAsync(CreateApnsNotification(), config.AppleDeviceId ?? appleDeviceId);
                    // Gather send outcome
                    var fcmDirectSendOutcomeDetails = await WaitForThePushStatusAsync("FCM direct", nhClient, outcomeFcmByDeviceId);
                    var apnsDirectSendOutcomeDetails = await WaitForThePushStatusAsync("APNS direct", nhClient, outcomeApnsByDeviceId);
                    PrintPushOutcome("FCM Direct", fcmDirectSendOutcomeDetails, fcmDirectSendOutcomeDetails.ApnsOutcomeCounts);
                    PrintPushOutcome("APNS Direct", apnsDirectSendOutcomeDetails, apnsDirectSendOutcomeDetails.ApnsOutcomeCounts);
                    break;
                default:
                    Console.WriteLine("Invalid Sendtype");
                    break;
            }
        }

        private static Notification CreateFcmNotification()
        {
            return new FcmNotification(FcmSampleNotificationContent);
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
