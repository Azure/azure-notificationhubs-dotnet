using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.NotificationHubs.Auth;
using Microsoft.Azure.NotificationHubs.Messaging;
using Microsoft.Extensions.Configuration;
using Xunit;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;

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
        
        private string _namespaceUriString;
        private string _notificationHubConnectionString;

        public ManagementApiE2ETests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _namespaceUriString = Environment.GetEnvironmentVariable(NotificationHubNamespaceUriString.ToUpper()) ?? configuration[NotificationHubNamespaceUriString];
            _notificationHubConnectionString = Environment.GetEnvironmentVariable(NotificationHubConnectionString.ToUpper()) ?? configuration[NotificationHubConnectionString];
            _notificationHubName = Environment.GetEnvironmentVariable(NotificationHubName.ToUpper()) ?? configuration[NotificationHubName];

            _storageAccount = Environment.GetEnvironmentVariable(StorageAccount.ToUpper()) ?? configuration[StorageAccount];
            _storagePassword = Environment.GetEnvironmentVariable(StoragePassword.ToUpper()) ?? configuration[StoragePassword];
            _storageEndpointAddress = Environment.GetEnvironmentVariable(StorageEndpointString.ToUpper()) ?? configuration[StorageEndpointString];
            _containerName = Environment.GetEnvironmentVariable(ContainerName.ToUpper()) ?? configuration[ContainerName];

            _namespaceManagerSettings = new NamespaceManagerSettings();
            _namespaceManagerSettings.TokenProvider = SharedAccessSignatureTokenProvider.CreateSharedAccessSignatureTokenProvider(_notificationHubConnectionString);
            _namespaceManager = new NamespaceManager(new Uri(_namespaceUriString), _namespaceManagerSettings);

            _storageEndpoint = new StorageUri(new Uri(_storageEndpointAddress));
        }

        [Fact]
        public void ExecuteCreateGetAndDeleteNotificationHubMethods_ShouldCreateGetOrDeleteHubOrRecieveConflictException()
        {
            CleanUp();

            bool notificationHubExists = true;
            IEnumerable<NotificationHubDescription> notificationHubDescriptions;

            var protocolVersion = _namespaceManager.GetVersionInfo();
            Assert.Equal("2017-11", protocolVersion);

            // Check that GetNotification returns MessagingEntityNotFoundException than hub is not exist
            Assert.Throws<MessagingEntityNotFoundException>(() => _namespaceManager.GetNotificationHub(_notificationHubName));

            // Check that NotificationHubExists return false when notification hub is not exist
            notificationHubExists = _namespaceManager.NotificationHubExists(_notificationHubName);
            Assert.False(notificationHubExists);

            // Check that GetNotificationHubs returns collection without not existed hub
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

            CleanUp();
        }

        [Fact]
        public void SubmitAndGetNotificationHubJob_ShouldReceiveCorrectJobs()
        {
            CleanUp();

            _namespaceManager.CreateNotificationHub(_notificationHubName);

            var blobClient = new CloudBlobClient(
                _storageEndpoint,
                new StorageCredentials(_storageAccount, _storagePassword));

            var container = blobClient.GetContainerReference(_containerName);

            var outputContainerSasUri = GetOutputDirectoryUrl(container);
            var inputFileSasUri = GetInputFileUrl(container, InputFileName);

            var notificationHubJob = new NotificationHubJob()
            {
                JobType = NotificationHubJobType.ImportCreateRegistrations,
                OutputContainerUri = outputContainerSasUri,
                ImportFileUri = inputFileSasUri
            };

            var submitedNotificationHubJob 
                = _namespaceManager.SubmitNotificationHubJobAsync(notificationHubJob, _notificationHubName)
                .GetAwaiter().GetResult();
            Assert.NotNull(submitedNotificationHubJob);
            Assert.NotEmpty(submitedNotificationHubJob.JobId);

            var recievedNotificationHubJob 
                = _namespaceManager.GetNotificationHubJobAsync(submitedNotificationHubJob.JobId, _notificationHubName)
                .GetAwaiter().GetResult();

            Assert.Equal(submitedNotificationHubJob.JobId, recievedNotificationHubJob.JobId);

            CleanUp();
        }

        [Fact]
        public void ExecuteCreateGetAndDeleteNotificationHubMethodsWithIncorrectConnectionString_ShouldRecieveUnauthorizedAccessException()
        {
            CleanUp();

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

            CleanUp();
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

        private Uri GetInputFileUrl(CloudBlobContainer container, string filePath)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4),
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Create
            };

            var sasToken = container.GetBlockBlobReference(filePath).GetSharedAccessSignature(sasConstraints);
            return new Uri(container.Uri + "/" + filePath + sasToken);
        }

        private Uri GetOutputDirectoryUrl(CloudBlobContainer container)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(4),
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read
            };
            
            string sasContainerToken = container.GetSharedAccessSignature(sasConstraints);
            return new Uri(container.Uri + sasContainerToken);
        }
    }
}
