//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a notification hub client.
    /// </summary>
    public interface INotificationHubClient
    {
        /// <summary>
        /// Gets or sets a value indicating whether the client enables a test send.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the client enables a test send; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// When test send is enabled, the following occurs:
        /// <ul><li>All notifications only reach up to 10 devices for each send call.</li><li>The <b>Send*</b> methods return a list of the outcomes for all those notification deliveries. The possible outcomes are
        /// the same as displayed in telemetry. Outcomes includes things like authentication errors, throttling errors, successful deliveries,
        /// and so on.</li></ul><p>This mode is for test purposes only, not for production, and is throttled.</p>
        /// </remarks>
        bool EnableTestSend { get; }

        /// <summary>
        /// Cancels the notification asynchronously.
        /// </summary>
        /// <param name="scheduledNotificationId">The scheduled notification identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CancelNotificationAsync(string scheduledNotificationId);

        /// <summary>
        /// Cancels the notification asynchronously.
        /// </summary>
        /// <param name="scheduledNotificationId">The scheduled notification identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CancelNotificationAsync(string scheduledNotificationId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates a native administrative registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId);

        /// <summary>
        /// Asynchronously creates a native administrative registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates a native administrative registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="tags">The tags for the registration.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates a native administrative registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="tags">The tags for the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates an administrative template registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload);

        /// <summary>
        /// Asynchronously creates an administrative template registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates an administrative template registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates an administrative template registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates an Apple native registration.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken);

        /// <summary>
        /// Asynchronously creates an Apple native registration.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates an Apple native registration.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates an Apple native registration.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates an Apple template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload);

        /// <summary>
        /// Asynchronously creates an Apple template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates an Apple template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates an Apple template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Creates the baidu native registration asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<BaiduRegistrationDescription> CreateBaiduNativeRegistrationAsync(string userId, string channelId);

        /// <summary>
        /// Creates the baidu native registration asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<BaiduRegistrationDescription> CreateBaiduNativeRegistrationAsync(string userId, string channelId, IEnumerable<string> tags);

        /// <summary>
        /// Creates the baidu template registration asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="jsonPayload">The json payload.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<BaiduTemplateRegistrationDescription> CreateBaiduTemplateRegistrationAsync(string userId, string channelId, string jsonPayload);

        /// <summary>
        /// Creates the baidu template registration asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="jsonPayload">The json payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<BaiduTemplateRegistrationDescription> CreateBaiduTemplateRegistrationAsync(string userId, string channelId, string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates a browser native registration.
        /// </summary>
        /// <param name="endpoint">String containing the endpoint associated with the push subscription.</param>
        /// <param name="auth">Value used to retrieve the authentication secret.</param>
        /// <param name="p256dh">Value used to retrieve the public key.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<BrowserRegistrationDescription> CreateBrowserNativeRegistrationAsync(string endpoint, string auth, string p256dh);

        /// <summary>
        /// Asynchronously creates a browser native registration.
        /// </summary>
        /// <param name="endpoint">String containing the endpoint associated with the push subscription.</param>
        /// <param name="auth">Value used to retrieve the authentication secret.</param>
        /// <param name="p256dh">Value used to retrieve the public key.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<BrowserRegistrationDescription> CreateBrowserNativeRegistrationAsync(string endpoint, string auth, string p256dh, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates a browser native registration.
        /// </summary>
        /// <param name="endpoint">String containing the endpoint associated with the push subscription.</param>
        /// <param name="auth">Value used to retrieve the authentication secret.</param>
        /// <param name="p256dh">Value used to retrieve the public key.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<BrowserRegistrationDescription> CreateBrowserNativeRegistrationAsync(string endpoint, string auth, string p256dh, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates a browser native registration.
        /// </summary>
        /// <param name="endpoint">String containing the endpoint associated with the push subscription.</param>
        /// <param name="auth">Value used to retrieve the authentication secret.</param>
        /// <param name="p256dh">Value used to retrieve the public key.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<BrowserRegistrationDescription> CreateBrowserNativeRegistrationAsync(string endpoint, string auth, string p256dh, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates FCM native registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId);

        /// <summary>
        /// Asynchronously creates FCM native registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates FCM native registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates FCM native registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates FCM template registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload);

        /// <summary>
        /// Asynchronously creates FCM template registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates FCM template registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates FCM template registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates FCM V1 native registration.
        /// </summary>
        /// <param name="fcmV1RegistrationId">The FCM V1 registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmV1RegistrationDescription> CreateFcmV1NativeRegistrationAsync(string fcmV1RegistrationId);

        /// <summary>
        /// Asynchronously creates FCM V1 native registration.
        /// </summary>
        /// <param name="fcmV1RegistrationId">The FCM V1 registration ID.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmV1RegistrationDescription> CreateFcmV1NativeRegistrationAsync(string fcmV1RegistrationId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates FCM V1 native registration.
        /// </summary>
        /// <param name="fcmV1RegistrationId">The FCM V1 registration ID.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmV1RegistrationDescription> CreateFcmV1NativeRegistrationAsync(string fcmV1RegistrationId, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates FCM V1 native registration.
        /// </summary>
        /// <param name="fcmV1RegistrationId">The FCM V1 registration ID.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmV1RegistrationDescription> CreateFcmV1NativeRegistrationAsync(string fcmV1RegistrationId, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates FCM V1 template registration.
        /// </summary>
        /// <param name="fcmV1RegistrationId">The FCM V1 registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmV1TemplateRegistrationDescription> CreateFcmV1TemplateRegistrationAsync(string fcmV1RegistrationId, string jsonPayload);

        /// <summary>
        /// Asynchronously creates FCM V1 template registration.
        /// </summary>
        /// <param name="fcmV1RegistrationId">The FCM V1 registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmV1TemplateRegistrationDescription> CreateFcmV1TemplateRegistrationAsync(string fcmV1RegistrationId, string jsonPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates FCM V1 template registration.
        /// </summary>
        /// <param name="fcmV1RegistrationId">The FCM V1 registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmV1TemplateRegistrationDescription> CreateFcmV1TemplateRegistrationAsync(string fcmV1RegistrationId, string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates FCM V1 template registration.
        /// </summary>
        /// <param name="fcmV1RegistrationId">The FCM V1 registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<FcmV1TemplateRegistrationDescription> CreateFcmV1TemplateRegistrationAsync(string fcmV1RegistrationId, string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates MPNS native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<MpnsRegistrationDescription> CreateMpnsNativeRegistrationAsync(string channelUri);

        /// <summary>
        /// Asynchronously creates MPNS native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<MpnsRegistrationDescription> CreateMpnsNativeRegistrationAsync(string channelUri, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates MPNS template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<MpnsTemplateRegistrationDescription> CreateMpnsTemplateRegistrationAsync(string channelUri, string xmlTemplate);

        /// <summary>
        /// Asynchronously creates MPNS template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<MpnsTemplateRegistrationDescription> CreateMpnsTemplateRegistrationAsync(string channelUri, string xmlTemplate, IEnumerable<string> tags);

        /// <summary>
        /// Creates or updates a device installation.
        /// </summary>
        /// <param name="installation">The device installation object.</param>
        void CreateOrUpdateInstallation(Installation installation);

        /// <summary>
        /// Creates or updates a device installation asynchronously.
        /// </summary>
        /// <param name="installation">The device installation object.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installation object is null</exception>
        /// <exception cref="System.InvalidOperationException">InstallationId must be specified</exception>
        Task CreateOrUpdateInstallationAsync(Installation installation);

        /// <summary>
        /// Creates or updates a device installation asynchronously.
        /// </summary>
        /// <param name="installation">The device installation object.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installation object is null</exception>
        /// <exception cref="System.InvalidOperationException">InstallationId must be specified</exception>
        Task CreateOrUpdateInstallationAsync(Installation installation, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates or updates the client registration.
        /// </summary>
        /// <typeparam name="T">The type of registration.</typeparam>
        /// <param name="registration">The registration to be created or updated.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when RegistrationId object is null</exception>
        Task<T> CreateOrUpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription;

        /// <summary>
        /// Asynchronously creates or updates the client registration.
        /// </summary>
        /// <typeparam name="T">The type of registration.</typeparam>
        /// <param name="registration">The registration to be created or updated.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when RegistrationId object is null</exception>
        Task<T> CreateOrUpdateRegistrationAsync<T>(T registration, CancellationToken cancellationToken) where T : RegistrationDescription;

        /// <summary>
        /// Asynchronously creates a registration.
        /// </summary>
        /// <typeparam name="T">The type of registration.</typeparam>
        /// <param name="registration">The registration to create.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// NotificationHubPath in RegistrationDescription is not valid.
        /// or
        /// RegistrationId should be null or empty
        /// </exception>
        Task<T> CreateRegistrationAsync<T>(T registration) where T : RegistrationDescription;

        /// <summary>
        /// Asynchronously creates a registration.
        /// </summary>
        /// <typeparam name="T">The type of registration.</typeparam>
        /// <param name="registration">The registration to create.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// NotificationHubPath in RegistrationDescription is not valid.
        /// or
        /// RegistrationId should be null or empty
        /// </exception>
        Task<T> CreateRegistrationAsync<T>(T registration, CancellationToken cancellationToken) where T : RegistrationDescription;

        /// <summary>
        /// Asynchronously creates a registration identifier.
        /// </summary>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<string> CreateRegistrationIdAsync();

        /// <summary>
        /// Asynchronously creates a registration identifier.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        Task<string> CreateRegistrationIdAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates Windows native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri);

        /// <summary>
        /// Asynchronously creates Windows native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates Windows native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates Windows native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates Windows template registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri, string xmlTemplate);

        /// <summary>
        /// Asynchronously creates Windows template registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri, string xmlTemplate, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates Windows template registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri, string xmlTemplate, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously creates Windows template registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri, string xmlTemplate, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the installation.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        void DeleteInstallation(string installationId);

        /// <summary>
        /// Deletes the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installationId object is null</exception>
        Task DeleteInstallationAsync(string installationId);

        /// <summary>
        /// Deletes the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installationId object is null</exception>
        Task DeleteInstallationAsync(string installationId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registration">The registration to delete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when registration object is null.</exception>
        Task DeleteRegistrationAsync(RegistrationDescription registration);

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registration">The registration to delete.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when registration object is null.</exception>
        Task DeleteRegistrationAsync(RegistrationDescription registration, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task DeleteRegistrationAsync(string registrationId);

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task DeleteRegistrationAsync(string registrationId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <param name="etag">The entity tag.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">registrationId</exception>
        Task DeleteRegistrationAsync(string registrationId, string etag);

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <param name="etag">The entity tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">registrationId</exception>
        Task DeleteRegistrationAsync(string registrationId, string etag, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously deletes the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pnsHandle</exception>
        Task DeleteRegistrationsByChannelAsync(string pnsHandle);

        /// <summary>
        /// Asynchronously deletes the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pnsHandle</exception>
        Task DeleteRegistrationsByChannelAsync(string pnsHandle, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(int top);

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="top">The location of the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(int top, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(string continuationToken, int top);

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location of the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(string continuationToken, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the base URI for the notification hub client.
        /// </summary>
        /// <returns>
        /// The base URI of the notification hub.
        /// </returns>
        Uri GetBaseUri();

        /// <summary>
        /// Gets the feedback container URI asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<Uri> GetFeedbackContainerUriAsync();

        /// <summary>
        /// Gets the feedback container URI asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<Uri> GetFeedbackContainerUriAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a device installation object.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>The device installation object</returns>
        Installation GetInstallation(string installationId);

        /// <summary>
        /// Gets the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installationId object is null</exception>
        Task<Installation> GetInstallationAsync(string installationId);

        /// <summary>
        /// Gets the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installationId object is null</exception>
        Task<Installation> GetInstallationAsync(string installationId, CancellationToken cancellationToken);

        /// <summary>
        /// Given a jobId, returns the associated <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />. This method
        /// is used to get the status of the job to see if that job completed, failed, or is still in progress.
        /// This API is only available for Standard namespaces.
        /// </summary>
        /// <param name="jobId">The jobId is returned after creating a new job using <see cref="Microsoft.Azure.NotificationHubs.NotificationHubClient.SubmitNotificationHubJobAsync(NotificationHubJob)" />.</param>
        /// <returns>
        /// The current state of the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubClient.SubmitNotificationHubJobAsync(NotificationHubJob)" />.
        /// </returns>
        Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId);

        /// <summary>
        /// Given a jobId, returns the associated <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />. This method
        /// is used to get the status of the job to see if that job completed, failed, or is still in progress.
        /// This API is only available for Standard namespaces.
        /// </summary>
        /// <param name="jobId">The jobId is returned after creating a new job using <see cref="Microsoft.Azure.NotificationHubs.NotificationHubClient.SubmitNotificationHubJobAsync(NotificationHubJob)" />.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The current state of the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubClient.SubmitNotificationHubJobAsync(NotificationHubJob)" />.
        /// </returns>
        Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, CancellationToken cancellationToken);

        /// <summary>
        /// Returns all known <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s. This method
        /// is used to get the status of all job to see if those jobs completed, failed, or are still in progress.
        /// This API is only available for Standard namespaces.
        /// </summary>
        /// <returns>
        /// The current state of the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s.
        /// </returns>
        Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync();

        /// <summary>
        /// Returns all known <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s. This method
        /// is used to get the status of all job to see if those jobs completed, failed, or are still in progress.
        /// This API is only available for Standard namespaces.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The current state of the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s.
        /// </returns>
        Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the results of a Send* operation. This can retrieve intermediate results if the send is being processed
        /// or final results if the Send* has completed. This API can only be called for Standard namespaces.
        /// </summary>
        /// <param name="notificationId"><see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome.NotificationId" /> which was returned
        /// when calling Send*.</param>
        /// <returns>
        /// The result of the Send operation, as expressed by a <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notificationId</exception>
        Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId);

        /// <summary>
        /// Retrieves the results of a Send* operation. This can retrieve intermediate results if the send is being processed
        /// or final results if the Send* has completed. This API can only be called for Standard namespaces.
        /// </summary>
        /// <param name="notificationId"><see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome.NotificationId" /> which was returned
        /// when calling Send*.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The result of the Send operation, as expressed by a <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notificationId</exception>
        Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves a registration with a given ID. The type of the registration depends upon the specified TRegistrationDescription parameter.
        /// </summary>
        /// <typeparam name="TRegistrationDescription">The type of registration description.</typeparam>
        /// <param name="registrationId">The registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when registrationId is null</exception>
        Task<TRegistrationDescription> GetRegistrationAsync<TRegistrationDescription>(string registrationId) where TRegistrationDescription : RegistrationDescription;

        /// <summary>
        /// Asynchronously retrieves a registration with a given ID. The type of the registration depends upon the specified TRegistrationDescription parameter.
        /// </summary>
        /// <typeparam name="TRegistrationDescription">The type of registration description.</typeparam>
        /// <param name="registrationId">The registration ID.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when registrationId is null</exception>
        Task<TRegistrationDescription> GetRegistrationAsync<TRegistrationDescription>(string registrationId, CancellationToken cancellationToken) where TRegistrationDescription : RegistrationDescription;

        /// <summary>
        /// Asynchronously gets the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, int top);

        /// <summary>
        /// Asynchronously gets the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="top">The location of the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously gets the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pnsHandle</exception>
        Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, string continuationToken, int top);

        /// <summary>
        /// Asynchronously gets the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location of the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pnsHandle</exception>
        Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, string continuationToken, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously gets the registrations by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="top">The location where to get the registrations.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, int top);

        /// <summary>
        /// Asynchronously gets the registrations by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="top">The location where to get the registrations.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously gets the registrations by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location where to get the registrations.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when tag object is null</exception>
        Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, string continuationToken, int top);

        /// <summary>
        /// Asynchronously gets the registrations by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location where to get the registrations.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when tag object is null</exception>
        Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, string continuationToken, int top, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether the given installation exists based upon the installation identifier.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>true if the installation exists, else false.</returns>
        bool InstallationExists(string installationId);

        /// <summary>
        /// Determines whether the given installation exists based upon the installation identifier.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>Returns a task which is true if the installation exists, else false.</returns>
        Task<bool> InstallationExistsAsync(string installationId);

        /// <summary>
        /// Determines whether the given installation exists based upon the installation identifier.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>Returns a task which is true if the installation exists, else false.</returns>
        Task<bool> InstallationExistsAsync(string installationId, CancellationToken cancellationToken);

        /// <summary>
        /// Patches the installation.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="operations">The collection of update operations.</param>
        void PatchInstallation(string installationId, IList<PartialUpdateOperation> operations);

        /// <summary>
        /// Patches the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="operations">The collection of update operations.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the installationId or operations object is null
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the operations list is empty</exception>
        Task PatchInstallationAsync(string installationId, IList<PartialUpdateOperation> operations);

        /// <summary>
        /// Patches the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="operations">The collection of update operations.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the installationId or operations object is null
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the operations list is empty</exception>
        Task PatchInstallationAsync(string installationId, IList<PartialUpdateOperation> operations, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously indicates that the registration already exists.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<bool> RegistrationExistsAsync(string registrationId);

        /// <summary>
        /// Asynchronously indicates that the registration already exists.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        Task<bool> RegistrationExistsAsync(string registrationId, CancellationToken cancellationToken);

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime);

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, CancellationToken cancellationToken);

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when tags object is null</exception>
        /// <exception cref="System.ArgumentException">tags argument should contain at least one tag</exception>
        Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, IEnumerable<string> tags);

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when tags object is null</exception>
        /// <exception cref="System.ArgumentException">tags argument should contain at least one tag</exception>
        Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="tagExpression">The tag expression.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, string tagExpression);

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="tagExpression">The tag expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload);

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, string tagExpression);

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Sends an Apple native notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/Chapters/ApplePushService.html">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload);

        /// <summary>
        /// Sends an Apple native notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/Chapters/ApplePushService.html">here</a>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously sends an Apple native notification to a non-empty set of tags (maximum 20). This is equivalent to a tagged expression with boolean ORs ("||"). To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/Chapters/ApplePushService.html">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously sends an Apple native notification to a non-empty set of tags (maximum 20). This is equivalent to a tagged expression with boolean ORs ("||"). To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/Chapters/ApplePushService.html">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously sends an Apple native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/Chapters/ApplePushService.html">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, string tagExpression);

        /// <summary>
        /// Asynchronously sends an Apple native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/Chapters/ApplePushService.html">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a Baidu native notification.
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message);

        /// <summary>
        /// Sends a Baidu native notification.
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, CancellationToken cancellationToken);

        /// <summary>
        /// Sends Baidu native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, IEnumerable<string> tags);

        /// <summary>
        /// Sends Baidu native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Sends Baidu native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, string tagExpression);

        /// <summary>
        /// Sends Baidu native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a browser notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid browser push notification payload.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBrowserNotificationAsync(string jsonPayload);

        /// <summary>
        /// Sends a browser notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid browser push notification payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBrowserNotificationAsync(string jsonPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a browser notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid browser push notification payload.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBrowserNotificationAsync(string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Sends a browser notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid browser push notification payload.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBrowserNotificationAsync(string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a browser notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid browser push notification payload.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBrowserNotificationAsync(string jsonPayload, string tagExpression);

        /// <summary>
        /// Sends a browser notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid browser push notification payload.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendBrowserNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a notification directly to all devices listed in deviceHandles (a valid tokens as expressed by the Notification type).
        /// Users of this API do not use Registrations or Installations. Instead, users of this API manage all devices
        /// on their own and use Azure Notification Hub solely as a pass through service to communicate with
        /// the various Push Notification Services.  This feature is not available on the free SKU.
        /// </summary>
        /// <param name="notification">A instance of a Notification, identifying which Push Notification Service to send to.</param>
        /// <param name="deviceHandles">A list of valid device identifiers.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when notification or deviceHandles object is null
        /// </exception>
        Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, IList<string> deviceHandles);

        /// <summary>
        /// Sends a notification directly to all devices listed in deviceHandles (a valid tokens as expressed by the Notification type).
        /// Users of this API do not use Registrations or Installations. Instead, users of this API manage all devices
        /// on their own and use Azure Notification Hub solely as a pass through service to communicate with
        /// the various Push Notification Services.  This feature is not available on the free SKU.
        /// </summary>
        /// <param name="notification">A instance of a Notification, identifying which Push Notification Service to send to.</param>
        /// <param name="deviceHandles">A list of valid device identifiers.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when notification or deviceHandles object is null
        /// </exception>
        Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, IList<string> deviceHandles, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a notification directly to a deviceHandle (a valid token as expressed by the Notification type).
        /// Users of this API do not use Registrations or Installations. Instead, users of this API manage all devices
        /// on their own and use Azure Notification Hub solely as a pass through service to communicate with
        /// the various Push Notification Services.
        /// </summary>
        /// <param name="notification">A instance of a Notification, identifying which Push Notification Service to send to.</param>
        /// <param name="deviceHandle">A valid device identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when notification or deviceHandle object is null
        /// </exception>
        Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, string deviceHandle);

        /// <summary>
        /// Sends a notification directly to a deviceHandle (a valid token as expressed by the Notification type).
        /// Users of this API do not use Registrations or Installations. Instead, users of this API manage all devices
        /// on their own and use Azure Notification Hub solely as a pass through service to communicate with
        /// the various Push Notification Services.
        /// </summary>
        /// <param name="notification">A instance of a Notification, identifying which Push Notification Service to send to.</param>
        /// <param name="deviceHandle">A valid device identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when notification or deviceHandle object is null
        /// </exception>
        Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, string deviceHandle, CancellationToken cancellationToken);

        /// <summary>
        /// Sends Firebase Cloud Messaging (FCM) native notification.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload);

        /// <summary>
        /// Sends Firebase Cloud Messaging (FCM) native notification.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a FCM native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags);

        /// <summary>
        /// Sends a FCM native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Sends FCM native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, string tagExpression);

        /// <summary>
        /// Sends FCM native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification. To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload);

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification. To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification to a non-empty set of tags (maximum 20). This is equivalent to a tag expression with boolean ORs ("||"). To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The notification payload.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, IEnumerable<string> tags);

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification to a non-empty set of tags (maximum 20). This is equivalent to a tag expression with boolean ORs ("||"). To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The notification payload.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, string tagExpression);

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        Task<NotificationOutcome> SendNotificationAsync(Notification notification);

        /// <summary>
        /// Sends a notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        Task<NotificationOutcome> SendNotificationAsync(Notification notification, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously sends a notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <param name="tags">A non-empty set of tags (max 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when notification or tag object is null
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// notification.Tag property should not be null
        /// or
        /// tags argument should contain at least one tag
        /// </exception>
        Task<NotificationOutcome> SendNotificationAsync(Notification notification, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously sends a notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <param name="tags">A non-empty set of tags (max 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when notification or tag object is null
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// notification.Tag property should not be null
        /// or
        /// tags argument should contain at least one tag
        /// </exception>
        Task<NotificationOutcome> SendNotificationAsync(Notification notification, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        /// <exception cref="System.ArgumentException">notification.Tag property should be null</exception>
        Task<NotificationOutcome> SendNotificationAsync(Notification notification, string tagExpression);

        /// <summary>
        /// Sends a notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        /// <exception cref="System.ArgumentException">notification.Tag property should be null</exception>
        Task<NotificationOutcome> SendNotificationAsync(Notification notification, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a template notification.
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties);

        /// <summary>
        /// Sends a template notification.
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a template notification to a non-empty set of tags (maximum 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, IEnumerable<string> tags);

        /// <summary>
        /// Sends a template notification to a non-empty set of tags (maximum 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a template notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, string tagExpression);

        /// <summary>
        /// Sends a template notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously sends a Windows native notification. To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, 
        /// including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload);

        /// <summary>
        /// Asynchronously sends a Windows native notification. To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, 
        /// including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously sends a Windows native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||"). To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="tags">A non-empty set of tags (max 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, IEnumerable<string> tags);

        /// <summary>
        /// Asynchronously sends a Windows native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||"). To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="tags">A non-empty set of tags (max 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, IEnumerable<string> tags, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously sends a Windows native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, string tagExpression);

        /// <summary>
        /// Asynchronously sends a Windows native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, string tagExpression, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />. This API is only
        /// available for Standard namespaces.
        /// </summary>
        /// <param name="job">The <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" /> to
        /// export registrations, import registrations, or create registrations.</param>
        /// <returns>
        /// The submitted <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s.
        /// </returns>
        Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job);

        /// <summary>
        /// Creates a <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />. This API is only
        /// available for Standard namespaces.
        /// </summary>
        /// <param name="job">The <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" /> to
        /// export registrations, import registrations, or create registrations.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The submitted <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s.
        /// </returns>
        Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously updates the registration.
        /// </summary>
        /// <typeparam name="T">The type of registration.</typeparam>
        /// <param name="registration">The registration to update.</param>
        /// <returns>
        /// A task that will complete when the update finishes.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when RegistrationId or ETag object is null
        /// </exception>
        Task<T> UpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription;

        /// <summary>
        /// Asynchronously updates the registration.
        /// </summary>
        /// <typeparam name="T">The type of registration.</typeparam>
        /// <param name="registration">The registration to update.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// A task that will complete when the update finishes.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when RegistrationId or ETag object is null
        /// </exception>
        Task<T> UpdateRegistrationAsync<T>(T registration, CancellationToken cancellationToken) where T : RegistrationDescription;
    }
}
