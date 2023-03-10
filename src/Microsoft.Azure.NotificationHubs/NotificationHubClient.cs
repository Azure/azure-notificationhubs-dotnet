//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.NotificationHubs.Auth;
using Microsoft.Azure.NotificationHubs.Messaging;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a notification hub client.
    /// </summary>
    public class NotificationHubClient : INotificationHubClient
    {
        private const int EntitiesPerRequest = 100;
        private readonly HttpClient _httpClient;

        private readonly Uri _baseUri;
        private readonly DataContractSerializer _debugResponseSerializer = new DataContractSerializer(typeof(NotificationOutcome));
        private readonly DataContractSerializer _notificationDetailsSerializer = new DataContractSerializer(typeof(NotificationDetails));
        private readonly EntityDescriptionSerializer _entitySerializer = new EntityDescriptionSerializer();
        private readonly string _notificationHubPath;
        private readonly TokenProvider _tokenProvider;
        private readonly NotificationHubRetryPolicy _retryPolicy;
        private NamespaceManager _namespaceManager;

        /// <summary>
        /// Initializes a new instance of <see cref="NotificationHubClient"/>
        /// </summary>
        /// <param name="connectionString">Namespace connection string</param>
        /// <param name="notificationHubPath">Hub name</param>
        public NotificationHubClient(string connectionString, string notificationHubPath) : this(connectionString, notificationHubPath, null)
        {                       
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NotificationHubClient"/> with settings
        /// </summary>
        /// <param name="connectionString">Namespace connection string</param>
        /// <param name="notificationHubPath">Hub name</param>
        /// <param name="settings">Settings</param>
        public NotificationHubClient(string connectionString, string notificationHubPath, NotificationHubSettings settings)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(notificationHubPath))
            {
                throw new ArgumentNullException(nameof(notificationHubPath));
            }

            _notificationHubPath = notificationHubPath;
            _tokenProvider = SharedAccessSignatureTokenProvider.CreateSharedAccessSignatureTokenProvider(connectionString);
            var configurationManager = new KeyValueConfigurationManager(connectionString);
            _namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            _baseUri = GetBaseUri(configurationManager);
            settings = settings ?? new NotificationHubSettings();
            
            if (settings.HttpClient != null)
            {
                _httpClient = settings.HttpClient;
            }
            else if (settings.MessageHandler != null)
            {
                var httpClientHandler = settings.MessageHandler;
                _httpClient = new HttpClient(httpClientHandler);
            }
            else if (settings.Proxy != null)
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.UseProxy = true;
                httpClientHandler.Proxy = settings.Proxy;
                _httpClient = new HttpClient(httpClientHandler);
            }
            else
            {
                _httpClient = new HttpClient();
            }

            if (settings.OperationTimeout == null)
            {
                OperationTimeout = TimeSpan.FromSeconds(60);
            }
            else
            {
                OperationTimeout = settings.OperationTimeout.Value;
            }

            _retryPolicy = settings.RetryOptions.ToRetryPolicy();

            _httpClient.Timeout = OperationTimeout;

            SetUserAgent();
        }

        /// <summary>
        /// Creates a client from connection string.
        /// </summary>
        /// <param name="connectionString">The connection string should have the Listen permission. <see cref="Microsoft.Azure.NotificationHubs.Messaging.AccessRights" /> for
        /// information about the Listen permission.</param>
        /// <param name="notificationHubPath">The notification hub path. If the full path to the notification hub is
        /// https://yourNamespace.notificationhubs.windows.net/yourHub, then you would pass in "yourHub" for notificationPath.</param>
        /// <returns>
        /// The created <see cref="T:Microsoft.Azure.NotificationHubs.NotificationHubClient" />.
        /// </returns>
        public static NotificationHubClient CreateClientFromConnectionString(string connectionString, string notificationHubPath)
        {
            return new NotificationHubClient(connectionString, notificationHubPath);
        }

        /// <summary>
        /// Creates a client from connection string.
        /// </summary>
        /// <param name="connectionString">The connection string should have the Listen permission. <see cref="Microsoft.Azure.NotificationHubs.Messaging.AccessRights" /> for
        /// information about the Listen permission.</param>
        /// <param name="notificationHubPath">The notification hub path. If the full path to the notification hub is
        /// https://yourNamespace.notificationhubs.windows.net/yourHub, then you would pass in "yourHub" for notificationPath.</param>
        /// <param name="enableTestSend">Indicates if the NotificationHubClient should be used to send debug messages by setting <see cref="Microsoft.Azure.NotificationHubs.NotificationHubClient.EnableTestSend" />.</param>
        /// <returns>
        /// The created <see cref="T:Microsoft.Azure.NotificationHubs.NotificationHubClient" />.
        /// </returns>
        public static NotificationHubClient CreateClientFromConnectionString(string connectionString, string notificationHubPath, bool enableTestSend)
        {
            return new NotificationHubClient(connectionString, notificationHubPath) { EnableTestSend = enableTestSend };
        }

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
        public bool EnableTestSend { get; private set; }

        /// <summary>
        /// Gets operation timeout of the HTTP operations.
        /// </summary>
        /// <value>
        ///   <c>Http operation timeout. Defaults to 60 seconds</c>.
        /// </value>
        /// <remarks>
        ///  </remarks>
        public TimeSpan OperationTimeout { get; private set; }

        /// <summary>
        /// Asynchronously sends a Windows native notification. To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, 
        /// including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload)
        {
            return SendWindowsNativeNotificationAsync(windowsNativePayload, string.Empty, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously sends a Windows native notification. To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, 
        /// including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, CancellationToken cancellationToken)
        {
            return SendWindowsNativeNotificationAsync(windowsNativePayload, string.Empty, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends a Windows native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, string tagExpression)
        {
            return SendNotificationAsync(new WindowsNotification(windowsNativePayload), tagExpression, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously sends a Windows native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, string tagExpression, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new WindowsNotification(windowsNativePayload), tagExpression, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends a Windows native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||"). To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="tags">A non-empty set of tags (max 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, IEnumerable<string> tags)
        {
            return SendWindowsNativeNotificationAsync(windowsNativePayload, tags, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously sends a Windows native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||"). To specify headers for WNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="windowsNativePayload">The Windows native payload. This can be used to send any valid WNS notification, including Tile, Toast, and Badge values, as described in the WNS documentation.</param>
        /// <param name="tags">A non-empty set of tags (max 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendWindowsNativeNotificationAsync(string windowsNativePayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new WindowsNotification(windowsNativePayload), tags, cancellationToken);
        }

        /// <summary>
        /// Given a jobId, returns the associated <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />. This method
        /// is used to get the status of the job to see if that job completed, failed, or is still in progress.
        /// This API is only available for Standard namespaces.
        /// </summary>
        /// <param name="jobId">The jobId is returned after creating a new job using <see cref="Microsoft.Azure.NotificationHubs.NotificationHubClient.SubmitNotificationHubJobAsync(NotificationHubJob)" />.</param>
        /// <returns>
        /// The current state of the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubClient.SubmitNotificationHubJobAsync(NotificationHubJob)" />.
        /// </returns>
        public Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId)
        {
            return GetNotificationHubJobAsync(jobId, CancellationToken.None);
        }

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
        public Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, CancellationToken cancellationToken)
        {
            if (jobId == null)
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            return _namespaceManager.GetNotificationHubJobAsync(jobId, _notificationHubPath, cancellationToken);
        }

        /// <summary>
        /// Returns all known <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s. This method
        /// is used to get the status of all job to see if those jobs completed, failed, or are still in progress.
        /// This API is only available for Standard namespaces.
        /// </summary>
        /// <returns>
        /// The current state of the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s.
        /// </returns>
        public Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync()
        {
            return GetNotificationHubJobsAsync(CancellationToken.None);
        }

        /// <summary>
        /// Returns all known <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s. This method
        /// is used to get the status of all job to see if those jobs completed, failed, or are still in progress.
        /// This API is only available for Standard namespaces.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The current state of the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s.
        /// </returns>
        public async Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(CancellationToken cancellationToken)
        {
            return await _namespaceManager.GetNotificationHubJobsAsync(_notificationHubPath, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />. This API is only
        /// available for Standard namespaces.
        /// </summary>
        /// <param name="job">The <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" /> to
        /// export registrations, import registrations, or create registrations.</param>
        /// <returns>
        /// The submitted <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s.
        /// </returns>
        public Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job)
        {
            return SubmitNotificationHubJobAsync(job, CancellationToken.None);
        }

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
        public Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, CancellationToken cancellationToken)
        {
            return _namespaceManager.SubmitNotificationHubJobAsync(job, _notificationHubPath, cancellationToken);
        }

        /// <summary>
        /// Sends an Apple native notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload)
        {
            return SendAppleNativeNotificationAsync(jsonPayload, string.Empty);
        }

        /// <summary>
        /// Sends an Apple native notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification">here</a>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, CancellationToken cancellationToken)
        {
            return SendAppleNativeNotificationAsync(jsonPayload, string.Empty, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends an Apple native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, string tagExpression)
        {
            return SendNotificationAsync(new AppleNotification(jsonPayload), tagExpression);
        }

        /// <summary>
        /// Asynchronously sends an Apple native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new AppleNotification(jsonPayload), tagExpression, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends an Apple native notification to a non-empty set of tags (maximum 20). This is equivalent to a tagged expression with boolean ORs ("||"). To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new AppleNotification(jsonPayload), tags);
        }

        /// <summary>
        /// Asynchronously sends an Apple native notification to a non-empty set of tags (maximum 20). This is equivalent to a tagged expression with boolean ORs ("||"). To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/generating_a_remote_notification">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new AppleNotification(jsonPayload), tags, cancellationToken);
        }

        /// <summary>
        /// Sends a template notification.
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties)
        {
            return SendTemplateNotificationAsync(properties, string.Empty);
        }

        /// <summary>
        /// Sends a template notification.
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, CancellationToken cancellationToken)
        {
            return SendTemplateNotificationAsync(properties, string.Empty, cancellationToken);
        }

        /// <summary>
        /// Sends a template notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, string tagExpression)
        {
            return SendNotificationAsync(new TemplateNotification(properties), tagExpression);
        }

        /// <summary>
        /// Sends a template notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, string tagExpression, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new TemplateNotification(properties), tagExpression, cancellationToken);
        }

        /// <summary>
        /// Sends a template notification to a non-empty set of tags (maximum 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new TemplateNotification(properties), tags);
        }

        /// <summary>
        /// Sends a template notification to a non-empty set of tags (maximum 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="properties">The properties to apply to the template.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendTemplateNotificationAsync(IDictionary<string, string> properties, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new TemplateNotification(properties), tags, cancellationToken);
        }

        /// <summary>
        /// Sends Google Cloud Messaging (GCM) native notification.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a GCM message can be found <a href="https://developers.google.com/cloud-messaging/downstream#notifications_and_data_messages">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        [Obsolete("SendGcmNativeNotificationAsync is deprecated, please use SendFcmNativeNotificationAsync instead.")]
        internal Task<NotificationOutcome> SendGcmNativeNotificationAsync(string jsonPayload)
        {
            return SendGcmNativeNotificationAsync(jsonPayload, string.Empty);
        }

        /// <summary>
        /// Sends GCM native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a GCM message can be found <a href="https://developers.google.com/cloud-messaging/downstream#notifications_and_data_messages">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        [Obsolete("SendGcmNativeNotificationAsync is deprecated, please use SendFcmNativeNotificationAsync instead.")]
        internal Task<NotificationOutcome> SendGcmNativeNotificationAsync(string jsonPayload, string tagExpression)
        {
            return SendNotificationAsync(new GcmNotification(jsonPayload), tagExpression);
        }

        /// <summary>
        /// Sends a GCM native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a GCM message can be found <a href="https://developers.google.com/cloud-messaging/downstream#notifications_and_data_messages">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        [Obsolete("SendGcmNativeNotificationAsync is deprecated, please use SendFcmNativeNotificationAsync instead.")]
        internal Task<NotificationOutcome> SendGcmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new GcmNotification(jsonPayload), tags);
        }

        /// <summary>
        /// Sends Firebase Cloud Messaging (FCM) native notification.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload)
        {
            return SendFcmNativeNotificationAsync(jsonPayload, string.Empty);
        }

        /// <summary>
        /// Sends Firebase Cloud Messaging (FCM) native notification.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, CancellationToken cancellationToken)
        {
            return SendFcmNativeNotificationAsync(jsonPayload, string.Empty, cancellationToken);
        }

        /// <summary>
        /// Sends FCM native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, string tagExpression)
        {
            return SendNotificationAsync(new FcmNotification(jsonPayload), tagExpression);
        }

        /// <summary>
        /// Sends FCM native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new FcmNotification(jsonPayload), tagExpression, cancellationToken);
        }

        /// <summary>
        /// Sends a FCM native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new FcmNotification(jsonPayload), tags);
        }

        /// <summary>
        /// Sends a FCM native notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a FCM message can be found <a href="https://firebase.google.com/docs/cloud-messaging/send-message">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendFcmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new FcmNotification(jsonPayload), tags, cancellationToken);
        }

        /// <summary>
        /// Sends a Baidu native notification.
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message)
        {
            return SendNotificationAsync(new BaiduNotification(message), string.Empty);
        }

        /// <summary>
        /// Sends a Baidu native notification.
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new BaiduNotification(message), string.Empty, cancellationToken);
        }

        /// <summary>
        /// Sends Baidu native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, string tagExpression)
        {
            return SendNotificationAsync(new BaiduNotification(message), tagExpression);
        }

        /// <summary>
        /// Sends Baidu native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, string tagExpression, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new BaiduNotification(message), tagExpression, cancellationToken);
        }

        /// <summary>
        /// Sends Baidu native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new BaiduNotification(message), tags);
        }

        /// <summary>
        /// Sends Baidu native notification to a tag expression (a single tag "tag" is a valid tag expression).
        /// </summary>
        /// <param name="message">This is a json request. Baidu documents the format for the json <a href="http://push.baidu.com/doc/restapi/restapi">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new BaiduNotification(message), tags, cancellationToken);
        }

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload)
        {
            return SendAdmNativeNotificationAsync(jsonPayload, string.Empty);
        }

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, CancellationToken cancellationToken)
        {
            return SendAdmNativeNotificationAsync(jsonPayload, string.Empty, cancellationToken);
        }

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, string tagExpression)
        {
            return SendNotificationAsync(new AdmNotification(jsonPayload), tagExpression);
        }

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, string tagExpression, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new AdmNotification(jsonPayload), tagExpression, cancellationToken);
        }

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new AdmNotification(jsonPayload), tags);
        }

        /// <summary>
        /// Sends the Amazon Device Messaging (ADM) native notification.
        /// </summary>
        /// <param name="jsonPayload">A valid, ADM JSON payload, described in detail <a href="https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message#Message Payloads and Uniqueness">here</a>.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new AdmNotification(jsonPayload), tags, cancellationToken);
        }

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification. To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload)
        {
            return SendMpnsNativeNotificationAsync(nativePayload, string.Empty);
        }

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification. To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, CancellationToken cancellationToken)
        {
            return SendMpnsNativeNotificationAsync(nativePayload, string.Empty, cancellationToken);
        }

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, string tagExpression)
        {
            return SendNotificationAsync(new MpnsNotification(nativePayload), tagExpression);
        }

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification to a tag expression (a single tag "tag" is a valid tag expression). To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <param name="tagExpression">A tag expression is any boolean expression constructed using the logical operators AND (&amp;&amp;), OR (||), NOT (!), and round parentheses. For example: (A || B) &amp;&amp; !C. If an expression uses only ORs, it can contain at most 20 tags. Other expressions are limited to 6 tags. Note that a single tag "A" is a valid expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, string tagExpression, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new MpnsNotification(nativePayload), tagExpression, cancellationToken);
        }

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification to a non-empty set of tags (maximum 20). This is equivalent to a tag expression with boolean ORs ("||"). To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The notification payload.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new MpnsNotification(nativePayload), tags);
        }

        /// <summary>
        /// Sends a Microsoft Push Notification Service (MPNS) native notification to a non-empty set of tags (maximum 20). This is equivalent to a tag expression with boolean ORs ("||"). To specify headers for MPNS, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The notification payload.</param>
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendMpnsNativeNotificationAsync(string nativePayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new MpnsNotification(nativePayload), tags, cancellationToken);
        }

        /// <summary>
        /// Sends a Xiaomi native notification. To specify headers for Xiaomi, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendXiaomiNativeNotificationAsync(string nativePayload)
        {
            return SendNotificationAsync(new XiaomiNotification(nativePayload), string.Empty);
        }

        /// <summary>
        /// Sends a Xiaomi native notification. To specify headers for Xiaomi, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="nativePayload">The native payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendXiaomiNativeNotificationAsync(string nativePayload, CancellationToken cancellationToken)
        {
            return SendNotificationAsync(new XiaomiNotification(nativePayload), string.Empty, cancellationToken);
        }

        /// <summary>
        /// Sends a notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        public Task<NotificationOutcome> SendNotificationAsync(Notification notification)
        {
            return SendNotificationAsync(notification, CancellationToken.None);
        }

        /// <summary>
        /// Sends a notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, CancellationToken cancellationToken)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            return SendNotificationImplAsync(notification, notification.tag, null, cancellationToken);
        }

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
        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, string tagExpression)
        {
            return SendNotificationAsync(notification, tagExpression, CancellationToken.None);
        }

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
        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, string tagExpression, CancellationToken cancellationToken)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            if (notification.tag != null)
            {
                throw new ArgumentException($"{nameof(notification)}.{nameof(notification.tag)} property should be null");
            }

            return SendNotificationImplAsync(notification, tagExpression, null, cancellationToken);
        }

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
        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, IEnumerable<string> tags)
        {
            return SendNotificationAsync(notification, tags, CancellationToken.None);
        }

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
        public Task<NotificationOutcome> SendNotificationAsync(Notification notification, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            if (notification.tag != null)
            {
                throw new ArgumentException($"{nameof(notification)}.{nameof(notification.tag)} property should be null");
            }

            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            if (tags.Count() == 0)
            {
                throw new ArgumentException($"{nameof(tags)} argument should contain at least one value");
            }

            string tagExpression = string.Join("||", tags);
            return SendNotificationImplAsync(notification, tagExpression, null, cancellationToken);
        }

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
        public Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId)
        {
            return GetNotificationOutcomeDetailsAsync(notificationId, CancellationToken.None);
        }

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
        public async Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(notificationId))
            {
                throw new ArgumentNullException(nameof(notificationId), "value may not be null, and must contain more than just whitespace");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"messages/{notificationId}";

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
                {
                    using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, ct).ConfigureAwait(false))
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            return (NotificationDetails)_notificationDetailsSerializer.ReadObject(responseStream);
                        }
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Gets the feedback container URI asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<Uri> GetFeedbackContainerUriAsync()
        {
            return GetFeedbackContainerUriAsync(CancellationToken.None);
        }
        
        /// <summary>
        /// Gets the feedback container URI asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<Uri> GetFeedbackContainerUriAsync(CancellationToken cancellationToken)
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "feedbackcontainer";

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
                {
                    using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, ct).ConfigureAwait(false))
                    {
                        return new Uri(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Creates or updates a device installation.
        /// </summary>
        /// <param name="installation">The device installation object.</param>
        public void CreateOrUpdateInstallation(Installation installation)
        {
            SyncOp(()=>CreateOrUpdateInstallationAsync(installation));
        }

        /// <summary>
        /// Creates or updates a device installation asynchronously.
        /// </summary>
        /// <param name="installation">The device installation object.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installation object is null</exception>
        /// <exception cref="System.InvalidOperationException">InstallationId must be specified</exception>
        public Task CreateOrUpdateInstallationAsync(Installation installation)
        {
            return CreateOrUpdateInstallationAsync(installation, CancellationToken.None);
        }

        /// <summary>
        /// Creates or updates a device installation asynchronously.
        /// </summary>
        /// <param name="installation">The device installation object.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installation object is null</exception>
        /// <exception cref="System.InvalidOperationException">InstallationId must be specified</exception>
        public async Task CreateOrUpdateInstallationAsync(Installation installation, CancellationToken cancellationToken)
        {
            if (installation == null)
            {
                throw new ArgumentNullException(nameof(installation));
            }

            if (string.IsNullOrWhiteSpace(installation.InstallationId))
            {
                throw new InvalidOperationException($"{nameof(installation)}.{nameof(installation.InstallationId)} must be specified");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installation.InstallationId}";

            await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Put, requestUri.Uri, out var trackingId))
                {
                    request.Content = new StringContent(installation.ToJson(), Encoding.UTF8, "application/json");

                    using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, ct).ConfigureAwait(false))
                    {
                        return true;
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Patches the installation.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="operations">The collection of update operations.</param>
        public void PatchInstallation(string installationId, IList<PartialUpdateOperation> operations)
        {
            SyncOp(() => PatchInstallationAsync(installationId, operations));
        }

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
        public Task PatchInstallationAsync(string installationId, IList<PartialUpdateOperation> operations)
        {
            return PatchInstallationAsync(installationId, operations, CancellationToken.None);
        }

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
        public async Task PatchInstallationAsync(string installationId, IList<PartialUpdateOperation> operations, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(installationId))
            {
                throw new ArgumentNullException(nameof(installationId));
            }

            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            if (operations.Count == 0)
            {
                throw new InvalidOperationException($"{nameof(operations)} list is empty");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installationId}";

            await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(new HttpMethod("PATCH"), requestUri.Uri, out var trackingId))
                {
                    request.Content = new StringContent(operations.ToJson(), Encoding.UTF8, "application/json-patch+json");

                    using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, ct).ConfigureAwait(false))
                    {
                        return true;
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Deletes the installation.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        public void DeleteInstallation(string installationId)
        {
            SyncOp(() => DeleteInstallationAsync(installationId));
        }

        /// <summary>
        /// Deletes the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installationId object is null</exception>
        public Task DeleteInstallationAsync(string installationId)
        {
            return DeleteInstallationAsync(installationId, CancellationToken.None);
        }

        /// <summary>
        /// Deletes the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installationId object is null</exception>
        public async Task DeleteInstallationAsync(string installationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(installationId))
            {
                throw new ArgumentNullException(nameof(installationId));
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installationId}";

            await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Delete, requestUri.Uri, out var trackingId))
                {
                    using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.NoContent, ct).ConfigureAwait(false))
                    {
                        return true;
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Determines whether the given installation exists based upon the installation identifier.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>true if the installation exists, else false.</returns>
        public bool InstallationExists(string installationId)
        {
            return SyncOp(() => InstallationExistsAsync(installationId));
        }

        /// <summary>
        /// Determines whether the given installation exists based upon the installation identifier.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>Returns a task which is true if the installation exists, else false.</returns>
        public Task<bool> InstallationExistsAsync(string installationId)
        {
            return InstallationExistsAsync(installationId, CancellationToken.None);
        }

        /// <summary>
        /// Determines whether the given installation exists based upon the installation identifier.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>Returns a task which is true if the installation exists, else false.</returns>
        public async Task<bool> InstallationExistsAsync(string installationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(installationId))
            {
                throw new ArgumentNullException(nameof(installationId));
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installationId}";

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
                {
                    using (var response = await SendRequestAsync(request, trackingId, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound }, ct))
                    {
                        return response.StatusCode == HttpStatusCode.OK;
                    }
                }
            }, cancellationToken);            
        }

        /// <summary>
        /// Gets a device installation object.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>The device installation object</returns>
        public Installation GetInstallation(string installationId)
        {
            return SyncOp(() => GetInstallationAsync(installationId));
        }

        /// <summary>
        /// Gets the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installationId object is null</exception>
        public Task<Installation> GetInstallationAsync(string installationId)
        {
            return GetInstallationAsync(installationId, CancellationToken.None);
        }

        /// <summary>
        /// Gets the installation asynchronously.
        /// </summary>
        /// <param name="installationId">The installation identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the installationId object is null</exception>
        public async Task<Installation> GetInstallationAsync(string installationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(installationId))
            {
                throw new ArgumentNullException(nameof(installationId));
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installationId}";

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
                {
                    using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, ct).ConfigureAwait(false))
                    {
                        var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return JsonConvert.DeserializeObject<Installation>(responseContent);
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates a registration identifier.
        /// </summary>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<string> CreateRegistrationIdAsync()
        {
            return CreateRegistrationIdAsync(CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously creates a registration identifier.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public async Task<string> CreateRegistrationIdAsync(CancellationToken cancellationToken)
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "registrationids";

            string registrationId = null;

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Post, requestUri.Uri, out var trackingId))
                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.Created, ct).ConfigureAwait(false))
                {
                    if (response.Headers.Location != null)
                    {
                        var location = response.Headers.Location;
                        if (location.Segments.Length == 4 && string.Equals(location.Segments[2], "registrationids/", StringComparison.OrdinalIgnoreCase))
                        {
                            registrationId = location.Segments[3];
                        }
                    }
                }

                return registrationId;
            }, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates Windows native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri)
        {
            return CreateWindowsNativeRegistrationAsync(channelUri, null);
        }

        /// <summary>
        /// Asynchronously creates Windows native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri, CancellationToken cancellationToken)
        {
            return CreateWindowsNativeRegistrationAsync(channelUri, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates Windows native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new WindowsRegistrationDescription(new Uri(channelUri), tags));
        }

        /// <summary>
        /// Asynchronously creates Windows native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateRegistrationAsync(new WindowsRegistrationDescription(new Uri(channelUri), tags), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates Windows template registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri, string xmlTemplate)
        {
            return CreateWindowsTemplateRegistrationAsync(channelUri, xmlTemplate, null);
        }

        /// <summary>
        /// Asynchronously creates Windows template registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(string channelUri, string xmlTemplate, CancellationToken cancellationToken)
        {
            return CreateWindowsTemplateRegistrationAsync(channelUri, xmlTemplate, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates Windows template registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(
            string channelUri, string xmlTemplate, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new WindowsTemplateRegistrationDescription(new Uri(channelUri), xmlTemplate, tags));
        }

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
        public Task<WindowsTemplateRegistrationDescription> CreateWindowsTemplateRegistrationAsync(
            string channelUri, string xmlTemplate, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateRegistrationAsync(new WindowsTemplateRegistrationDescription(new Uri(channelUri), xmlTemplate, tags), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates an Apple native registration.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken)
        {
            return CreateAppleNativeRegistrationAsync(deviceToken, null);
        }

        /// <summary>
        /// Asynchronously creates an Apple native registration.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken, CancellationToken cancellationToken)
        {
            return CreateAppleNativeRegistrationAsync(deviceToken, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates an Apple native registration.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new AppleRegistrationDescription(deviceToken, tags));
        }

        /// <summary>
        /// Asynchronously creates an Apple native registration.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateRegistrationAsync(new AppleRegistrationDescription(deviceToken, tags), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates an Apple template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload)
        {
            return CreateAppleTemplateRegistrationAsync(deviceToken, jsonPayload, null);
        }

        /// <summary>
        /// Asynchronously creates an Apple template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload, CancellationToken cancellationToken)
        {
            return CreateAppleTemplateRegistrationAsync(deviceToken, jsonPayload, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates an Apple template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new AppleTemplateRegistrationDescription(deviceToken, jsonPayload, tags));
        }
        
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
        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateRegistrationAsync(new AppleTemplateRegistrationDescription(deviceToken, jsonPayload, tags), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates a native administrative registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId)
        {
            return CreateAdmNativeRegistrationAsync(admRegistrationId, null);
        }

        /// <summary>
        /// Asynchronously creates a native administrative registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId, CancellationToken cancellationToken)
        {
            return CreateAdmNativeRegistrationAsync(admRegistrationId, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates a native administrative registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="tags">The tags for the registration.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new AdmRegistrationDescription(admRegistrationId, tags));
        }

        /// <summary>
        /// Asynchronously creates a native administrative registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="tags">The tags for the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateRegistrationAsync(new AdmRegistrationDescription(admRegistrationId, tags), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates an administrative template registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload)
        {
            return CreateAdmTemplateRegistrationAsync(admRegistrationId, jsonPayload, null);
        }

        /// <summary>
        /// Asynchronously creates an administrative template registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload, CancellationToken cancellationToken)
        {
            return CreateAdmTemplateRegistrationAsync(admRegistrationId, jsonPayload, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates an administrative template registration.
        /// </summary>
        /// <param name="admRegistrationId">The administrative registration identifier.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new AdmTemplateRegistrationDescription(admRegistrationId, jsonPayload, tags));
        }

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
        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateRegistrationAsync(new AdmTemplateRegistrationDescription(admRegistrationId, jsonPayload, tags), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates GCM native registration.
        /// </summary>
        /// <param name="gcmRegistrationId">The GCM registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        [Obsolete("CreateGcmNativeRegistrationAsync is deprecated, please use CreateFcmNativeRegistrationAsync instead.")]
        internal Task<GcmRegistrationDescription> CreateGcmNativeRegistrationAsync(string gcmRegistrationId)
        {
            return CreateGcmNativeRegistrationAsync(gcmRegistrationId, null);
        }

        /// <summary>
        /// Asynchronously creates GCM native registration.
        /// </summary>
        /// <param name="gcmRegistrationId">The GCM registration ID.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        [Obsolete("CreateGcmNativeRegistrationAsync is deprecated, please use CreateFcmNativeRegistrationAsync instead.")]
        internal Task<GcmRegistrationDescription> CreateGcmNativeRegistrationAsync(string gcmRegistrationId, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new GcmRegistrationDescription(gcmRegistrationId, tags));
        }

        /// <summary>
        /// Asynchronously creates GCM template registration.
        /// </summary>
        /// <param name="gcmRegistrationId">The GCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        [Obsolete("CreateGcmTemplateRegistrationAsync is deprecated, please use CreateFcmTemplateRegistrationAsync instead.")]
        internal Task<GcmTemplateRegistrationDescription> CreateGcmTemplateRegistrationAsync(string gcmRegistrationId, string jsonPayload)
        {
            return CreateGcmTemplateRegistrationAsync(gcmRegistrationId, jsonPayload, null);
        }

        /// <summary>
        /// Asynchronously creates GCM template registration.
        /// </summary>
        /// <param name="gcmRegistrationId">The GCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        [Obsolete("CreateGcmTemplateRegistrationAsync is deprecated, please use CreateFcmTemplateRegistrationAsync instead.")]
        internal Task<GcmTemplateRegistrationDescription> CreateGcmTemplateRegistrationAsync(string gcmRegistrationId, string jsonPayload, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new GcmTemplateRegistrationDescription(gcmRegistrationId, jsonPayload, tags));
        }

        /// <summary>
        /// Asynchronously creates FCM native registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId)
        {
            return CreateFcmNativeRegistrationAsync(fcmRegistrationId, null);
        }

        /// <summary>
        /// Asynchronously creates FCM native registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId, CancellationToken cancellationToken)
        {
            return CreateFcmNativeRegistrationAsync(fcmRegistrationId, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates FCM native registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new FcmRegistrationDescription(fcmRegistrationId, tags));
        }

        /// <summary>
        /// Asynchronously creates FCM native registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateRegistrationAsync(new FcmRegistrationDescription(fcmRegistrationId, tags), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates FCM template registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload)
        {
            return CreateFcmTemplateRegistrationAsync(fcmRegistrationId, jsonPayload, null);
        }

        /// <summary>
        /// Asynchronously creates FCM template registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload, CancellationToken cancellationToken)
        {
            return CreateFcmTemplateRegistrationAsync(fcmRegistrationId, jsonPayload, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates FCM template registration.
        /// </summary>
        /// <param name="fcmRegistrationId">The FCM registration ID.</param>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new FcmTemplateRegistrationDescription(fcmRegistrationId, jsonPayload, tags));
        }

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
        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            return CreateRegistrationAsync(new FcmTemplateRegistrationDescription(fcmRegistrationId, jsonPayload, tags), cancellationToken);
        }

        #region Baidu Create Registration

        /// <summary>
        /// Creates the baidu native registration asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<BaiduRegistrationDescription> CreateBaiduNativeRegistrationAsync(string userId, string channelId)
        {
            return CreateBaiduNativeRegistrationAsync(userId, channelId, null);
        }

        /// <summary>
        /// Creates the baidu native registration asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<BaiduRegistrationDescription> CreateBaiduNativeRegistrationAsync(string userId, string channelId,
            IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new BaiduRegistrationDescription(userId, channelId, tags));
        }

        /// <summary>
        /// Creates the baidu template registration asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="jsonPayload">The json payload.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<BaiduTemplateRegistrationDescription> CreateBaiduTemplateRegistrationAsync(string userId,
            string channelId, string jsonPayload, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new BaiduTemplateRegistrationDescription(userId, channelId, jsonPayload, tags));
        }

        /// <summary>
        /// Creates the baidu template registration asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="jsonPayload">The json payload.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<BaiduTemplateRegistrationDescription> CreateBaiduTemplateRegistrationAsync(string userId,
            string channelId, string jsonPayload)
        {
            return CreateBaiduTemplateRegistrationAsync(userId, channelId, jsonPayload, null);
        }

        #endregion

        /// <summary>
        /// Asynchronously creates MPNS native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<MpnsRegistrationDescription> CreateMpnsNativeRegistrationAsync(string channelUri)
        {
            return CreateMpnsNativeRegistrationAsync(channelUri, null);
        }

        /// <summary>
        /// Asynchronously creates MPNS native registration.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<MpnsRegistrationDescription> CreateMpnsNativeRegistrationAsync(string channelUri, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new MpnsRegistrationDescription(new Uri(channelUri), tags));
        }

        /// <summary>
        /// Asynchronously creates MPNS template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<MpnsTemplateRegistrationDescription> CreateMpnsTemplateRegistrationAsync(string channelUri, string xmlTemplate)
        {
            return CreateMpnsTemplateRegistrationAsync(channelUri, xmlTemplate, null);
        }

        /// <summary>
        /// Asynchronously creates MPNS template registration. To specify additional properties at creation, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.CreateRegistrationAsync``1(``0)" /> method.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<MpnsTemplateRegistrationDescription> CreateMpnsTemplateRegistrationAsync(
            string channelUri, string xmlTemplate, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new MpnsTemplateRegistrationDescription(new Uri(channelUri), xmlTemplate, tags));
        }

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
        public Task<T> CreateRegistrationAsync<T>(T registration) where T : RegistrationDescription
        {
            return CreateRegistrationAsync(registration, CancellationToken.None);
        }

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
        public Task<T> CreateRegistrationAsync<T>(T registration, CancellationToken cancellationToken) where T : RegistrationDescription
        {
            if (!string.IsNullOrWhiteSpace(registration.NotificationHubPath) &&
                registration.NotificationHubPath != _notificationHubPath)
            {
                throw new ArgumentException($"{nameof(registration)}.{nameof(registration.NotificationHubPath)} in {nameof(RegistrationDescription)} is not valid.");
            }

            if (!string.IsNullOrWhiteSpace(registration.RegistrationId))
            {
                throw new ArgumentException($"{nameof(registration)}.{nameof(registration.RegistrationId)} should be null or empty");
            }

            return CreateOrUpdateRegistrationImplAsync(registration, EntityOperatonType.Create, cancellationToken);
        }

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
        public Task<T> UpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription
        {
            return UpdateRegistrationAsync(registration, CancellationToken.None);
        }

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
        public Task<T> UpdateRegistrationAsync<T>(T registration, CancellationToken cancellationToken) where T : RegistrationDescription
        {
            if (string.IsNullOrWhiteSpace(registration.RegistrationId))
            {
                throw new ArgumentNullException($"{nameof(registration)}.{nameof(registration.RegistrationId)}");
            }

            if (string.IsNullOrWhiteSpace(registration.ETag))
            {
                throw new ArgumentNullException($"{nameof(registration)}.{nameof(registration.ETag)}");
            }

            return CreateOrUpdateRegistrationImplAsync(registration, EntityOperatonType.Update, cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates or updates the client registration.
        /// </summary>
        /// <typeparam name="T">The type of registration.</typeparam>
        /// <param name="registration">The registration to be created or updated.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when RegistrationId object is null</exception>
        public Task<T> CreateOrUpdateRegistrationAsync<T>(T registration) where T : RegistrationDescription
        {
            return CreateOrUpdateRegistrationAsync(registration, CancellationToken.None);
        }

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
        public Task<T> CreateOrUpdateRegistrationAsync<T>(T registration, CancellationToken cancellationToken) where T : RegistrationDescription
        {
            if (string.IsNullOrWhiteSpace(registration.RegistrationId))
            {
                throw new ArgumentNullException($"{nameof(registration)}.{nameof(registration.RegistrationId)}");
            }

            return CreateOrUpdateRegistrationImplAsync(registration, EntityOperatonType.CreateOrUpdate, cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves a registration with a given ID. The type of the registration depends upon the specified TRegistrationDescription parameter.
        /// </summary>
        /// <typeparam name="TRegistrationDescription">The type of registration description.</typeparam>
        /// <param name="registrationId">The registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when registrationId is null</exception>
        public Task<TRegistrationDescription> GetRegistrationAsync<TRegistrationDescription>(string registrationId) where TRegistrationDescription : RegistrationDescription
        {
            return GetRegistrationAsync<TRegistrationDescription>(registrationId, CancellationToken.None);
        }

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
        public Task<TRegistrationDescription> GetRegistrationAsync<TRegistrationDescription>(string registrationId, CancellationToken cancellationToken) where TRegistrationDescription : RegistrationDescription
        {
            if (string.IsNullOrWhiteSpace(registrationId))
            {
                throw new ArgumentNullException(nameof(registrationId));
            }

            return GetEntityImplAsync<TRegistrationDescription>("registrations", registrationId, cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        public Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(int top)
        {
            return GetAllRegistrationsImplAsync(null, top, null, null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="top">The location of the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        public Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(int top, CancellationToken cancellationToken)
        {
            return GetAllRegistrationsImplAsync(null, top, null, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        public Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(string continuationToken, int top)
        {
            return GetAllRegistrationsAsync(continuationToken, top, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location of the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        public Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(string continuationToken, int top, CancellationToken cancellationToken)
        {
            if (continuationToken == null)
            {
                throw new ArgumentNullException(nameof(continuationToken));
            }

            return GetAllRegistrationsImplAsync(continuationToken, top, null, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously gets the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        public Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, int top)
        {
            return GetAllRegistrationsImplAsync(null, top, pnsHandle, null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously gets the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="top">The location of the registration.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        public Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, int top, CancellationToken cancellationToken)
        {
            return GetAllRegistrationsImplAsync(null, top, pnsHandle, null, cancellationToken);
        }

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
        public Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, string continuationToken, int top)
        {
            return GetRegistrationsByChannelAsync(pnsHandle, continuationToken, top, CancellationToken.None);
        }

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
        public Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, string continuationToken, int top, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(pnsHandle))
            {
                throw new ArgumentNullException(nameof(pnsHandle));
            }

            return GetAllRegistrationsImplAsync(continuationToken, top, pnsHandle, null, cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registration">The registration to delete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when registration object is null.</exception>
        public Task DeleteRegistrationAsync(RegistrationDescription registration)
        {
            return DeleteRegistrationAsync(registration, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registration">The registration to delete.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when registration object is null.</exception>
        public Task DeleteRegistrationAsync(RegistrationDescription registration, CancellationToken cancellationToken)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            return DeleteRegistrationAsync(registration.RegistrationId, registration.ETag, cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task DeleteRegistrationAsync(string registrationId)
        {
            return DeleteRegistrationAsync(registrationId, "*");
        }

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task DeleteRegistrationAsync(string registrationId, CancellationToken cancellationToken)
        {
            return DeleteRegistrationAsync(registrationId, "*", cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes the registration.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <param name="etag">The entity tag.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">registrationId</exception>
        public Task DeleteRegistrationAsync(string registrationId, string etag)
        {
            return DeleteRegistrationAsync(registrationId, etag, CancellationToken.None);
        }

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
        public Task DeleteRegistrationAsync(string registrationId, string etag, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(registrationId))
            {
                throw new ArgumentNullException(nameof(registrationId));
            }

            return DeleteRegistrationImplAsync(registrationId, etag, cancellationToken);
        }

        /// <summary>
        /// Asynchronously deletes the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pnsHandle</exception>
        public Task DeleteRegistrationsByChannelAsync(string pnsHandle)
        {
            return DeleteRegistrationsByChannelAsync(pnsHandle, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously deletes the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pnsHandle</exception>
        public async Task DeleteRegistrationsByChannelAsync(string pnsHandle, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(pnsHandle))
            {
                throw new ArgumentNullException(nameof(pnsHandle));
            }

            var registrationsToDelete = await GetRegistrationsByChannelAsync(pnsHandle, EntitiesPerRequest, cancellationToken).ConfigureAwait(false);
            do
            {
                var deletionTasks = registrationsToDelete.Select(r => DeleteRegistrationImplAsync(r.RegistrationId, null, cancellationToken));
                await Task.WhenAll(deletionTasks).ConfigureAwait(false);
            }
            while (!string.IsNullOrEmpty(registrationsToDelete.ContinuationToken));
        }

        /// <summary>
        /// Asynchronously indicates that the registration already exists.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<bool> RegistrationExistsAsync(string registrationId)
        {
            return RegistrationExistsAsync(registrationId, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously indicates that the registration already exists.
        /// </summary>
        /// <param name="registrationId">The registration ID.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public async Task<bool> RegistrationExistsAsync(string registrationId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(registrationId))
                throw new ArgumentNullException(nameof(registrationId));

            return await GetEntityImplAsync<RegistrationDescription>("registrations", registrationId, cancellationToken, throwIfNotFound: false) != null;
        }

        /// <summary>
        /// Asynchronously gets the registrations by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="top">The location where to get the registrations.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        public Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, int top)
        {
            return GetRegistrationsByTagAsync(tag, top, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously gets the registrations by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="top">The location where to get the registrations.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>
        /// The task that completes the asynchronous operation, which will contain a null or empty continuation token when there is no additional data available in the query.
        /// </returns>
        public Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, int top, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return GetAllRegistrationsImplAsync(null, top, null, tag, cancellationToken);
        }

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
        public Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, string continuationToken, int top)
        {
            return GetRegistrationsByTagAsync(tag, continuationToken, top, CancellationToken.None);
        }

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
        public Task<ICollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, string continuationToken, int top, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return GetAllRegistrationsImplAsync(continuationToken, top, null, tag, cancellationToken);
        }

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime)
        {
            return ScheduleNotificationAsync(notification, scheduledTime, CancellationToken.None);
        }

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, CancellationToken cancellationToken)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            return SendScheduledNotificationImplAsync(notification, scheduledTime, null, cancellationToken);
        }

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when tags object is null</exception>
        /// <exception cref="System.ArgumentException">tags argument should contain at least one tag</exception>
        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, IEnumerable<string> tags)
        {
            return ScheduleNotificationAsync(notification, scheduledTime, tags, CancellationToken.None);
        }

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
        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, IEnumerable<string> tags, CancellationToken cancellationToken)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            if (tags.Count() == 0)
            {
                throw new ArgumentException(message: $"{nameof(tags)} argument should contain at least one value", paramName: nameof(tags));
            }

            string tagExpression = String.Join("||", tags);
            return SendScheduledNotificationImplAsync(notification, scheduledTime, tagExpression, cancellationToken);
        }

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="tagExpression">The tag expression.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, string tagExpression)
        {
            return ScheduleNotificationAsync(notification, scheduledTime, tagExpression, CancellationToken.None);
        }

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <param name="tagExpression">The tag expression.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime, string tagExpression, CancellationToken cancellationToken)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            return SendScheduledNotificationImplAsync(notification, scheduledTime, tagExpression, cancellationToken);
        }

        /// <summary>
        /// Cancels the notification asynchronously.
        /// </summary>
        /// <param name="scheduledNotificationId">The scheduled notification identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task CancelNotificationAsync(string scheduledNotificationId)
        {
            return CancelNotificationAsync(scheduledNotificationId, CancellationToken.None);
        }

        /// <summary>
        /// Cancels the notification asynchronously.
        /// </summary>
        /// <param name="scheduledNotificationId">The scheduled notification identifier.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CancelNotificationAsync(string scheduledNotificationId, CancellationToken cancellationToken)
        {
            if (scheduledNotificationId == null)
            {
                throw new ArgumentNullException(nameof(scheduledNotificationId));
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"schedulednotifications/{scheduledNotificationId}";

            await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Delete, requestUri.Uri, out var trackingId))
                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, ct).ConfigureAwait(false))
                {
                    return true;
                }
            }, cancellationToken);
        }

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
        public Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, string deviceHandle)
        {
            return SendDirectNotificationAsync(notification, deviceHandle, CancellationToken.None);
        }

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
        public Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, string deviceHandle, CancellationToken cancellationToken)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            if (string.IsNullOrEmpty(deviceHandle))
            {
                throw new ArgumentNullException(nameof(deviceHandle));
            }

            return SendNotificationImplAsync(notification, null, deviceHandle, cancellationToken);
        }

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
        public Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, IList<string> deviceHandles)
        {
            return SendDirectNotificationAsync(notification, deviceHandles, CancellationToken.None);
        }

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
        public async Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, IList<string> deviceHandles, CancellationToken cancellationToken)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            if (deviceHandles==null)
            {
                throw new ArgumentNullException(nameof(deviceHandles));
            }

            if (deviceHandles.Count == 0)
            {
                throw new ArgumentException(message: $"{nameof(deviceHandles)} should contain at least one value", paramName: nameof(deviceHandles));
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "messages/$batch";
            AddToQuery(requestUri, "&direct");

            // Convert FcmNotification into GcmNotification
            notification = FcmToGcmNotificationTypeCast(notification);

            notification.ValidateAndPopulateHeaders();

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Post, requestUri.Uri, out var trackingId))
                {
                    foreach (var item in notification.Headers)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }

                    var content = new MultipartContent("mixed", "nh-batch-multipart-boundary");

                    ParseContentType(notification.ContentType, out var mediaType, out var encoding);

                    var notificationContent = new StringContent(notification.Body, encoding, mediaType);
                    notificationContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { Name = "notification" };
                    content.Add(notificationContent);

                    var devicesContent = new StringContent(JsonConvert.SerializeObject(deviceHandles), Encoding.UTF8, "application/json");
                    devicesContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { Name = "devices" };
                    content.Add(devicesContent);

                    request.Content = content;

                    using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.Created, ct).ConfigureAwait(false))
                    {
                        return new NotificationOutcome()
                        {
                            State = NotificationOutcomeState.Enqueued,
                            TrackingId = trackingId,
                            NotificationId = GetNotificationIdFromResponse(response)
                        };
                    }
                }
            }, cancellationToken);
        }

        private T SyncOp<T>(Func<Task<T>> func)
        {
            try
            {
                return func().Result;
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }

        private void SyncOp(Func<Task> action)
        {
            try
            {
                action().Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }

        /// <summary>
        /// Returns the base URI for the notification hub client.
        /// </summary>
        /// <returns>
        /// The base URI of the notification hub.
        /// </returns>
        public Uri GetBaseUri()
        {
            return _baseUri;
        }

        private async Task<NotificationOutcome> SendNotificationImplAsync(Notification notification, string tagExpression, string deviceHandle, CancellationToken cancellationToken)
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "messages";

            if (!string.IsNullOrWhiteSpace(deviceHandle))
            {
                AddToQuery(requestUri, "&direct");
            }

            // Convert FcmNotification into GcmNotification
            notification = FcmToGcmNotificationTypeCast(notification);

            notification.ValidateAndPopulateHeaders();

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Post, requestUri.Uri, out var trackingId))
                {
                    if (!string.IsNullOrWhiteSpace(deviceHandle))
                    {
                        request.Headers.Add("ServiceBusNotification-DeviceHandle", deviceHandle);
                    }

                    if (!string.IsNullOrWhiteSpace(tagExpression))
                    {
                        request.Headers.Add("ServiceBusNotification-Tags", tagExpression);
                    }

                    foreach (var item in notification.Headers)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }

                    ParseContentType(notification.ContentType, out var mediaType, out var encoding);
                    request.Content = new StringContent(notification.Body, encoding, mediaType);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                    using (var response = await SendRequestAsync(request, trackingId, new[] { HttpStatusCode.OK, HttpStatusCode.Created }, ct).ConfigureAwait(false))
                    {
                        if (EnableTestSend)
                        {
                            using (var responseContent = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            using (var reader = XmlReader.Create(responseContent, new XmlReaderSettings { CloseInput = true }))
                            {
                                var result = (NotificationOutcome)_debugResponseSerializer.ReadObject(reader);
                                result.State = NotificationOutcomeState.DetailedStateAvailable;
                                result.TrackingId = trackingId;
                                return result;
                            }
                        }
                        else
                        {
                            var result = new NotificationOutcome
                            {
                                State = NotificationOutcomeState.Enqueued,
                                TrackingId = trackingId,
                                NotificationId = GetNotificationIdFromResponse(response)
                            };

                            return result;
                        }
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="mediaType"></param>
        /// <param name="encoding"></param>
        public static void ParseContentType(string contentType, out string mediaType, out Encoding encoding)
        {
            var splitted = contentType.Split(';');
            mediaType = splitted[0];
            if (!MediaTypeHeaderValue.TryParse(mediaType, out _))
            {
                throw new ArgumentException($"{nameof(contentType)} is not valid.");
            }

            encoding = Encoding.UTF8;

            if (splitted.Count() == 2)
            {
                var matches = Regex.Matches(splitted[1], @"charset\s*=[\s""']*([^\s""']*)", RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(matches[0].Groups[1].Value);
                    }
                    catch
                    {
                        throw new ArgumentException($"{nameof(contentType)} is not valid.");
                    }
                }
                else
                {
                    throw new ArgumentException($"{nameof(contentType)} is not valid.");
                }
            }
            else if (splitted.Count() > 2)
            {
                throw new ArgumentException($"{nameof(contentType)} is not valid.");
            }
        }

        private async Task<ScheduledNotification> SendScheduledNotificationImplAsync(Notification notification, DateTimeOffset scheduledTime, string tagExpression, CancellationToken cancellationToken)
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "schedulednotifications";

            // Convert FcmNotification into GcmNotification
            notification = FcmToGcmNotificationTypeCast(notification);

            notification.ValidateAndPopulateHeaders();

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Post, requestUri.Uri, out var trackingId))
                {
                    request.Headers.Add("ServiceBusNotification-ScheduleTime", scheduledTime.UtcDateTime.ToString("s", CultureInfo.InvariantCulture));

                    if (!string.IsNullOrWhiteSpace(tagExpression))
                    {
                        request.Headers.Add("ServiceBusNotification-Tags", tagExpression);
                    }

                    foreach (var item in notification.Headers)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }

                    ParseContentType(notification.ContentType, out var mediaType, out var encoding);
                    request.Content = new StringContent(notification.Body, encoding, mediaType);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                    using (var response = await SendRequestAsync(request, trackingId, new[] { HttpStatusCode.OK, HttpStatusCode.Created }, ct).ConfigureAwait(false))
                    {
                        string notificationId = null;
                        if (response.Headers.Location != null)
                        {
                            notificationId = response.Headers.Location.Segments.Last().Trim('/');
                        }

                        var result = new ScheduledNotification()
                        {
                            ScheduledNotificationId = notificationId,
                            Tags = tagExpression,
                            ScheduledTime = scheduledTime,
                            Payload = notification,
                            TrackingId = trackingId
                        };

                        return result;
                    }
                }
            }, cancellationToken);
        }

        private async Task<ICollectionQueryResult<TEntity>> GetAllEntitiesImplAsync<TEntity>(UriBuilder requestUri, string continuationToken, int top, CancellationToken cancellationToken) where TEntity : EntityDescription
        {
            if (top > 0)
            {
                AddToQuery(requestUri, $"&$top={top}");
            }

            if (!string.IsNullOrWhiteSpace(continuationToken))
            {
                AddToQuery(requestUri, $"&{Constants.ContinuationTokenQueryName}={continuationToken}");
            }

            using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
            using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, cancellationToken).ConfigureAwait(false))
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var entities = await ReadEntitiesAsync<TEntity>(responseStream).ConfigureAwait(false);
                    string newContinuationToken = null;
                    if (response.Headers.TryGetValues(Constants.ContinuationTokenHeaderName, out var continuationTokenHeaderValues))
                    {
                        newContinuationToken = continuationTokenHeaderValues.First();
                    }

                    return new CollectionQueryResult<TEntity>(entities, newContinuationToken);
                }
            }

        }

        private Task<ICollectionQueryResult<RegistrationDescription>> GetAllRegistrationsImplAsync(string continuationToken, int top, string deviceHandle, string tag, CancellationToken cancellationToken)
        {
            var requestUri = GetGenericRequestUriBuilder();

            if (string.IsNullOrWhiteSpace(tag))
            {
                requestUri.Path += "registrations";

                if (!string.IsNullOrWhiteSpace(deviceHandle))
                {
                    AddToQuery(requestUri, "&$filter=" + SharedAccessSignatureBuilder.UrlEncode($"ChannelUri eq '{deviceHandle}'"));
                }
            }
            else
            {
                requestUri.Path += $"tags/{tag}/registrations";
            }

            return GetAllEntitiesImplAsync<RegistrationDescription>(requestUri, continuationToken, top, cancellationToken);
        }

        private async Task DeleteRegistrationImplAsync(string registrationId, string etag, CancellationToken cancellationToken)
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"registrations/{registrationId}";

            using (var request = CreateHttpRequest(HttpMethod.Delete, requestUri.Uri, out var trackingId))
            {
                request.Headers.Add(ManagementStrings.IfMatch, string.IsNullOrWhiteSpace(etag) ? "*" : $"\"{etag}\"");

                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, cancellationToken).ConfigureAwait(false))
                {
                }
            }
        }

        private async Task<TRegistration> CreateOrUpdateRegistrationImplAsync<TRegistration>(TRegistration registration, EntityOperatonType operationType, CancellationToken cancellationToken) where TRegistration : RegistrationDescription
        {
            registration = (TRegistration)registration.Clone();
            registration.NotificationHubPath = _notificationHubPath;
            registration.ExpirationTime = null;

            RegistrationSDKHelper.ValidateRegistration(registration);

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "registrations";

            switch (operationType)
            {
                case EntityOperatonType.Create:
                    registration.ETag = null;
                    break;
                case EntityOperatonType.CreateOrUpdate:
                    registration.ETag = null;
                    requestUri.Path += $"/{registration.RegistrationId}";
                    break;
                case EntityOperatonType.Update:
                    requestUri.Path += $"/{registration.RegistrationId}";
                    break;
            }

            registration.RegistrationId = null;

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(operationType == EntityOperatonType.Create ? HttpMethod.Post : HttpMethod.Put, requestUri.Uri, out var trackingId))
                {
                    if (operationType == EntityOperatonType.Update)
                    {
                        request.Headers.Add(ManagementStrings.IfMatch, string.IsNullOrEmpty(registration.ETag) ? "*" : $"\"{registration.ETag}\"");
                    }

                    AddEntityToRequestContent(request, registration);

                    using (var response = await SendRequestAsync(request, trackingId, new[] { HttpStatusCode.OK, HttpStatusCode.Created }, ct).ConfigureAwait(false))
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            return await ReadEntityAsync<TRegistration>(responseStream).ConfigureAwait(false);
                        }
                    }
                }
            }, cancellationToken);
        }

        private async Task<TEntity> GetEntityImplAsync<TEntity>(string entityCollection, string entityId, CancellationToken cancellationToken, bool throwIfNotFound = true) where TEntity : EntityDescription
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"{entityCollection}/{entityId}";

            HttpStatusCode[] successfulResponseStatuses;
            if (throwIfNotFound)
            {
                successfulResponseStatuses = new[] { HttpStatusCode.OK };
            }
            else
            {
                successfulResponseStatuses = new[] { HttpStatusCode.OK, HttpStatusCode.NotFound };
            }

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
                {
                    using (var response = await SendRequestAsync(request, trackingId, successfulResponseStatuses, ct).ConfigureAwait(false))
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                            return null;

                        using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            return await ReadEntityAsync<TEntity>(responseStream).ConfigureAwait(false);
                        }
                    }
                }
            }, cancellationToken);
        }

        private HttpRequestMessage CreateHttpRequest(HttpMethod method, Uri requestUri, out string trackingId)
        {
            trackingId = Guid.NewGuid().ToString();

            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Add("TrackingId", trackingId);
            request.Headers.Add("Authorization", _tokenProvider.GetToken(requestUri.ToString()));

            return request;
        }

        private UriBuilder GetGenericRequestUriBuilder()
        {
            var uriBuilder = new UriBuilder(_baseUri)
            {
                Scheme = Uri.UriSchemeHttps
            };

            if (!uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
            {
                uriBuilder.Path += "/";
            }

            uriBuilder.Path = $"{uriBuilder.Path}{_notificationHubPath}/";
            AddToQuery(uriBuilder, $"{ManagementStrings.ApiVersionName}={ManagementStrings.ApiVersion}");

            if (EnableTestSend)
            {
                AddToQuery(uriBuilder, "&test");
            }

            return uriBuilder;
        }

        private static Uri GetBaseUri(KeyValueConfigurationManager manager)
        {
            var endpointString = manager.connectionProperties[KeyValueConfigurationManager.EndpointConfigName];
            return new Uri(endpointString);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, string trackingId, HttpStatusCode successfulResponseStatus, CancellationToken cancellationToken)
        {
            return await SendRequestAsync(request, trackingId, new[] { successfulResponseStatus }, cancellationToken).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, string trackingId, HttpStatusCode[] successfulResponseStatuses, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
                if (!successfulResponseStatuses.Any(s => s.Equals(response.StatusCode)))
                {
                    throw await response.TranslateToMessagingExceptionAsync(trackingId).ConfigureAwait(false);
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                var innerException = ex.GetBaseException();
                if (innerException is SocketException socketException)
                {
                    throw ExceptionsUtility.HandleSocketException(socketException, OperationTimeout.Milliseconds, trackingId);
                }
                else
                {
                    throw ExceptionsUtility.HandleUnexpectedException(ex, trackingId);
                }
            }
            catch (XmlException ex)
            {
                throw ExceptionsUtility.HandleXmlException(ex, trackingId);
            }
        }

        private string GetNotificationIdFromResponse(HttpResponseMessage response)
        {
            if (response.Headers.Location != null)
            {
                return response.Headers.Location.Segments[response.Headers.Location.Segments.Length - 1];
            }

            return string.Empty;
        }

        private void SetUserAgent()
        {
            if (!_httpClient.DefaultRequestHeaders.Contains(Constants.HttpUserAgentHeaderName))
            {
                _httpClient.DefaultRequestHeaders.Add(Constants.HttpUserAgentHeaderName,
                    $"NHub/{ManagementStrings.ApiVersion} (api-origin=DotNetSdk;os={Environment.OSVersion.Platform};os-version={Environment.OSVersion.Version})");
            }
        }

        private async Task<List<TEntity>> ReadEntitiesAsync<TEntity>(Stream source) where TEntity : EntityDescription
        {
            var result = new List<TEntity>();

            using (var xmlReader = XmlReader.Create(source, new XmlReaderSettings { Async = true }))
            {
                // Advancing to the first element skipping non-content nodes
                await xmlReader.MoveToContentAsync().ConfigureAwait(false);

                if (!xmlReader.IsStartElement("feed"))
                {
                    throw new FormatException("Required 'feed' element is missing");
                }

                // Advancing to the next Atom entry
                while (xmlReader.ReadToFollowing("entry"))
                {
                    // Advancing to content of the Atom entry
                    if (xmlReader.ReadToDescendant("content"))
                    {
                        xmlReader.ReadStartElement();
                        var entity = (TEntity)_entitySerializer.Deserialize(xmlReader, xmlReader.Name);

#pragma warning disable CS0618

                        if (entity is GcmTemplateRegistrationDescription)
                        {
                            var fcmTemplateRegistrationDescription = new FcmTemplateRegistrationDescription(entity as GcmTemplateRegistrationDescription);
                            entity = (fcmTemplateRegistrationDescription as TEntity);
                        }
                        
                        if (entity is GcmRegistrationDescription)
                        {
                            var fcmRegistrationDescription = new FcmRegistrationDescription(entity as GcmRegistrationDescription);
                            entity = (fcmRegistrationDescription as TEntity);
                        }

#pragma warning restore CS0618

                        result.Add(entity);
                    }
                }
            }

            return result;
        }

        private async Task<TEntity> ReadEntityAsync<TEntity>(Stream source) where TEntity : EntityDescription
        {
            using (var xmlReader = XmlReader.Create(source, new XmlReaderSettings { Async = true }))
            {
                await xmlReader.MoveToContentAsync().ConfigureAwait(false);
                if (xmlReader.Name != "entry")
                {
                    throw new FormatException("Required 'entry' element is missing");
                }

                xmlReader.ReadToDescendant("content");
                xmlReader.ReadStartElement();

                var entity = _entitySerializer.Deserialize(xmlReader, xmlReader.Name);

#pragma warning disable CS0618

                if (typeof(GcmRegistrationDescription).IsAssignableFrom(typeof(TEntity)))
                {
                    return (TEntity)entity;
                }

                if (entity is GcmTemplateRegistrationDescription gcmTemplateRegistrationDescription)
                {
                    var fcmTemplateRegistrationDescription = new FcmTemplateRegistrationDescription(gcmTemplateRegistrationDescription);
                    return (fcmTemplateRegistrationDescription as TEntity);
                }

                if (entity is GcmRegistrationDescription gcmRegistrationDescription)
                {
                    var fcmRegistrationDescription = new FcmRegistrationDescription(gcmRegistrationDescription);
                    return (fcmRegistrationDescription as TEntity);
                }

#pragma warning restore CS0618

                return (TEntity)entity;
            }
        }

        private void AddEntityToRequestContent(HttpRequestMessage request, EntityDescription entity)
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement("entry", "http://www.w3.org/2005/Atom");
                writer.WriteStartElement("content");
                writer.WriteAttributeString("type", "application/xml");
                _entitySerializer.Serialize(entity, writer);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            request.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/atom+xml");
            request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("type", "entry"));
        }

        private static void AddToQuery(UriBuilder uriBuilder, string query)
        {
            if (string.IsNullOrEmpty(uriBuilder.Query))
            {
                uriBuilder.Query = query;
            }
            else
            {
                // Removing leading '?' from the result query
                uriBuilder.Query = uriBuilder.Query.Substring(1) + query;
            }
        }

#pragma warning disable CS0618  

        private static Notification FcmToGcmNotificationTypeCast(Notification notification)
        {
            if (notification.GetType().Name == "FcmNotification")
            {
                notification = new GcmNotification((FcmNotification) notification);
            }

            return notification;
        }

#pragma warning restore CS0618
    }
}
