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

namespace RegistrationSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Getting connection key from the new resource
            var config = LoadConfiguration(args);
            var nhClient = NotificationHubClient.CreateClientFromConnectionString(config.PrimaryConnectionString, config.HubName);
            await CreateAndDeleteInstallationAsync(nhClient);
            await CreateAndDeleteRegistrationAsync(nhClient);
        }

        private static async Task CreateAndDeleteRegistrationAsync(NotificationHubClient nhClient)
        {
            var registrationId = await nhClient.CreateRegistrationIdAsync();
            var registrationDescr = await nhClient.CreateFcmNativeRegistrationAsync(registrationId);
            Console.WriteLine($"Created FCM registration {registrationDescr.FcmRegistrationId}");

            var allRegistrations = await nhClient.GetAllRegistrationsAsync(1000);
            foreach (var regFromServer in allRegistrations)
            {
                if (regFromServer.RegistrationId == registrationDescr.RegistrationId)
                {
                    Console.WriteLine($"Found FCM registration {registrationDescr.FcmRegistrationId}");
                    break;
                }
            }

            //registrationDescr = await nhClient.GetRegistrationAsync<FcmRegistrationDescription>(registrationId);
            //Console.WriteLine($"Retrieved FCM registration {registrationDescr.FcmRegistrationId}");

            await nhClient.DeleteRegistrationAsync(registrationDescr);
            Console.WriteLine($"Deleted FCM registration {registrationDescr.FcmRegistrationId}");
        }

        private static async Task CreateAndDeleteInstallationAsync(NotificationHubClient nhClient)
        {
            // Register some fake devices
            var fcmDeviceId = Guid.NewGuid().ToString();
            var fcmInstallation = new Installation
            {
                InstallationId = fcmDeviceId,
                Platform = NotificationPlatform.Fcm,
                PushChannel = fcmDeviceId,
                PushChannelExpired = false,
                Tags = new[] { "fcm" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(fcmInstallation);

            while (true)
            {
                try
                {
                    var installationFromServer = await nhClient.GetInstallationAsync(fcmInstallation.InstallationId);
                    break;
                }
                catch (MessagingEntityNotFoundException)
                {
                    // Wait for installation to be created
                    await Task.Delay(1000);
                }
            }
            Console.WriteLine($"Created FCM installation {fcmInstallation.InstallationId}");
            await nhClient.DeleteInstallationAsync(fcmInstallation.InstallationId);
            while (true)
            {
                try
                {
                    var installationFromServer = await nhClient.GetInstallationAsync(fcmInstallation.InstallationId);
                    await Task.Delay(1000);
                }
                catch (MessagingEntityNotFoundException)
                {
                    Console.WriteLine($"Deleted FCM installation {fcmInstallation.InstallationId}");
                    break;
                }
            }
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
