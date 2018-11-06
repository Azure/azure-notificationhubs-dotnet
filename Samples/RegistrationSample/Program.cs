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
            var registrationDescr = await nhClient.CreateGcmNativeRegistrationAsync(registrationId);
            Console.WriteLine($"Created GCM registration {registrationDescr.GcmRegistrationId}");

            var allRegistrations = await nhClient.GetAllRegistrationsAsync(1000);
            foreach (var regFromServer in allRegistrations)
            {
                if (regFromServer.RegistrationId == registrationDescr.RegistrationId)
                {
                    Console.WriteLine($"Found GCM registration {registrationDescr.GcmRegistrationId}");
                    break;
                }
            }

            //registrationDescr = await nhClient.GetRegistrationAsync<GcmRegistrationDescription>(registrationId);
            //Console.WriteLine($"Retrieved GCM registration {registrationDescr.GcmRegistrationId}");

            await nhClient.DeleteRegistrationAsync(registrationDescr);
            Console.WriteLine($"Deleted GCM registration {registrationDescr.GcmRegistrationId}");
        }

        private static async Task CreateAndDeleteInstallationAsync(NotificationHubClient nhClient)
        {
            // Register some fake devices
            var gcmDeviceId = Guid.NewGuid().ToString();
            var gcmInstallation = new Installation
            {
                InstallationId = gcmDeviceId,
                Platform = NotificationPlatform.Gcm,
                PushChannel = gcmDeviceId,
                PushChannelExpired = false,
                Tags = new[] { "gcm" }
            };
            await nhClient.CreateOrUpdateInstallationAsync(gcmInstallation);

            while (true)
            {
                try
                {
                    var installationFromServer = await nhClient.GetInstallationAsync(gcmInstallation.InstallationId);
                    break;
                }
                catch (MessagingEntityNotFoundException)
                {
                    // Wait for installation to be created
                    await Task.Delay(1000);
                }
            }
            Console.WriteLine($"Created GCM installation {gcmInstallation.InstallationId}");
            await nhClient.DeleteInstallationAsync(gcmInstallation.InstallationId);
            while (true)
            {
                try
                {
                    var installationFromServer = await nhClient.GetInstallationAsync(gcmInstallation.InstallationId);
                    await Task.Delay(1000);
                }
                catch (MessagingEntityNotFoundException)
                {
                    Console.WriteLine($"Deleted GCM installation {gcmInstallation.InstallationId}");
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
