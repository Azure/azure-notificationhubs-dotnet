using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace RedundantHubSample
{
    public enum DefaultNamespace
    {
        Primary = 1,
        Backup = 2
    }

    public class RedundantNotificationHubClient
    {
        private readonly INotificationHubClient _primaryNotificationHubClient;
        private readonly INotificationHubClient _backupNotificationHubClient;

        public RedundantNotificationHubClient(string primaryConnectionString, string backupConnectionString, string hubName)
        {
            _primaryNotificationHubClient = NotificationHubClient.CreateClientFromConnectionString(primaryConnectionString, hubName);
            _backupNotificationHubClient = NotificationHubClient.CreateClientFromConnectionString(backupConnectionString, hubName);
        }

        public DefaultNamespace DefaultNamespace { get; set; }

        public Task CreateOrUpdateInstallationAsync(Installation installation, CancellationToken cancellationToken = default)
            => Task.WhenAll(
                _primaryNotificationHubClient.CreateOrUpdateInstallationAsync(installation, cancellationToken),
                _backupNotificationHubClient.CreateOrUpdateInstallationAsync(installation, cancellationToken));

        public async Task<Installation> GetInstallationAsync(string installationId)
        {
            while (true)
            {
                try
                {
                    return DefaultNamespace == DefaultNamespace.Primary ?
                        await _primaryNotificationHubClient.GetInstallationAsync(installationId) :
                        await _backupNotificationHubClient.GetInstallationAsync(installationId);
                }
                catch (Exception)
                {
                    Console.WriteLine("Waiting for installation to be created");
                    Thread.Sleep(1000);
                }
            }
        }

        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken = default)
            => DefaultNamespace == DefaultNamespace.Primary ?
                _primaryNotificationHubClient.SendFcmNativeNotificationAsync(jsonPayload, tagExpression, cancellationToken) :
                _backupNotificationHubClient.SendFcmNativeNotificationAsync(jsonPayload, tagExpression, cancellationToken);


        public Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId, CancellationToken cancellationToken = default)
            => DefaultNamespace == DefaultNamespace.Primary ?
                _primaryNotificationHubClient.GetNotificationOutcomeDetailsAsync(notificationId, cancellationToken) :
                _backupNotificationHubClient.GetNotificationOutcomeDetailsAsync(notificationId, cancellationToken);
    }
}
