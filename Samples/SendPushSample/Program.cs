// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Configuration;

namespace SendPushSample
{
    class Program
    {

        private const string GcmSampleNotificationContent = "{\"data\":{\"message\":\"Notification Hub test notification from SDK sample\"}}";
        private const string AppleSampleNotificationContent = "{\"aps\":{\"alert\":\"Notification Hub test notification from SDK sample\"}}";

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

            // Send notifications to all users
            var outcomeGcm = await nhClient.SendGcmNativeNotificationAsync(GcmSampleNotificationContent);
            var outcomeApns = await nhClient.SendAppleNativeNotificationAsync(AppleSampleNotificationContent);

            // Send notifications by tag
            var outcomeGcmByTag = await nhClient.SendGcmNativeNotificationAsync(GcmSampleNotificationContent, "gcm");
            var outcomeApnsByTag = await nhClient.SendAppleNativeNotificationAsync(AppleSampleNotificationContent, "apns");

            // Send notifications by deviceId
            var outcomeGcmByDeviceId = await nhClient.SendDirectNotificationAsync(CreateGcmNotification(), gcmDeviceId);
            var outcomeApnsByDeviceId = await nhClient.SendDirectNotificationAsync(CreateApnsNotification(), appleDeviceId);

            // Gather send outcome
            var gcmOutcomeDetails = await WaitForThePushStatusAsync("GCM", nhClient, outcomeGcm);
            var apnsOutcomeDetails = await WaitForThePushStatusAsync("APNS", nhClient, outcomeApns);
            var gcmTagOutcomeDetails = await WaitForThePushStatusAsync("GCM Tags", nhClient, outcomeGcmByTag);
            var apnsTagOutcomeDetails = await WaitForThePushStatusAsync("APNS Tags", nhClient, outcomeApnsByTag);
            var gcmDirectSendOutcomeDetails = await WaitForThePushStatusAsync("GCM direct", nhClient, outcomeGcmByDeviceId);
            var apnsDirectSendOutcomeDetails = await WaitForThePushStatusAsync("APNS direct", nhClient, outcomeApnsByDeviceId);

            PrintPushOutcome("GCM", gcmOutcomeDetails, gcmOutcomeDetails.GcmOutcomeCounts);
            PrintPushOutcome("APNS", apnsOutcomeDetails, apnsOutcomeDetails.ApnsOutcomeCounts);
            PrintPushOutcome("GCM Tags", gcmTagOutcomeDetails, gcmTagOutcomeDetails.GcmOutcomeCounts);
            PrintPushOutcome("APNS Tags", apnsTagOutcomeDetails, apnsTagOutcomeDetails.ApnsOutcomeCounts);
            PrintPushOutcome("GCM Direct", gcmDirectSendOutcomeDetails, gcmDirectSendOutcomeDetails.ApnsOutcomeCounts);
            PrintPushOutcome("APNS Direct", apnsDirectSendOutcomeDetails, apnsDirectSendOutcomeDetails.ApnsOutcomeCounts);
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
            NotificationDetails outcomeDetails = await nhClient.GetNotificationOutcomeDetailsAsync(notificationId);
            var state = outcomeDetails.State;
            var count = 0;
            while ((state == NotificationOutcomeState.Enqueued || state == NotificationOutcomeState.Processing) && ++count < 10)
            {
                Console.WriteLine($"{pnsType} status: {state}");
                outcomeDetails = await nhClient.GetNotificationOutcomeDetailsAsync(notificationId);
                state = outcomeDetails.State;
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
