//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using Auth;
    using Messaging;
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
    using System.Web;
    using System.Xml;

    /// <summary>
    /// Represents a notification hub client.
    /// </summary>
    public class NotificationHubClient
    {
        private const int EntitiesPerRequest = 100;
        private readonly HttpClient _httpClient;

        private readonly Uri _baseUri;
        private readonly DataContractSerializer _debugResponseSerializer = new DataContractSerializer(typeof(NotificationOutcome));
        private readonly DataContractSerializer _notificationDetailsSerializer = new DataContractSerializer(typeof(NotificationDetails));
        private readonly EntityDescriptionSerializer _entitySerializer = new EntityDescriptionSerializer();
        private readonly string _notificationHubPath;
        private readonly TokenProvider _tokenProvider;

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
        public NotificationHubClient(string connectionString, string notificationHubPath, NotificationHubClientSettings settings)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrWhiteSpace(notificationHubPath))
            {
                throw new ArgumentNullException("notificationHubPath");
            }

            _notificationHubPath = notificationHubPath;
            _tokenProvider = Auth.SharedAccessSignatureTokenProvider.CreateSharedAccessSignatureTokenProvider(connectionString);
            var configurationManager = new KeyValueConfigurationManager(connectionString);
            _baseUri = GetBaseUri(configurationManager);

            if (settings?.MessageHandler != null)
            {
                var httpClientHandler = settings?.MessageHandler;
                _httpClient = new HttpClient(httpClientHandler);
            }
            else if (settings?.Proxy != null)
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

            if (settings?.OperationTimeout == null)
            {
                OperationTimeout = TimeSpan.FromSeconds(60);
            }
            else
            {
                OperationTimeout = settings.OperationTimeout.Value;
            }

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
            return SendWindowsNativeNotificationAsync(windowsNativePayload, string.Empty);
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
            return SendNotificationAsync(new WindowsNotification(windowsNativePayload), tagExpression);
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
            return SendNotificationAsync(new WindowsNotification(windowsNativePayload), tags);
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
            if (jobId == null)
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            return GetEntityImplAsync<NotificationHubJob>("jobs", jobId, CancellationToken.None);
        }

        /// <summary>
        /// Returns all known <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s. This method
        /// is used to get the status of all job to see if those jobs completed, failed, or are still in progress.
        /// This API is only available for Standard namespaces.
        /// </summary>
        /// <returns>
        /// The current state of the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob" />s.
        /// </returns>
        public async Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync()
        {
            var requestUri = GetGenericRequestUriBuilder();

            requestUri.Path += "jobs";

            var jobs = await GetAllEntitiesImplAsync<NotificationHubJob>(requestUri, null, EntitiesPerRequest, CancellationToken.None).ConfigureAwait(false);
            return jobs;
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
        public async Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job");
            }

            if (job.OutputContainerUri == null)
            {
                throw new ArgumentNullException("OutputContainerUri");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "jobs";

            using (var request = CreateHttpRequest(HttpMethod.Post, requestUri.Uri, out var trackingId))
            {
                AddEntityToRequestContent(request, job);

                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.Created, CancellationToken.None).ConfigureAwait(false))
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        return await ReadEntityAsync<NotificationHubJob>(responseStream).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Sends an Apple native notification. To specify an expiry, use the <see cref="M:Microsoft.Azure.NotificationHubs.NotificationHubClient.SendNotificationAsync(Microsoft.Azure.NotificationHubs.Notification)" /> method.
        /// </summary>
        /// <param name="jsonPayload">This is a valid Apple Push Notification Service (APNS) payload.
        /// Documentation on the APNS payload can be found
        /// <a href="https://developer.apple.com/library/ios/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/Chapters/ApplePushService.html">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload)
        {
            return SendAppleNativeNotificationAsync(jsonPayload, string.Empty);
        }

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
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, string tagExpression)
        {
            return SendNotificationAsync(new AppleNotification(jsonPayload), tagExpression);
        }

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
        public Task<NotificationOutcome> SendAppleNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new AppleNotification(jsonPayload), tags);
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
        /// Sends Google Cloud Messaging (GCM) native notification.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload. Documentation on proper formatting of a GCM message can be found <a href="https://developers.google.com/cloud-messaging/downstream#notifications_and_data_messages">here</a>.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        [Obsolete("SendGcmNativeNotificationAsync is deprecated, please use SendFcmNativeNotificationAsync instead.")]
        public Task<NotificationOutcome> SendGcmNativeNotificationAsync(string jsonPayload)
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
        public Task<NotificationOutcome> SendGcmNativeNotificationAsync(string jsonPayload, string tagExpression)
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
        public Task<NotificationOutcome> SendGcmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
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
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendBaiduNativeNotificationAsync(string message, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new BaiduNotification(message), tags);
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
        /// <param name="tags">A non-empty set of tags (maximum 20 tags). Each string in the set can contain a single tag.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        public Task<NotificationOutcome> SendAdmNativeNotificationAsync(string jsonPayload, IEnumerable<string> tags)
        {
            return SendNotificationAsync(new AdmNotification(jsonPayload), tags);
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
        /// Sends a notification to a non-empty set of tags (max 20). This is equivalent to a tag expression with boolean ORs ("||").
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        /// <returns>
        ///   <see cref="Microsoft.Azure.NotificationHubs.NotificationOutcome" /> which describes the result of the Send operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">notification</exception>
        public Task<NotificationOutcome> SendNotificationAsync(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            return SendNotificationImplAsync(notification, notification.tag, null, CancellationToken.None);
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
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            if (notification.tag != null)
            {
                throw new ArgumentException("notification.Tag property should be null");
            }

            return SendNotificationImplAsync(notification, tagExpression, null, CancellationToken.None);
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
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            if (notification.tag != null)
            {
                throw new ArgumentException("notification.Tag property should be null");
            }

            if (tags == null)
            {
                throw new ArgumentNullException("tags");
            }

            if (tags.Count() == 0)
            {
                throw new ArgumentException("tags argument should contain at least one tag");
            }

            string tagExpression = string.Join("||", tags);
            return SendNotificationImplAsync(notification, tagExpression, null, CancellationToken.None);
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
        public async Task<NotificationDetails> GetNotificationOutcomeDetailsAsync(string notificationId)
        {
            if (String.IsNullOrWhiteSpace(notificationId))
            {
                throw new ArgumentNullException("notificationId");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"messages/{notificationId}";

            using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
            {
                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK,  CancellationToken.None).ConfigureAwait(false))
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        return (NotificationDetails)_notificationDetailsSerializer.ReadObject(responseStream);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the feedback container URI asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<Uri> GetFeedbackContainerUriAsync()
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "feedbackcontainer";

            using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
            {
                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, CancellationToken.None).ConfigureAwait(false))
                {
                    return new Uri(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }
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
        public async Task CreateOrUpdateInstallationAsync(Installation installation)
        {
            if (installation==null)
            {
                throw new ArgumentNullException("installation");
            }

            if (String.IsNullOrWhiteSpace(installation.InstallationId))
            {
                throw new InvalidOperationException("InstallationId must be specified");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installation.InstallationId}";

            using (var request = CreateHttpRequest(HttpMethod.Put, requestUri.Uri, out var trackingId))
            {
                request.Content = new StringContent(installation.ToJson(), Encoding.UTF8, "application/json");

                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, CancellationToken.None).ConfigureAwait(false))
                {
                }
            }
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
        public async Task PatchInstallationAsync(string installationId, IList<PartialUpdateOperation> operations)
        {
            if (String.IsNullOrWhiteSpace(installationId))
            {
                throw new ArgumentNullException("installationId");
            }

            if (operations == null)
            {
                throw new ArgumentNullException("operations");
            }

            if (operations.Count == 0)
            {
                throw new InvalidOperationException("Operations list is empty");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installationId}";

            using (var request = CreateHttpRequest(new HttpMethod("PATCH"), requestUri.Uri, out var trackingId))
            {
                request.Content = new StringContent(operations.ToJson(), Encoding.UTF8, "application/json-patch+json");

                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, CancellationToken.None).ConfigureAwait(false))
                {
                }
            }
            
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
        public async Task DeleteInstallationAsync(string installationId)
        {
            if (String.IsNullOrWhiteSpace(installationId))
            {
                throw new ArgumentNullException("installationId");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installationId}";

            using (var request = CreateHttpRequest(HttpMethod.Delete, requestUri.Uri, out var trackingId))
            {
                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.NoContent, CancellationToken.None).ConfigureAwait(false))
                {
                }
            }
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
        public async Task<Installation> GetInstallationAsync(string installationId)
        {
            if (String.IsNullOrWhiteSpace(installationId))
            {
                throw new ArgumentNullException("installationId");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"installations/{installationId}";

            using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
            {
                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, CancellationToken.None).ConfigureAwait(false))
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<Installation>(responseContent);
                }
            }
        }

        /// <summary>
        /// Asynchronously creates a registration identifier.
        /// </summary>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public async Task<string> CreateRegistrationIdAsync()
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "registrationids";

            string registrationId = null;

            using (var request = CreateHttpRequest(HttpMethod.Post, requestUri.Uri, out var trackingId))
            using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.Created, CancellationToken.None).ConfigureAwait(false))
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
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<WindowsRegistrationDescription> CreateWindowsNativeRegistrationAsync(string channelUri, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new WindowsRegistrationDescription(new Uri(channelUri), tags));
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
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleRegistrationDescription> CreateAppleNativeRegistrationAsync(string deviceToken, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new AppleRegistrationDescription(deviceToken, tags));
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
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<AppleTemplateRegistrationDescription> CreateAppleTemplateRegistrationAsync(string deviceToken, string jsonPayload, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new AppleTemplateRegistrationDescription(deviceToken, jsonPayload, tags));
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
        /// <param name="tags">The tags for the registration.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmRegistrationDescription> CreateAdmNativeRegistrationAsync(string admRegistrationId, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new AdmRegistrationDescription(admRegistrationId, tags));
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
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public Task<AdmTemplateRegistrationDescription> CreateAdmTemplateRegistrationAsync(string admRegistrationId, string jsonPayload, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new AdmTemplateRegistrationDescription(admRegistrationId, jsonPayload, tags));
        }

        /// <summary>
        /// Asynchronously creates GCM native registration.
        /// </summary>
        /// <param name="gcmRegistrationId">The GCM registration ID.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        [Obsolete("CreateGcmNativeRegistrationAsync is deprecated, please use CreateFcmNativeRegistrationAsync instead.")]
        public Task<GcmRegistrationDescription> CreateGcmNativeRegistrationAsync(string gcmRegistrationId)
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
        public Task<GcmRegistrationDescription> CreateGcmNativeRegistrationAsync(string gcmRegistrationId, IEnumerable<string> tags)
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
        public Task<GcmTemplateRegistrationDescription> CreateGcmTemplateRegistrationAsync(string gcmRegistrationId, string jsonPayload)
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
        public Task<GcmTemplateRegistrationDescription> CreateGcmTemplateRegistrationAsync(string gcmRegistrationId, string jsonPayload, IEnumerable<string> tags)
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
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmRegistrationDescription> CreateFcmNativeRegistrationAsync(string fcmRegistrationId, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new FcmRegistrationDescription(fcmRegistrationId, tags));
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
        /// <param name="tags">The tags.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<FcmTemplateRegistrationDescription> CreateFcmTemplateRegistrationAsync(string fcmRegistrationId, string jsonPayload, IEnumerable<string> tags)
        {
            return CreateRegistrationAsync(new FcmTemplateRegistrationDescription(fcmRegistrationId, jsonPayload, tags));
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
            if (!string.IsNullOrWhiteSpace(registration.NotificationHubPath) &&
                registration.NotificationHubPath != _notificationHubPath)
            {
                throw new ArgumentException("NotificationHubPath in RegistrationDescription is not valid.");
            }

            if (!string.IsNullOrWhiteSpace(registration.RegistrationId))
            {
                throw new ArgumentException("RegistrationId should be null or empty");
            }

            return CreateOrUpdateRegistrationImplAsync(registration, EntityOperatonType.Create, CancellationToken.None);
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
            if (string.IsNullOrWhiteSpace(registration.RegistrationId))
            {
                throw new ArgumentNullException("RegistrationId");
            }

            if (string.IsNullOrWhiteSpace(registration.ETag))
            {
                throw new ArgumentNullException("ETag");
            }

            return CreateOrUpdateRegistrationImplAsync(registration, EntityOperatonType.Update, CancellationToken.None);
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
            if (string.IsNullOrWhiteSpace(registration.RegistrationId))
            {
                throw new ArgumentNullException("RegistrationId");
            }

            return CreateOrUpdateRegistrationImplAsync(registration, EntityOperatonType.CreateOrUpdate, CancellationToken.None);
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
            if (string.IsNullOrWhiteSpace(registrationId))
            {
                throw new ArgumentNullException("registrationId");
            }

            return GetEntityImplAsync<TRegistrationDescription>("registrations", registrationId, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(int top)
        {
            return GetAllRegistrationsImplAsync(null, top, null, null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously retrieves all registrations in this notification hub.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsAsync(string continuationToken, int top)
        {
            if (continuationToken == null)
            {
                throw new ArgumentNullException(nameof(continuationToken));
            }

            return GetAllRegistrationsImplAsync(continuationToken, top, null, null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously gets the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, int top)
        {
            return GetAllRegistrationsImplAsync(null, top, pnsHandle, null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously gets the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location of the registration.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pnsHandle</exception>
        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByChannelAsync(string pnsHandle, string continuationToken, int top)
        {
            if (string.IsNullOrWhiteSpace(pnsHandle))
            {
                throw new ArgumentNullException("pnsHandle");
            }

            return GetAllRegistrationsImplAsync(continuationToken, top, pnsHandle, null, CancellationToken.None);
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
            if (registration == null)
            {
                throw new ArgumentNullException("registration");
            }

            return DeleteRegistrationAsync(registration.RegistrationId, registration.ETag);
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
        /// <param name="etag">The entity tag.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">registrationId</exception>
        public Task DeleteRegistrationAsync(string registrationId, string etag)
        {
            if (string.IsNullOrWhiteSpace(registrationId))
            {
                throw new ArgumentNullException("registrationId");
            }

            return DeleteRegistrationImplAsync(registrationId, etag, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously deletes the registrations by channel.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">pnsHandle</exception>
        public async Task DeleteRegistrationsByChannelAsync(string pnsHandle)
        {
            if (string.IsNullOrWhiteSpace(pnsHandle))
            {
                throw new ArgumentNullException("pnsHandle");
            }

            var registrationsToDelete = await GetRegistrationsByChannelAsync(pnsHandle, EntitiesPerRequest).ConfigureAwait(false);
            do
            {
                var deletionTasks = registrationsToDelete.Select(r => DeleteRegistrationImplAsync(r.RegistrationId, null, CancellationToken.None));
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
        public async Task<bool> RegistrationExistsAsync(string registrationId)
        {
            return await GetRegistrationAsync<RegistrationDescription>(registrationId).ConfigureAwait(false) != null;
        }

        /// <summary>
        /// Asynchronously gets the registrations by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="top">The location where to get the registrations.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, int top)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException("tag");
            }

            return GetAllRegistrationsImplAsync(null, top, null, tag, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously gets the registrations by tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="top">The location where to get the registrations.</param>
        /// <returns>
        /// The task that completes the asynchronous operation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown when tag object is null</exception>
        public Task<CollectionQueryResult<RegistrationDescription>> GetRegistrationsByTagAsync(string tag, string continuationToken, int top)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException("tag");
            }

            return GetAllRegistrationsImplAsync(continuationToken, top, null, tag, CancellationToken.None);
        }

        /// <summary>
        /// Schedules the notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="scheduledTime">The scheduled time.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task<ScheduledNotification> ScheduleNotificationAsync(Notification notification, DateTimeOffset scheduledTime)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            return SendScheduledNotificationImplAsync(notification, scheduledTime, null, CancellationToken.None);
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
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            if (tags == null)
            {
                throw new ArgumentNullException("tags");
            }

            if (tags.Count() == 0)
            {
                throw new ArgumentException("tags argument should contain at least one tag");
            }

            string tagExpression = String.Join("||", tags);
            return SendScheduledNotificationImplAsync(notification, scheduledTime, tagExpression, CancellationToken.None);
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
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            return SendScheduledNotificationImplAsync(notification, scheduledTime, tagExpression, CancellationToken.None);
        }

        /// <summary>
        /// Cancels the notification asynchronously.
        /// </summary>
        /// <param name="scheduledNotificationId">The scheduled notification identifier.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CancelNotificationAsync(string scheduledNotificationId)
        {
            if (scheduledNotificationId == null)
            {
                throw new ArgumentNullException(nameof(scheduledNotificationId));
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"schedulednotifications/{scheduledNotificationId}";

            using (var request = CreateHttpRequest(HttpMethod.Delete, requestUri.Uri, out var trackingId))
            using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, CancellationToken.None).ConfigureAwait(false))
            {
            }
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
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            if (string.IsNullOrEmpty(deviceHandle))
            {
                throw new ArgumentNullException("deviceHandle");
            }

            return SendNotificationImplAsync(notification, null, deviceHandle, CancellationToken.None);
        }

        /// <summary>
        /// Sends a notification directly to all devices listed in deviceHandles (a valid tokens as expressed by the Notification type).
        /// Users of this API do not use Registrations or Installations. Instead, users of this API manage all devices
        /// on their own and use Azure Notification Hub solely as a pass through service to communicate with
        /// the various Push Notification Services.
        /// </summary>
        /// <param name="notification">A instance of a Notification, identifying which Push Notification Service to send to.</param>
        /// <param name="deviceHandles">A list of valid device identifiers.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when notification or deviceHandles object is null
        /// </exception>
        public async Task<NotificationOutcome> SendDirectNotificationAsync(Notification notification, IList<string> deviceHandles)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }

            if (deviceHandles==null)
            {
                throw new ArgumentNullException("deviceHandles");
            }

            if (deviceHandles.Count == 0)
            {
                throw new ArgumentException("deviceHandles");
            }

            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "messages/$batch";
            AddToQuery(requestUri, "&direct");

            notification.ValidateAndPopulateHeaders();

            using (var request = CreateHttpRequest(HttpMethod.Post, requestUri.Uri, out var trackingId))
            {
                foreach (var item in notification.Headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }

                var content = new MultipartContent("mixed", "nh-batch-multipart-boundary");

                var notificationContent = new StringContent(notification.Body, Encoding.UTF8, notification.ContentType);
                notificationContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { Name = "notification" };
                content.Add(notificationContent);

                var devicesContent = new StringContent(JsonConvert.SerializeObject(deviceHandles), Encoding.UTF8, "application/json");
                devicesContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { Name = "devices" };
                content.Add(devicesContent);

                request.Content = content;

                using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.Created, CancellationToken.None).ConfigureAwait(false))
                {
                    return new NotificationOutcome()
                    {
                        State = NotificationOutcomeState.Enqueued,
                        TrackingId = trackingId,
                        NotificationId = GetNotificationIdFromResponse(response)
                    };
                }
            }
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


            notification.ValidateAndPopulateHeaders();

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

                request.Content = new StringContent(notification.Body, Encoding.UTF8, notification.ContentType);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(notification.ContentType);

                using (var response = await SendRequestAsync(request, trackingId, new[] { HttpStatusCode.OK, HttpStatusCode.Created }, cancellationToken).ConfigureAwait(false))
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
        }

        private async Task<ScheduledNotification> SendScheduledNotificationImplAsync(Notification notification, DateTimeOffset scheduledTime, string tagExpression, CancellationToken cancellationToken)
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += "schedulednotifications";

            notification.ValidateAndPopulateHeaders();

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

                request.Content = new StringContent(notification.Body, Encoding.UTF8, notification.ContentType);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(notification.ContentType);

                using (var response = await SendRequestAsync(request, trackingId, new[] { HttpStatusCode.OK, HttpStatusCode.Created }, cancellationToken).ConfigureAwait(false))
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
        }

        private async Task<CollectionQueryResult<TEntity>> GetAllEntitiesImplAsync<TEntity>(UriBuilder requestUri, string continuationToken, int top, CancellationToken cancellationToken) where TEntity : EntityDescription
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

        private Task<CollectionQueryResult<RegistrationDescription>> GetAllRegistrationsImplAsync(string continuationToken, int top, string deviceHandle, string tag, CancellationToken cancellationToken)
        {
            var requestUri = GetGenericRequestUriBuilder();

            if (string.IsNullOrWhiteSpace(tag))
            {
                requestUri.Path += "registrations";

                if (!string.IsNullOrWhiteSpace(deviceHandle))
                {
                    AddToQuery(requestUri, "&$filter=" + HttpUtility.UrlEncode($"ChannelUri eq '{deviceHandle}'"));
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


            using (var request = CreateHttpRequest(operationType == EntityOperatonType.Create ? HttpMethod.Post : HttpMethod.Put, requestUri.Uri, out var trackingId))
            {
                if (operationType == EntityOperatonType.Update)
                {
                    request.Headers.Add(ManagementStrings.IfMatch, string.IsNullOrEmpty(registration.ETag) ? "*" : $"\"{registration.ETag}\"");
                }

                AddEntityToRequestContent(request, registration);

                using (var response = await SendRequestAsync(request, trackingId, new[] { HttpStatusCode.OK, HttpStatusCode.Created }, cancellationToken).ConfigureAwait(false))
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        return await ReadEntityAsync<TRegistration>(responseStream).ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task<TEntity> GetEntityImplAsync<TEntity>(string entityCollection, string entityId, CancellationToken cancellationToken) where TEntity : EntityDescription
        {
            var requestUri = GetGenericRequestUriBuilder();
            requestUri.Path += $"{entityCollection}/{entityId}";

            using (var request = CreateHttpRequest(HttpMethod.Get, requestUri.Uri, out var trackingId))
            using (var response = await SendRequestAsync(request, trackingId, HttpStatusCode.OK, cancellationToken).ConfigureAwait(false))
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    return await ReadEntityAsync<TEntity>(responseStream).ConfigureAwait(false);
                }

            }
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
                    throw await response.TranslateToMessagingExceptionAsync(request.Method.Method, OperationTimeout.Milliseconds, trackingId).ConfigureAwait(false);
                }

                return response;
            }
            catch (Exception e) when (!e.IsMessagingException())
            {
                throw e.TranslateToMessagingException(OperationTimeout.Milliseconds, trackingId);
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
                    $"NHub/{ApiVersionConstants.MaxSupportedApiVersion} (api-origin=DotNetSdk;os={Environment.OSVersion.Platform};os-version={Environment.OSVersion.Version})");
            }
        }

        private async Task<List<TEntity>> ReadEntitiesAsync<TEntity>(Stream source) where TEntity : EntityDescription
        {
            var result = new List<TEntity>();

            using (var xmlReader = XmlReader.Create(source, new XmlReaderSettings { Async = true }))
            {
                // Advancing to the first element skiping non-content nodes
                await xmlReader.MoveToContentAsync().ConfigureAwait(false);

                if (!xmlReader.IsStartElement("feed"))
                {
                    throw new FormatException("Required 'feed' element is missing");
                }

                // Advancing to the next Atom entry
                while (xmlReader.ReadToFollowing("entry"))
                {
                    // Anvancing to content of the Atom entry
                    if (xmlReader.ReadToDescendant("content"))
                    {
                        xmlReader.ReadStartElement();
                        result.Add((TEntity)_entitySerializer.Deserialize(xmlReader, xmlReader.Name));
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

                return (TEntity)_entitySerializer.Deserialize(xmlReader, xmlReader.Name);
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
    }
}
