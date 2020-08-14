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
    /// <summary>
    /// Represents a namespace manager
    /// </summary>
    public interface INamespaceManager
    {
        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        NotificationHubDescription CreateNotificationHub(NotificationHubDescription description);

        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="hubName">The notification hub description name.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        NotificationHubDescription CreateNotificationHub(string hubName);

        /// <summary>
        /// Creates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>A task that represents the asynchronous create hub operation</returns>
        Task<NotificationHubDescription> CreateNotificationHubAsync(NotificationHubDescription description);

        /// <summary>
        /// Creates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous create hub operation</returns>
        Task<NotificationHubDescription> CreateNotificationHubAsync(NotificationHubDescription description, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="hubName">The notification hub description name.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        Task<NotificationHubDescription> CreateNotificationHubAsync(string hubName);

        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="hubName">The notification hub description name.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        Task<NotificationHubDescription> CreateNotificationHubAsync(string hubName, CancellationToken cancellationToken);
    
        /// <summary>
        /// Delete the notification hub.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        void DeleteNotificationHub(string path);

        /// <summary>
        /// Delete the notification hub.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        Task DeleteNotificationHubAsync(string path);

        /// <summary>
        /// Delete the notification hub.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <param name="path">The notification hub path.</param>
        Task DeleteNotificationHubAsync(string path, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the notification hub.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>A notification hub description object.</returns>
        NotificationHubDescription GetNotificationHub(string path);

        /// <summary>
        /// Gets the notification hub asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get hub operation</returns>
        Task<NotificationHubDescription> GetNotificationHubAsync(string path);

        /// <summary>
        /// Gets the notification hub asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get hub operation</returns>
        Task<NotificationHubDescription> GetNotificationHubAsync(string path, CancellationToken cancellationToken);

        /// <summary>Gets the notification hub job asynchronously.</summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, string notificationHubPath);

        /// <summary>Gets the notification hub job asynchronously.</summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, string notificationHubPath, CancellationToken cancellationToken);

        /// <summary>Gets the notification hub jobs asynchronously.</summary>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get jobs operation</returns>
        Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(string notificationHubPath);

        /// <summary>Gets the notification hub jobs asynchronously.</summary>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get jobs operation</returns>
        Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(string notificationHubPath, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the notification hubs.
        /// </summary>
        /// <returns>A collection of notification hubs</returns>
        IEnumerable<NotificationHubDescription> GetNotificationHubs();

        /// <summary>
        /// Gets the notification hubs asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous get hubs operation</returns>
        Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync();

        /// <summary>
        /// Gets the notification hubs asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get hubs operation</returns>
        Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync(CancellationToken cancellationToken);

        /// <summary>Checks whether a notifications hub exists.</summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>True if the hub exists</returns>
        bool NotificationHubExists(string path);

        /// <summary>
        /// Checks whether a notification hub exists asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous hub check operation</returns>
        Task<bool> NotificationHubExistsAsync(string path);

        /// <summary>
        /// Checks whether a notification hub exists asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous hub check operation</returns>
        Task<bool> NotificationHubExistsAsync(string path, CancellationToken cancellationToken);

        /// <summary>Submits the notification hub job asynchronously.</summary>
        /// <param name="job">The job to submit.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, string notificationHubPath);

        /// <summary>Submits the notification hub job asynchronously.</summary>
        /// <param name="job">The job to submit.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>\
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, string notificationHubPath, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the notification hub.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>The updated hub object</returns>
        NotificationHubDescription UpdateNotificationHub(NotificationHubDescription description);

        /// <summary>
        /// Updates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>A task that represents the asynchronous hub update operation</returns>
        Task<NotificationHubDescription> UpdateNotificationHubAsync(NotificationHubDescription description);

        /// <summary>
        /// Updates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous hub update operation</returns>
        Task<NotificationHubDescription> UpdateNotificationHubAsync(NotificationHubDescription description, CancellationToken cancellationToken);
    }
}