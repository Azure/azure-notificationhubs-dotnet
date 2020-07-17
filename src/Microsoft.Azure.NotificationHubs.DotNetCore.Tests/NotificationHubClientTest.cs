//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Extensions.Configuration;
    using Messaging;
    using Newtonsoft.Json;
    using Xunit;

    public class NotificationHubClientTest
    {
        private readonly IConfigurationRoot _configuration;
        private readonly NotificationHubClient _hubClient;
        private readonly TestServerProxy _testServer;

        public NotificationHubClientTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
            _testServer = new TestServerProxy();

            var settings = new NotificationHubClientSettings
            {
                MessageHandler = _testServer
            };

            if (_configuration["NotificationHubConnectionString"] != "<insert value here before running tests>")
            {
                _testServer.RecordingMode = true;
            }
            else
            { 
                _configuration["NotificationHubConnectionString"] = "Endpoint=sb://sample.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=xxxxxx";
                _configuration["NotificationHubName"] = "test";
            }
            _hubClient = new NotificationHubClient(_configuration["NotificationHubConnectionString"], _configuration["NotificationHubName"], settings);
        }

        Task Sleep(TimeSpan delay) => _testServer.RecordingMode ? Task.Delay(delay) : Task.FromResult(false);

        async Task DeleteAllRegistrationsAndInstallations()
        {
            string continuationToken;

            do
            {
                var registrations = await _hubClient.GetAllRegistrationsAsync(100);
                continuationToken = registrations.ContinuationToken;

                foreach (var registrationDescription in registrations)
                {
                   await _hubClient.DeleteRegistrationAsync(registrationDescription.RegistrationId);
                }
            } while (continuationToken != null);
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidAdmNativeRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AdmRegistrationDescription(_configuration["AdmDeviceToken"]);
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>(){"tag1"};

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string,string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.AdmRegistrationId, createdRegistration.AdmRegistrationId);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidAdmTemplateRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AdmTemplateRegistrationDescription(_configuration["AdmDeviceToken"], "{\"data\":{\"key1\":\"value1\"}}");
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>() { "tag1" };
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.AdmRegistrationId, createdRegistration.AdmRegistrationId);
            Assert.Equal(registration.BodyTemplate.Value, createdRegistration.BodyTemplate.Value);
            Assert.Equal(registration.TemplateName, createdRegistration.TemplateName);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidAppleNativeRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>() { "tag1" };

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.DeviceToken, createdRegistration.DeviceToken);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidAppleTemplateRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AppleTemplateRegistrationDescription(_configuration["AppleDeviceToken"], "{\"aps\":{\"alert\":\"alert!\"}}");
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>() { "tag1" };
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.DeviceToken, createdRegistration.DeviceToken);
            Assert.Equal(registration.BodyTemplate.Value, createdRegistration.BodyTemplate.Value);
            Assert.Equal(registration.TemplateName, createdRegistration.TemplateName);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidBaiduNativeRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new BaiduRegistrationDescription(_configuration["BaiduUserId"], _configuration["BaiduChannelId"], new []{"tag1"});
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.BaiduUserId, createdRegistration.BaiduUserId);
            Assert.Equal(registration.BaiduChannelId, createdRegistration.BaiduChannelId);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidBaiduTemplateRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new BaiduTemplateRegistrationDescription(_configuration["BaiduUserId"], _configuration["BaiduChannelId"], "{\"title\":\"Title\",\"description\":\"Description\"}", new[] { "tag1" });
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.BaiduUserId, createdRegistration.BaiduUserId);
            Assert.Equal(registration.BaiduChannelId, createdRegistration.BaiduChannelId);
            Assert.Equal(registration.BodyTemplate.Value, createdRegistration.BodyTemplate.Value);
            Assert.Equal(registration.TemplateName, createdRegistration.TemplateName);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidGcmNativeRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>() { "tag1" };

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.FcmRegistrationId, createdRegistration.FcmRegistrationId);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidGcmTemplateRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>() { "tag1" };
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.FcmRegistrationId, createdRegistration.FcmRegistrationId);
            Assert.Equal(registration.BodyTemplate.Value, createdRegistration.BodyTemplate.Value);
            Assert.Equal(registration.TemplateName, createdRegistration.TemplateName);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidMpnsNativeRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new MpnsRegistrationDescription(_configuration["MpnsDeviceToken"]);
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>() { "tag1" };
            registration.SecondaryTileName = "Tile name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.ChannelUri, createdRegistration.ChannelUri);
            Assert.Equal(registration.SecondaryTileName, createdRegistration.SecondaryTileName);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidMpnsTemplateRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new MpnsTemplateRegistrationDescription(_configuration["MpnsDeviceToken"], "<wp:Notification xmlns:wp=\"WPNotification\" Version=\"2.0\"><wp:Tile Id=\"TileId\" Template=\"IconicTile\"><wp:Title Action=\"Clear\">Title</wp:Title></wp:Tile></wp:Notification>", new []{ "tag1" });
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.SecondaryTileName = "Tile name";
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.ChannelUri, createdRegistration.ChannelUri);
            Assert.Equal(registration.SecondaryTileName, createdRegistration.SecondaryTileName);
            Assert.Equal(registration.BodyTemplate.Value, createdRegistration.BodyTemplate.Value);
            Assert.Equal(registration.TemplateName, createdRegistration.TemplateName);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidWindowsNativeRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new WindowsRegistrationDescription(_configuration["WindowsDeviceToken"]);
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>() { "tag1" };
            registration.SecondaryTileName = "Tile name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.ChannelUri, createdRegistration.ChannelUri);
            Assert.Equal(registration.SecondaryTileName, createdRegistration.SecondaryTileName);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_PassValidWindowsTemplateRegistration_GetCreatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new WindowsTemplateRegistrationDescription(_configuration["WindowsDeviceToken"], "<toast><visual><binding template=\"ToastText01\"><text id=\"1\">bodyText</text></binding>  </visual></toast>", new[] { "tag1" });
            registration.PushVariables = new Dictionary<string, string>()
            {
                {"var1", "value1"}
            };
            registration.Tags = new HashSet<string>() { "tag1" };
            registration.SecondaryTileName = "Tile name";
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.NotNull(createdRegistration.RegistrationId);
            Assert.NotNull(createdRegistration.ETag);
            Assert.NotNull(createdRegistration.ExpirationTime);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdRegistration.PushVariables);
            Assert.Contains("tag1", createdRegistration.Tags);
            Assert.Equal(registration.ChannelUri, createdRegistration.ChannelUri);
            Assert.Equal(registration.SecondaryTileName, createdRegistration.SecondaryTileName);
            Assert.Equal(registration.BodyTemplate.Value, createdRegistration.BodyTemplate.Value);
            Assert.Equal(registration.TemplateName, createdRegistration.TemplateName);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateOrUpdateRegistrationAsync_UpsertAppleNativeRegistration_GetUpsertedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            createdRegistration.Tags = new HashSet<string>() { "tag1" };

            var updatedRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(createdRegistration);

            Assert.Contains("tag1", updatedRegistration.Tags);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateOrUpdateRegistrationAsync_UpsertAppleNativeRegistrationWithCustomId_GetUpsertedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);

            registration.RegistrationId = "123-234-1";

            var createdRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(registration);

            Assert.Equal(registration.RegistrationId, createdRegistration.RegistrationId);
            RecordTestResults();
        }

        [Fact]
        public async Task GetAllRegistrationsAsync_CreateTwoRegistrations_GetAllCreatedRegistrations()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var appleRegistration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);
            var fcmRegistration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);

            var createdAppleRegistration = await _hubClient.CreateRegistrationAsync(appleRegistration);
            var createdFcmRegistration = await _hubClient.CreateRegistrationAsync(fcmRegistration);

            var allRegistrations = await _hubClient.GetAllRegistrationsAsync(100);
            var allRegistrationIds = allRegistrations.Select(r => r.RegistrationId).ToArray();

            Assert.Equal(2, allRegistrationIds.Count());
            Assert.Contains(createdAppleRegistration.RegistrationId, allRegistrationIds);
            Assert.Contains(createdFcmRegistration.RegistrationId, allRegistrationIds);
            RecordTestResults();
        }

        [Fact]
        public async Task GetAllRegistrationsAsync_UsingTopLessThenNumberOfRegistrations_CorrectContinuationTokenIsReturnedThatAllowsToReadAllRegistrations()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var appleRegistration1 = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);
            var appleRegistration2 = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);
            var appleRegistration3 = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);

            await _hubClient.CreateRegistrationAsync(appleRegistration1);
            await _hubClient.CreateRegistrationAsync(appleRegistration2);
            await _hubClient.CreateRegistrationAsync(appleRegistration3);

            string continuationToken = null;
            var allRegistrationIds = new List<string>();
            var numberOfCalls = 0;

            do
            {
                var registrations = await (continuationToken == null ? _hubClient.GetAllRegistrationsAsync(2) : _hubClient.GetAllRegistrationsAsync(continuationToken, 2));
                continuationToken = registrations.ContinuationToken;
                allRegistrationIds.AddRange(registrations.Select(r => r.RegistrationId));
                numberOfCalls++;
            } while (continuationToken != null);

            Assert.Equal(2, numberOfCalls);
            Assert.Equal(3, allRegistrationIds.Count);
            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationsByChannelAsync_CreateTwoRegistrationsWithTheSameChannel_GetCreatedRegistrationsWithRequestedChannel()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var appleRegistration1 = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);
            var appleRegistration2 = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);
            var fcmRegistration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);

            var createdAppleRegistration1 = await _hubClient.CreateRegistrationAsync(appleRegistration1);
            var createdAppleRegistration2 = await _hubClient.CreateRegistrationAsync(appleRegistration2);
            // Create a registration with another channel to make sure that SDK passes correct channel and two registrations will be returned
            var createdFcmRegistration = await _hubClient.CreateRegistrationAsync(fcmRegistration);

            var allRegistrations = await _hubClient.GetRegistrationsByChannelAsync(_configuration["AppleDeviceToken"], 100);
            var allRegistrationIds = allRegistrations.Select(r => r.RegistrationId).ToArray();

            Assert.Equal(2, allRegistrationIds.Count());
            Assert.Contains(createdAppleRegistration1.RegistrationId, allRegistrationIds);
            Assert.Contains(createdAppleRegistration2.RegistrationId, allRegistrationIds);
            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationsByTagAsync_CreateTwoRegistrationsWithTheSameTag_GetCreatedRegistrationsWithRequestedTag()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var appleRegistration1 = new AppleRegistrationDescription(_configuration["AppleDeviceToken"], new []{ "tag1" });
            var appleRegistration2 = new AppleRegistrationDescription(_configuration["AppleDeviceToken"], new[] { "tag1" });
            var fcmRegistration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);

            var createdAppleRegistration1 = await _hubClient.CreateRegistrationAsync(appleRegistration1);
            var createdAppleRegistration2 = await _hubClient.CreateRegistrationAsync(appleRegistration2);
            // Create a registration with another channel to make sure that SDK passes correct tag and two registrations will be returned
            var createdFcmRegistration = await _hubClient.CreateRegistrationAsync(fcmRegistration);

            var allRegistrations = await _hubClient.GetRegistrationsByTagAsync("tag1", 100);
            var allRegistrationIds = allRegistrations.Select(r => r.RegistrationId).ToArray();

            Assert.Equal(2, allRegistrationIds.Count());
            Assert.Contains(createdAppleRegistration1.RegistrationId, allRegistrationIds);
            Assert.Contains(createdAppleRegistration2.RegistrationId, allRegistrationIds);
            RecordTestResults();
        }

        [Fact]
        public async Task RegistrationExistsAsync_GetExistingRegistration()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var appleRegistration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"], new []{ "tag1" });
            var createdRegistration = await _hubClient.CreateRegistrationAsync(appleRegistration);

            var registrationExists = await _hubClient.RegistrationExistsAsync(createdRegistration.RegistrationId);
            Assert.True(registrationExists);
        }

        [Fact]
        public async Task RegistrationExistsAsync_GetNonExistingRegistration()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var appleRegistration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"], new []{ "tag1" });
            var createdRegistration = await _hubClient.CreateRegistrationAsync(appleRegistration);

            var registrationExists = await _hubClient.RegistrationExistsAsync(createdRegistration.RegistrationId);
            Assert.True(registrationExists);

            await _hubClient.DeleteRegistrationAsync(createdRegistration.RegistrationId);

            registrationExists = await _hubClient.RegistrationExistsAsync(createdRegistration.RegistrationId);
            Assert.False(registrationExists);
        }

        [Fact]
        public async Task UpdateRegistrationAsync_UpdateAppleNativeRegistration_GetUpdatedRegistrationBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            createdRegistration.Tags = new HashSet<string>(){"tag1"};

            var updatedRegistration = await _hubClient.UpdateRegistrationAsync(createdRegistration);

            Assert.Contains("tag1", updatedRegistration.Tags);
            RecordTestResults();
        }

        [Fact]
        public async Task DeleteRegistrationAsync_DeleteAppleNativeRegistration_RegistrationIsDeleted()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            await _hubClient.DeleteRegistrationAsync(createdRegistration);

            await Assert.ThrowsAsync<MessagingEntityNotFoundException>(async () => await _hubClient.GetRegistrationAsync<AppleRegistrationDescription>(createdRegistration.RegistrationId));
            RecordTestResults();
        }

        [Fact]
        public async Task DeleteRegistrationAsync_DeleteAppleNativeRegistrationByRegistrationId_RegistrationIsDeleted()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            await _hubClient.DeleteRegistrationAsync(createdRegistration.RegistrationId);

            await Assert.ThrowsAsync<MessagingEntityNotFoundException>(async () => await _hubClient.GetRegistrationAsync<AppleRegistrationDescription>(createdRegistration.RegistrationId));
            RecordTestResults();
        }

        [Fact]
        public async Task DeleteRegistrationsByChannelAsync_DeleteAppleNativeRegistrationByChannel_RegistrationIsDeleted()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new AppleRegistrationDescription(_configuration["AppleDeviceToken"]);

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            await _hubClient.DeleteRegistrationsByChannelAsync(_configuration["AppleDeviceToken"]);

            await Assert.ThrowsAsync<MessagingEntityNotFoundException>(async () => await _hubClient.GetRegistrationAsync<AppleRegistrationDescription>(createdRegistration.RegistrationId));
            RecordTestResults();
        }

        [Fact]
        private async Task CreateRegistrationIdAsync_CallMethod_GetRegistrationId()
        {
            LoadMockData();
            var registrationId = await _hubClient.CreateRegistrationIdAsync();

            Assert.NotNull(registrationId);
            RecordTestResults();
        }

        [Fact]
        private async Task CreateOrUpdateInstallationAsync_CreateInstallationWithExpiryInTemplate_GetCreatedInstallationWithExpiryInTemplateBack()
        {
            LoadMockData();

            await DeleteAllRegistrationsAndInstallations();

            var installationId = _testServer.NewGuid().ToString();

            var installation = new Installation
            {
                InstallationId = installationId,
                Platform = NotificationPlatform.Apns,
                PushChannel = _configuration["AppleDeviceToken"],
                PushVariables = new Dictionary<string, string> { { "var1", "value1" } },
                Tags = new List<string> { "tag1" },
                Templates = new Dictionary<string, InstallationTemplate>
                {
                    {
                        "Template Name", new InstallationTemplate
                        {
                            Body = "{\"aps\":{\"alert\":\"alert!\"}}",
                            Expiry = DateTime.Now.AddDays(1).ToString("o")
                        }
                    }
                }
            };

            await _hubClient.CreateOrUpdateInstallationAsync(installation);

            await Sleep(TimeSpan.FromSeconds(1));

            var createdInstallation = await _hubClient.GetInstallationAsync(installationId);

            Assert.Equal(installation.InstallationId, createdInstallation.InstallationId);
            Assert.Equal(installation.Platform, createdInstallation.Platform);
            Assert.Equal(installation.PushChannel, createdInstallation.PushChannel);
            Assert.Contains(new KeyValuePair<string, string>("var1", "value1"), createdInstallation.PushVariables);
            Assert.Contains("tag1", createdInstallation.Tags);
            Assert.Contains("Template Name", createdInstallation.Templates.Keys);
            Assert.Equal(installation.Templates["Template Name"].Body, createdInstallation.Templates["Template Name"].Body);

            RecordTestResults();
        }
        

        [Fact]
        private async Task CreateOrUpdateInstallationAsync_CreateInstallationWithHeadersInTemplate_GetCreatedInstallationWithHeadersInTemplateBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var installationId = Guid.NewGuid().ToString();

            var installation = new Installation
            {
                InstallationId = installationId,
                Platform = NotificationPlatform.Wns,
                PushChannel = _configuration["WindowsDeviceToken"],
                Templates = new Dictionary<string, InstallationTemplate>
                {
                    {
                        "Template Name", new InstallationTemplate
                        {
                            Body = "{\"aps\":{\"alert\":\"alert!\"}}",
                            Headers = new Dictionary<string, string>
                            {
                                {"X-WNS-Type", "wns/toast"}
                            }
                        }
                    }
                }
            };

            await _hubClient.CreateOrUpdateInstallationAsync(installation);

            await Sleep(TimeSpan.FromSeconds(1));

            var createdInstallation = await _hubClient.GetInstallationAsync(installationId);

            Assert.Contains("Template Name", createdInstallation.Templates.Keys);
            Assert.Contains(new KeyValuePair<string, string>("X-WNS-Type", "wns/toast"), createdInstallation.Templates["Template Name"].Headers);
            RecordTestResults();
        }

        [Fact]
        private async Task PatchInstallationAsync_CreateInstallationWithoutTagsAndAddTagThroughPatch_GetInstallationWithAddedTagBack()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var installationId = Guid.NewGuid().ToString();

            var installation = new Installation
            {
                InstallationId = installationId,
                Platform = NotificationPlatform.Apns,
                PushChannel = _configuration["AppleDeviceToken"]
            };

            await _hubClient.CreateOrUpdateInstallationAsync(installation);

            await _hubClient.PatchInstallationAsync(installationId, new List<PartialUpdateOperation>
            {
                new PartialUpdateOperation()
                {
                    Operation = UpdateOperationType.Add,
                    Path = "/tags",
                    Value = "tag1"
                }
            });

            await Sleep(TimeSpan.FromSeconds(1));

            var updatedInstallation = await _hubClient.GetInstallationAsync(installationId);

            Assert.Contains("tag1", updatedInstallation.Tags);
            RecordTestResults();
        }

        [Fact]
        private async Task DeleteInstallationAsync_CreateAndDeleteInstallation_InstallationIsDeleted()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var installationId = Guid.NewGuid().ToString();

            var installation = new Installation
            {
                InstallationId = installationId,
                Platform = NotificationPlatform.Apns,
                PushChannel = _configuration["AppleDeviceToken"]
            };

            await _hubClient.CreateOrUpdateInstallationAsync(installation);

            await Sleep(TimeSpan.FromSeconds(1));

            var createdInstallation = await _hubClient.GetInstallationAsync(installationId);

            await _hubClient.DeleteInstallationAsync(createdInstallation.InstallationId);

            await Sleep(TimeSpan.FromSeconds(1));

            await Assert.ThrowsAsync<MessagingEntityNotFoundException>(async () => await _hubClient.GetInstallationAsync(createdInstallation.InstallationId));
            RecordTestResults();
        }

        [Fact]
        private async Task InstallationExists_ChecksInstallation()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var installationId = Guid.NewGuid().ToString();

            var installation = new Installation
            {
                InstallationId = installationId,
                Platform = NotificationPlatform.Apns,
                PushChannel = _configuration["AppleDeviceToken"]
            };

            await _hubClient.CreateOrUpdateInstallationAsync(installation);

            await Sleep(TimeSpan.FromSeconds(1));

            Assert.True(await _hubClient.InstallationExistsAsync(installationId));

            await _hubClient.DeleteInstallationAsync(installationId);

            await Sleep(TimeSpan.FromSeconds(1));

            Assert.False(await _hubClient.InstallationExistsAsync(installationId));
            RecordTestResults();
        }

        [Fact]
        private async Task SendNotificationAsync_SendAdmNativeNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new AdmNotification("{\"data\":{\"key1\":\"value1\"}}");

            var notificationResult = await _hubClient.SendNotificationAsync(notification, "someRandomTag1 && someRandomTag2");

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendNotificationAsync_SendAppleNativeNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new AppleNotification("{\"aps\":{\"alert\":\"alert!\"}}");
            notification.Expiry = DateTime.Now.AddDays(1);
            notification.Priority = 5;

            var notificationResult = await _hubClient.SendNotificationAsync(notification, "someRandomTag1 && someRandomTag2");

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendNotificationAsync_SendBaiduNativeNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new BaiduNotification("{\"title\":\"Title\",\"description\":\"Description\"}");
            notification.MessageType = 1;

            var notificationResult = await _hubClient.SendNotificationAsync(notification, "someRandomTag1 && someRandomTag2");

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendNotificationAsync_SendGcmNativeNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new FcmNotification("{\"data\":{\"message\":\"Message\"}}");

            var notificationResult = await _hubClient.SendNotificationAsync(notification, "someRandomTag1 && someRandomTag2");

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendDirectNotificationAsync_SendDirectFcmBatchNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new FcmNotification("{\"data\":{\"message\":\"Message\"}}");

            var notificationResult = await _hubClient.SendDirectNotificationAsync(notification, new[] { _configuration["FcmDeviceToken"] });

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendNotificationAsync_SendMpnsNativeNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new MpnsNotification("<wp:Notification xmlns:wp=\"WPNotification\" Version=\"2.0\"><wp:Tile Id=\"TileId\" Template=\"IconicTile\"><wp:Title Action=\"Clear\">Title</wp:Title></wp:Tile></wp:Notification>");

            var notificationResult = await _hubClient.SendNotificationAsync(notification, "someRandomTag1 && someRandomTag2");

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendNotificationAsync_SendWindowsNativeNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new WindowsNotification("<toast><visual><binding template=\"ToastText01\"><text id=\"1\">bodyText</text></binding>  </visual></toast>");

            var notificationResult = await _hubClient.SendNotificationAsync(notification, "someRandomTag1 && someRandomTag2");

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendNotificationAsync_SendTemplateNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new TemplateNotification(new Dictionary<string, string>());

            var notificationResult = await _hubClient.SendNotificationAsync(notification, "someRandomTag1 && someRandomTag2");

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendDirectNotificationAsync_SendDirectAppleNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new AppleNotification("{\"aps\":{\"alert\":\"alert!\"}}");

            var notificationResult = await _hubClient.SendDirectNotificationAsync(notification, _configuration["AppleDeviceToken"]);

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task SendDirectNotificationAsync_SendDirectAppleBatchNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new AppleNotification("{\"aps\":{\"alert\":\"alert!\"}}");
            notification.Expiry = DateTime.Now.AddDays(1);
            notification.Priority = 5;

            var notificationResult = await _hubClient.SendDirectNotificationAsync(notification, new [] {_configuration["AppleDeviceToken"]});

            Assert.Equal(NotificationOutcomeState.Enqueued, notificationResult.State);
            RecordTestResults();
        }

        [Fact]
        private async Task ScheduleNotificationAsync_SendAppleNativeNotification_GetSuccessfulResultBack()
        {
            LoadMockData();
            var notification = new AppleNotification("{\"aps\":{\"alert\":\"alert!\"}}");
            notification.Expiry = DateTime.Now.AddDays(1);
            notification.Priority = 5;

            var notificationResult = await _hubClient.ScheduleNotificationAsync(notification, DateTimeOffset.Now.AddDays(1));

            Assert.NotNull(notificationResult.ScheduledNotificationId);
            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationAsync_CreateFcmNativeRegistrationThenGetRegistrationDescription_GetFcmRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);
            var receivedRegistration = await _hubClient.GetRegistrationAsync<RegistrationDescription>(createdRegistration.RegistrationId);

            Assert.IsType<FcmRegistrationDescription>(receivedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationAsync_CreateFcmNativeRegistrationThenGetFcmRegistrationDescription_GetFcmRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);
            var receivedRegistration = await _hubClient.GetRegistrationAsync<FcmRegistrationDescription>(createdRegistration.RegistrationId);

            Assert.IsType<FcmRegistrationDescription>(receivedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationAsync_CreateGcmNativeRegistrationThenGetGcmRegistrationDescription_GetGcmRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new GcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);
            var receivedRegistration = await _hubClient.GetRegistrationAsync<GcmRegistrationDescription>(createdRegistration.RegistrationId);

            Assert.IsType<GcmRegistrationDescription>(receivedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationAsync_CreateFcmTemplateRegistrationThenGetRegistrationDescription_GetFcmTemplateRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);
            var receivedRegistration = await _hubClient.GetRegistrationAsync<RegistrationDescription>(createdRegistration.RegistrationId);
            
            Assert.IsType<FcmTemplateRegistrationDescription>(receivedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationAsync_CreateFcmTemplateRegistrationThenGetFcmTemplateRegistrationDescription_GetFcmTemplateRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);
            var receivedRegistration = await _hubClient.GetRegistrationAsync<FcmTemplateRegistrationDescription>(createdRegistration.RegistrationId);

            Assert.IsType<FcmTemplateRegistrationDescription>(receivedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationAsync_CreateGcmTemplateRegistrationThenGetGcmTemplateRegistrationDescription_GetGcmTemplateRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new GcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);
            var receivedRegistration = await _hubClient.GetRegistrationAsync<GcmTemplateRegistrationDescription>(createdRegistration.RegistrationId);

            Assert.IsType<GcmTemplateRegistrationDescription>(receivedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_CreateGcmNativeRegistration_GetGcmRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new GcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.IsType<GcmRegistrationDescription>(createdRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_CreateFcmNativeRegistration_GetFcmRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.IsType<FcmRegistrationDescription>(createdRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_CreateGcmTemplateRegistration_GetGcmTemplateRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new GcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.IsType<GcmTemplateRegistrationDescription>(createdRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateRegistrationAsync_CreateFcmTemplateRegistration_GetFcmTemplateRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            Assert.IsType<FcmTemplateRegistrationDescription>(createdRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateOrUpdateRegistrationAsync_UpdateGcmNativeRegistration_GetGcmRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new GcmRegistrationDescription(_configuration["GcmDeviceToken"]);

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            createdRegistration.Tags = new HashSet<string>() { "tag2" };
            var updatedRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(createdRegistration);

            Assert.IsType<GcmRegistrationDescription>(updatedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateOrUpdateRegistrationAsync_UpdateFcmNativeRegistration_GetFcmRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            registration.Tags = new HashSet<string>() { "tag1" };

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            createdRegistration.Tags = new HashSet<string>() { "tag2" };
            var updatedRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(createdRegistration);

            Assert.IsType<FcmRegistrationDescription>(updatedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateOrUpdateRegistrationAsync_UpdateGcmTemplateRegistration_GetGcmTemplateRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new GcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            registration.Tags = new HashSet<string>() { "tag1" };
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            createdRegistration.Tags = new HashSet<string>() { "tag2" };
            var updatedRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(createdRegistration);

            Assert.IsType<GcmTemplateRegistrationDescription>(updatedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task CreateOrUpdateRegistrationAsync_UpdateFcmTemplateRegistration_GetFcmTemplateRegistrationType()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var registration = new FcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            registration.Tags = new HashSet<string>() { "tag1" };
            registration.TemplateName = "Template Name";

            var createdRegistration = await _hubClient.CreateRegistrationAsync(registration);

            createdRegistration.Tags = new HashSet<string>() { "tag2" };
            var updatedRegistration = await _hubClient.CreateOrUpdateRegistrationAsync(createdRegistration);

            Assert.IsType<FcmTemplateRegistrationDescription>(updatedRegistration);
            RecordTestResults();
        }

        [Fact]
        public async Task GetAllRegistrationsAsync_CreateGcmAndFcmRegistrations_GetTwoFcmCreatedRegistrations()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var gcmRegistration = new GcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            var fcmRegistration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);

            await _hubClient.CreateRegistrationAsync(gcmRegistration);
            await _hubClient.CreateRegistrationAsync(fcmRegistration);

            var allRegistrations = await _hubClient.GetAllRegistrationsAsync(100);
            
            foreach(var registration in allRegistrations)
            {
                Assert.IsType<FcmRegistrationDescription>(registration);
            }

            RecordTestResults();
        }

        [Fact]
        public async Task GetAllRegistrationsAsync_CreateGcmAndFcmTemplateRegistrations_GetTwoFcmTemplateCreatedRegistrations()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var gcmTemplateRegistration = new GcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            gcmTemplateRegistration.TemplateName = "Gcm Template Name";

            var fcmTemplateRegistration = new FcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            fcmTemplateRegistration.TemplateName = "Fcm Template Name";

            await _hubClient.CreateRegistrationAsync(gcmTemplateRegistration);
            await _hubClient.CreateRegistrationAsync(fcmTemplateRegistration);

            var allRegistrations = await _hubClient.GetAllRegistrationsAsync(100);

            foreach (var registration in allRegistrations)
            {
                Assert.IsType<FcmTemplateRegistrationDescription>(registration);
            }

            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationsByTagAsync_CreateGcmAndFcmRegistrations_GetTwoFcmCreatedRegistrations()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var gcmRegistration = new GcmRegistrationDescription(_configuration["GcmDeviceToken"], new[] { "tag1" });
            var fcmRegistration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"], new[] { "tag1" });

            await _hubClient.CreateRegistrationAsync(gcmRegistration);
            await _hubClient.CreateRegistrationAsync(fcmRegistration);

            var allRegistrations = await _hubClient.GetRegistrationsByTagAsync("tag1", 100);

            foreach (var registration in allRegistrations)
            {
                Assert.IsType<FcmRegistrationDescription>(registration);
            }

            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationsByTagAsync_CreateGcmAndFcmTemplateRegistrations_GetTwoFcmTemplateCreatedRegistrations()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var gcmTemplateRegistration = new GcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            gcmTemplateRegistration.Tags = new HashSet<string>() { "tag2" };
            gcmTemplateRegistration.TemplateName = "Gcm Template Name";

            var fcmTemplateRegistration = new FcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            fcmTemplateRegistration.Tags = new HashSet<string>() { "tag2" };
            fcmTemplateRegistration.TemplateName = "Fcm Template Name";

            await _hubClient.CreateRegistrationAsync(gcmTemplateRegistration);
            await _hubClient.CreateRegistrationAsync(fcmTemplateRegistration);

            var allRegistrations = await _hubClient.GetRegistrationsByTagAsync("tag2", 100);

            foreach (var registration in allRegistrations)
            {
                Assert.IsType<FcmTemplateRegistrationDescription>(registration);
            }

            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationsByChannelAsync_CreateGcmAndFcmRegistrations_GetTwoFcmCreatedRegistrations()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var gcmRegistration = new GcmRegistrationDescription(_configuration["GcmDeviceToken"]);
            var fcmRegistration = new FcmRegistrationDescription(_configuration["GcmDeviceToken"]);

            await _hubClient.CreateRegistrationAsync(gcmRegistration);
            await _hubClient.CreateRegistrationAsync(fcmRegistration);

            var allRegistrations = await _hubClient.GetRegistrationsByChannelAsync(_configuration["GcmDeviceToken"], 100);

            foreach (var registration in allRegistrations)
            {
                Assert.IsType<FcmRegistrationDescription>(registration);
            }

            RecordTestResults();
        }

        [Fact]
        public async Task GetRegistrationsByChannelAsync_CreateGcmAndFcmTemplateRegistrations_GetTwoFcmTemplateCreatedRegistrations()
        {
            LoadMockData();
            await DeleteAllRegistrationsAndInstallations();

            var gcmTemplateRegistration = new GcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            gcmTemplateRegistration.TemplateName = "Gcm Template Name";

            var fcmTemplateRegistration = new FcmTemplateRegistrationDescription(_configuration["GcmDeviceToken"], "{\"data\":{\"message\":\"Message\"}}");
            fcmTemplateRegistration.TemplateName = "Fcm Template Name";

            await _hubClient.CreateRegistrationAsync(gcmTemplateRegistration);
            await _hubClient.CreateRegistrationAsync(fcmTemplateRegistration);

            var allRegistrations = await _hubClient.GetRegistrationsByChannelAsync(_configuration["GcmDeviceToken"], 100);

            foreach (var registration in allRegistrations)
            {
                Assert.IsType<FcmTemplateRegistrationDescription>(registration);
            }

            RecordTestResults();
        }

        private string GetMockDataFilePath(string methodName)
        {
            string[] dataFilePaths = new string[]
            {
                $"{methodName}.http",
                Path.Combine("MockData", $"{methodName}.http"),
            };

            foreach (var dataFilePath in dataFilePaths)
            {
                if (File.Exists(dataFilePath))
                {
                    return dataFilePath;
                }
            }
            return null;
        }

        private void LoadMockData([CallerMemberName]string methodName = "")
        {
            if (!_testServer.RecordingMode)
            {
                string filePath = GetMockDataFilePath(methodName);
                if (filePath == null)
                {
                    throw new Exception($"Cannot find data file for method '{methodName}'. Test data must be recorded first.");
                }

                var payloads = JsonConvert.DeserializeObject<TestServerSession>(File.ReadAllText(filePath));

                _testServer.LoadResponses(payloads);
                _testServer.BaseUri = "http://test";
            }
        }

        private void RecordTestResults([CallerMemberName]string methodName = "")
        {
            if (_testServer.RecordingMode)
            {
                File.WriteAllText($"{methodName}.http", JsonConvert.SerializeObject(_testServer.Session));
            }
        }
    }
}
