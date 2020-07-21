//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs
{
    public interface INamespaceManager
    {
        NotificationHubDescription CreateNotificationHub(NotificationHubDescription description);
        NotificationHubDescription CreateNotificationHub(string hubName);
        Task<NotificationHubDescription> CreateNotificationHubAsync(NotificationHubDescription description);
        Task<NotificationHubDescription> CreateNotificationHubAsync(NotificationHubDescription description, CancellationToken cancellationToken);
        Task<NotificationHubDescription> CreateNotificationHubAsync(string hubName);
        Task<NotificationHubDescription> CreateNotificationHubAsync(string hubName, CancellationToken cancellationToken);
        void DeleteNotificationHub(string path);
        Task DeleteNotificationHubAsync(string path);
        Task DeleteNotificationHubAsync(string path, CancellationToken cancellationToken);
        NotificationHubDescription GetNotificationHub(string path);
        Task<NotificationHubDescription> GetNotificationHubAsync(string path);
        Task<NotificationHubDescription> GetNotificationHubAsync(string path, CancellationToken cancellationToken);
        Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, string notificationHubPath);
        Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, string notificationHubPath, CancellationToken cancellationToken);
        Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(string notificationHubPath);
        Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(string notificationHubPath, CancellationToken cancellationToken);
        IEnumerable<NotificationHubDescription> GetNotificationHubs();
        Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync();
        Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync(CancellationToken cancellationToken);
        bool NotificationHubExists(string path);
        Task<bool> NotificationHubExistsAsync(string path);
        Task<bool> NotificationHubExistsAsync(string path, CancellationToken cancellationToken);
        Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, string notificationHubPath);
        Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, string notificationHubPath, CancellationToken cancellationToken);
        NotificationHubDescription UpdateNotificationHub(NotificationHubDescription description);
        Task<NotificationHubDescription> UpdateNotificationHubAsync(NotificationHubDescription description);
        Task<NotificationHubDescription> UpdateNotificationHubAsync(NotificationHubDescription description, CancellationToken cancellationToken);
    }
}