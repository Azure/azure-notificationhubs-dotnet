// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Configuration;

namespace ParseFeedbackSample
{
    class Program
    {

        private const string GcmSampleNotificationContent = "{\"data\":{\"message\":\"Notification Hub test notification from SDK sample\"}}";

        static async Task Main(string[] args)
        {
            // Getting connection key from the new resource
            var config = LoadConfiguration(args);
            var nhClient = NotificationHubClient.CreateClientFromConnectionString(config.PrimaryConnectionString, config.HubName);

            // Register some fake devices
            var gcmDeviceId1 = Guid.NewGuid().ToString();
            var gcmInstallation1 = new Installation
            {
                InstallationId = "fake-gcm-install-id1",
                Platform = NotificationPlatform.Gcm,
                PushChannel = gcmDeviceId1,
                PushChannelExpired = false,
                Tags = new [] { "gcm" }
            };
            var gcmDeviceId2 = Guid.NewGuid().ToString();
            var gcmInstallation2 = new Installation
            {
                InstallationId = "fake-gcm-install-id2",
                Platform = NotificationPlatform.Gcm,
                PushChannel = gcmDeviceId2,
                PushChannelExpired = false,
                Tags = new[] { "gcm" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(gcmInstallation1);
            await nhClient.CreateOrUpdateInstallationAsync(gcmInstallation2);

            await Task.Delay(5000);

            // Send notifications to all users
            var outcomeGcm = await nhClient.SendGcmNativeNotificationAsync(GcmSampleNotificationContent);
            
            // Gather send outcome
            var gcmOutcomeDetails = await WaitForThePushStatusAsync("GCM", nhClient, outcomeGcm);

            Console.WriteLine($"GCM error details URL: {gcmOutcomeDetails.PnsErrorDetailsUri}");

            if (gcmOutcomeDetails.PnsErrorDetailsUri != null)
            {
                var httpClient = new HttpClient();
                var pnsFeedbackXmlStrings = await httpClient.GetStringAsync(gcmOutcomeDetails.PnsErrorDetailsUri);

                // Parse XML
                var pnsFeedbackXmlStringsSplit = pnsFeedbackXmlStrings.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var pnsFeedbackXmlString in pnsFeedbackXmlStringsSplit)
                {
                    var xdoc = XDocument.Parse(pnsFeedbackXmlString);
                    XNamespace ns = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect";
                    Console.WriteLine($"GCM PNS Feedback Content: \n{xdoc}");
                    PnsFeedback feedback = new PnsFeedback
                    {
                        FeedbackTime = DateTime.Parse(xdoc.Root.Element(ns + "FeedbackTime").Value),
                        NotificationSystemError = Enum.Parse<PnsError>(xdoc.Root.Element(ns + "NotificationSystemError").Value),
                        Platform = xdoc.Root.Element(ns + "Platform").Value,
                        PnsHandle = xdoc.Root.Element(ns + "PnsHandle").Value,
                        InstallationId = xdoc.Root.Element(ns + "InstallationId").Value,
                        NotificationId = xdoc.Root.Element(ns + "NotificationId").Value
                    };
                }
            }
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
