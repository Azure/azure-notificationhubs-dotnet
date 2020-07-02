using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Azure.NotificationHubs.Auth;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Azure.NotificationHubs.Tests;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.NotificationHubs.DotNetCore.Tests
{
    public class ManagementApiE2ETests
    {
        private const string IncorrectConnectionString = "Endpoint=sb://sample-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SharedAccessKey";
        private const string NotificationHubNamespaceUriString = "NotificationHubNamespaceUriString";
        private const string NotificationHubConnectionString = "NotificationHubConnectionString";
        private const string NotificationHubName = "NotificationHubName";
        private const string InputFileName = "ImportRegistrations.txt";

        private const string StorageAccount = "StorageAccount";
        private const string StoragePassword = "StoragePassword";
        private const string StorageEndpointString = "StorageEndpointString";
        private const string ContainerName = "ContainerName";

        private NamespaceManager _namespaceManager;
        private NamespaceManagerSettings _namespaceManagerSettings;
        private static StorageUri _storageEndpoint;
        private readonly string _notificationHubName;

        private readonly string _storageAccount;
        private readonly string _storagePassword;
        private readonly string _storageEndpointAddress;
        private readonly string _containerName;

        private readonly TestServerProxy _testServer;
        private string _namespaceUriString;
        private string _notificationHubConnectionString;

        public ManagementApiE2ETests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _namespaceUriString = Environment.GetEnvironmentVariable(NotificationHubNamespaceUriString.ToUpperInvariant()) ?? configuration[NotificationHubNamespaceUriString];
            _notificationHubConnectionString = Environment.GetEnvironmentVariable(NotificationHubConnectionString.ToUpperInvariant()) ?? configuration[NotificationHubConnectionString];
            _notificationHubName = Environment.GetEnvironmentVariable(NotificationHubName.ToUpperInvariant()) ?? configuration[NotificationHubName];

            _storageAccount = Environment.GetEnvironmentVariable(StorageAccount.ToUpperInvariant()) ?? configuration[StorageAccount];
            _storagePassword = Environment.GetEnvironmentVariable(StoragePassword.ToUpperInvariant()) ?? configuration[StoragePassword];
            _storageEndpointAddress = Environment.GetEnvironmentVariable(StorageEndpointString.ToUpperInvariant()) ?? configuration[StorageEndpointString];
            _containerName = Environment.GetEnvironmentVariable(ContainerName.ToUpperInvariant()) ?? configuration[ContainerName];


            _testServer = new TestServerProxy();
            if (_notificationHubConnectionString != "<insert value here before running tests>")
            {
                _testServer.RecordingMode = true;
            }
            else
            {
                _notificationHubConnectionString = "Endpoint=sb://sample.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=xxxxxx";
                _notificationHubName = "test";
            }

            _namespaceManagerSettings = new NamespaceManagerSettings();
            _namespaceManagerSettings.TokenProvider = SharedAccessSignatureTokenProvider.CreateSharedAccessSignatureTokenProvider(_notificationHubConnectionString);
            _namespaceManagerSettings.MessageHandler = _testServer;
            _namespaceManager = new NamespaceManager(new Uri(_namespaceUriString), _namespaceManagerSettings);

            _storageEndpoint = new StorageUri(new Uri(_storageEndpointAddress));
            CleanUp();
        }

        [Fact]
        public void ExecuteCreateGetAndDeleteNotificationHubMethods_ShouldCreateGetOrDeleteHubOrRecieveConflictException()
        {
            LoadMockData();
            try
            {
                bool notificationHubExists = true;
                IEnumerable<NotificationHubDescription> notificationHubDescriptions;

                // Check that GetNotification returns MessagingEntityNotFoundException than hub does not exist
                Assert.Throws<MessagingEntityNotFoundException>(() => _namespaceManager.GetNotificationHub(_notificationHubName));

                // Check that NotificationHubExists returns false when notification hub does not exist
                notificationHubExists = _namespaceManager.NotificationHubExists(_notificationHubName);
                Assert.False(notificationHubExists);

                // Check that GetNotificationHubs returns collection without hub
                notificationHubDescriptions = _namespaceManager.GetNotificationHubs();
                Assert.DoesNotContain(notificationHubDescriptions, nhd => nhd.Path == _notificationHubName);

                // Check that CreateNotificationHub method create hub with correct Path
                var createNotificationHubDescription = _namespaceManager.CreateNotificationHub(_notificationHubName);
                Assert.Equal(_notificationHubName, createNotificationHubDescription.Path);

                // Check that NotificationHubExists return true when notification hub exist
                notificationHubExists = _namespaceManager.NotificationHubExists(_notificationHubName);
                Assert.True(notificationHubExists);

                // Check that GetNotificationHubs returns collection with existed hub
                notificationHubDescriptions = _namespaceManager.GetNotificationHubs();
                Assert.Contains(notificationHubDescriptions, nhd => nhd.Path == _notificationHubName);

                // Check that CreateNotificationHub returns MessagingEntityAlreadyExistsException than hub is alredy exist
                Assert.Throws<MessagingEntityAlreadyExistsException>(() => _namespaceManager.CreateNotificationHub(_notificationHubName));

                // Check that GetNotificationHub returns correct hub
                var getNotificationHubDescription = _namespaceManager.GetNotificationHub(_notificationHubName);
                Assert.Equal(_notificationHubName, getNotificationHubDescription.Path);

                // Check that UpdateNotificationHub correctly update hub
                createNotificationHubDescription.IsDisabled = true;
                var updatedNotificationHubDescription = _namespaceManager.UpdateNotificationHub(createNotificationHubDescription);
                Assert.True(updatedNotificationHubDescription.IsDisabled);

                // Check that DeleteNotificationHub correctly remove hub
                _namespaceManager.DeleteNotificationHub(_notificationHubName);

                // Check that NotificationHubExists return false when notification hub is not exist
                notificationHubExists = _namespaceManager.NotificationHubExists(_notificationHubName);
                Assert.False(notificationHubExists);

                // Check that GetNotificationHubs returns collection without not existed hub
                notificationHubDescriptions = _namespaceManager.GetNotificationHubs();
                Assert.DoesNotContain(notificationHubDescriptions, nhd => nhd.Path == _notificationHubName);

                // Check that DeleteNotificationHub returns MessagingEntityNotFoundException than hub is not exist
                Assert.Throws<MessagingEntityNotFoundException>(() => _namespaceManager.DeleteNotificationHub(_notificationHubName));
            }
            finally
            {
                RecordTestResults();
            }
        }

        [Fact]
        public async void SubmitAndGetNotificationHubJob_ShouldReceiveCorrectJobs()
        {
            LoadMockData();
            try
            {
                _namespaceManager.CreateNotificationHub(_notificationHubName);

                var blobClient = new CloudBlobClient(
                    _storageEndpoint,
                    new StorageCredentials(_storageAccount, _storagePassword));

                var container = blobClient.GetContainerReference(_containerName);

                var outputContainerSasUri = GetOutputDirectoryUrl(container);
                var inputFileSasUri = GetInputFileUrl(container, InputFileName);

                var notificationHubJob = new NotificationHubJob
                {
                    JobType = NotificationHubJobType.ImportCreateRegistrations,
                    OutputContainerUri = outputContainerSasUri,
                    ImportFileUri = inputFileSasUri
                };

                var submitedNotificationHubJob
                    = await _namespaceManager.SubmitNotificationHubJobAsync(notificationHubJob, _notificationHubName);
                Assert.NotNull(submitedNotificationHubJob);
                Assert.NotEmpty(submitedNotificationHubJob.JobId);

                var recievedNotificationHubJob
                    = await _namespaceManager.GetNotificationHubJobAsync(submitedNotificationHubJob.JobId, _notificationHubName);

                Assert.Equal(submitedNotificationHubJob.JobId, recievedNotificationHubJob.JobId);
            }
            finally
            {
                RecordTestResults();
            }
        }

        [Fact]
        public void ExecuteCreateGetAndDeleteNotificationHubMethodsWithIncorrectConnectionString_ShouldRecieveUnauthorizedAccessException()
        {
            var namespaceManagerSettings = new NamespaceManagerSettings();
            namespaceManagerSettings.TokenProvider
                = SharedAccessSignatureTokenProvider.CreateSharedAccessSignatureTokenProvider(IncorrectConnectionString);

            var namespaceManager = new NamespaceManager(new Uri(_namespaceUriString), namespaceManagerSettings);

            // Check that CreateNotificationHub returns UnauthorizedAccessException when connection string is incorrect
            Assert.Throws<UnauthorizedAccessException>(() => namespaceManager.CreateNotificationHub(_notificationHubName));

            // We must create hub to recieve UnauthorizedAccessException when GetNotificationHub and DeleteNotificationHub execute
            var notificationHubDescription = _namespaceManager.CreateNotificationHub(_notificationHubName);

            // Check that GetNotificationHub returns UnauthorizedAccessException when connection string is incorrect
            Assert.Throws<UnauthorizedAccessException>(() => namespaceManager.GetNotificationHub(_notificationHubName));

            // Check that NotificationHubExists returns UnauthorizedAccessException when connection string is incorrect
            Assert.Throws<UnauthorizedAccessException>(() => namespaceManager.NotificationHubExists(_notificationHubName));

            // Check that UpdateNotificationHub returns UnauthorizedAccessException when connection string is incorrect
            Assert.Throws<UnauthorizedAccessException>(() => namespaceManager.UpdateNotificationHub(notificationHubDescription));

            // Check that DeleteNotificationHub returns UnauthorizedAccessException when connection string is incorrect
            Assert.Throws<UnauthorizedAccessException>(() => namespaceManager.DeleteNotificationHub(_notificationHubName));

        }

        private void CleanUp()
        {
            try
            {
                _namespaceManager.DeleteNotificationHub(_notificationHubName);
            }
            catch
            {
                // no-op
            }
        }

        private static Uri GetInputFileUrl(CloudBlobContainer container, string filePath)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4),
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Create
            };

            var sasToken = container.GetBlockBlobReference(filePath).GetSharedAccessSignature(sasConstraints);
            return new Uri(container.Uri + "/" + filePath + sasToken);
        }

        private static Uri GetOutputDirectoryUrl(CloudBlobContainer container)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4),
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read
            };

            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);
            return new Uri(container.Uri + sasContainerToken);
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

        private void LoadMockData([CallerMemberName] string methodName = "")
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

        private void RecordTestResults([CallerMemberName] string methodName = "")
        {
            if (_testServer.RecordingMode)
            {
                File.WriteAllText($"{methodName}.http", JsonConvert.SerializeObject(_testServer.Session));
            }
        }
    }
}
