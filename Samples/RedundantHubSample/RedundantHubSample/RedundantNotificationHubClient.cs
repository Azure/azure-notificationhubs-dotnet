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

        public Task<Installation> GetInstallationAsync(string installationId) 
            => DefaultNamespace == DefaultNamespace.Primary ?
                _primaryNotificationHubClient.GetInstallationAsync(installationId) :
                _backupNotificationHubClient.GetInstallationAsync(installationId);

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
