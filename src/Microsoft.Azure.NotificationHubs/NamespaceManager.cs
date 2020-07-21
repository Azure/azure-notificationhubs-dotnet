//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.NotificationHubs.Auth;
using Microsoft.Azure.NotificationHubs.Messaging;
using static Microsoft.Azure.NotificationHubs.Messaging.MessagingExceptionDetail;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a namespace manager
    /// </summary>
    public sealed class NamespaceManager
    {
        private const string ApiVersion = "2017-04";
        private const string Header = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns = \"http://www.w3.org/2005/Atom\"><content type = \"application/xml\">";
        private const string Footer = "</content></entry>";
        private const string TrackingIdHeaderKey = "TrackingId";
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;
        private readonly TokenProvider _tokenProvider;
        private readonly NotificationHubRetryPolicy _retryPolicy;

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
        /// Creates an instance of the <see cref="NamespaceManager"/> class based on key-value configuration connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>An instance of the <see cref="NamespaceManager"/> class</returns>
        public static NamespaceManager CreateFromConnectionString(string connectionString)
        {
            return new NamespaceManager(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NamespaceManager"/>
        /// </summary>
        /// <param name="connectionString">Namespace connection string</param>
        public NamespaceManager(string connectionString) : this(connectionString, null)
        {
        }

        /// Initializes a new instance of <see cref="NamespaceManager"/> with settings
        /// <param name="connectionString">Namespace connection string</param>
        /// <param name="settings"> Namespace manager settings. </param>
        public NamespaceManager(string connectionString, NotificationHubSettings settings)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _tokenProvider = SharedAccessSignatureTokenProvider.CreateSharedAccessSignatureTokenProvider(connectionString);
            var configurationManager = new KeyValueConfigurationManager(connectionString);
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
        /// Creates a notification hub.
        /// </summary>
        /// <param name="hubName">The notification hub description name.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        public NotificationHubDescription CreateNotificationHub(string hubName) =>
            CreateNotificationHubAsync(hubName).GetAwaiter().GetResult();

        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        public NotificationHubDescription CreateNotificationHub(NotificationHubDescription description) =>
            CreateNotificationHubAsync(description).GetAwaiter().GetResult();

        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="hubName">The notification hub description name.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        public Task<NotificationHubDescription> CreateNotificationHubAsync(string hubName) =>
            CreateNotificationHubAsync(new NotificationHubDescription(hubName));

        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="hubName">The notification hub description name.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        public Task<NotificationHubDescription> CreateNotificationHubAsync(string hubName, CancellationToken cancellationToken) =>
            CreateNotificationHubAsync(new NotificationHubDescription(hubName), cancellationToken);

        /// <summary>
        /// Creates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>A task that represents the asynchronous create hub operation</returns>
        public Task<NotificationHubDescription> CreateNotificationHubAsync(NotificationHubDescription description)
        {
            return CreateOrUpdateNotificationHubAsync(description, false, CancellationToken.None);
        }

        /// <summary>
        /// Creates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous create hub operation</returns>
        public Task<NotificationHubDescription> CreateNotificationHubAsync(NotificationHubDescription description, CancellationToken cancellationToken)
        {
            return CreateOrUpdateNotificationHubAsync(description, false, cancellationToken);
        }

        /// <summary>
        /// Gets the notification hub.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>A notification hub description object.</returns>
        public NotificationHubDescription GetNotificationHub(string path) =>
            GetNotificationHubAsync(path).GetAwaiter().GetResult();

        /// <summary>
        /// Gets the notification hub asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get hub operation</returns>
        public Task<NotificationHubDescription> GetNotificationHubAsync(string path)
        {
            return GetNotificationHubAsync(path, CancellationToken.None);
        }

        /// <summary>
        /// Gets the notification hub asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get hub operation</returns>
        public async Task<NotificationHubDescription> GetNotificationHubAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var requestUri = new UriBuilder(_baseUri)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = path,
                Query = $"api-version={ApiVersion}"
            };

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var response = await SendAsync(() =>
                {
                    var httpRequestMessage = CreateHttpRequest(HttpMethod.Get, requestUri.Uri);

                    return httpRequestMessage;
                }, ct).ConfigureAwait(false))
                {
                    var trackingId = string.Empty;
                    if (response.Headers.TryGetValues(TrackingIdHeaderKey, out var values))
                    {
                        trackingId = values.FirstOrDefault();
                    }
                    var xmlResponse = await GetXmlContent(response, trackingId).ConfigureAwait(false);
                    if (xmlResponse.NodeType != XmlNodeType.None)
                    {
                        var model = GetModelFromResponse<NotificationHubDescription>(xmlResponse, trackingId);
                        model.Path = path;
                        return model;
                    }
                    else
                    {
                        throw new MessagingEntityNotFoundException(new MessagingExceptionDetail(ExceptionErrorCodes.ConflictGeneric, "Notification Hub not found", ErrorLevelType.UserError, response.StatusCode, trackingId));
                    }
                };
            }, cancellationToken);
        }

        /// <summary>
        /// Gets the notification hubs.
        /// </summary>
        /// <returns>A collection of notification hubs</returns>
        public IEnumerable<NotificationHubDescription> GetNotificationHubs() =>
            GetNotificationHubsAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Gets the notification hubs asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous get hubs operation</returns>
        public Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync()
        {
            return GetNotificationHubsAsync(CancellationToken.None);
        }

        /// <summary>
        /// Gets the notification hubs asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get hubs operation</returns>
        public async Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync(CancellationToken cancellationToken)
        {
            var requestUri = new UriBuilder(_baseUri)
            {
                Scheme = Uri.UriSchemeHttps
            };

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var response = await SendAsync(() =>
                {
                    var httpRequestMessage = CreateHttpRequest(HttpMethod.Get, requestUri.Uri);

                    return httpRequestMessage;
                }, ct).ConfigureAwait(false))
                {
                    var result = new List<NotificationHubDescription>();

                    using (var xmlReader = XmlReader.Create(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), new XmlReaderSettings { Async = true }))
                    {
                        // Advancing to the first element skipping non-content nodes
                        await xmlReader.MoveToContentAsync().ConfigureAwait(false);

                        if (!xmlReader.IsStartElement("feed"))
                        {
                            throw new FormatException("Required 'feed' element is missing");
                        }

                        while (xmlReader.ReadToFollowing("entry"))
                        {
                            if (xmlReader.ReadToDescendant("title"))
                            {
                                xmlReader.ReadStartElement();
                                var hubName = xmlReader.Value;

                                result.Add(await GetNotificationHubAsync(hubName, cancellationToken).ConfigureAwait(false));
                            }
                        }
                    }

                    return result;
                };
            }, cancellationToken);
        }

        /// <summary>
        /// Delete the notification hub.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        public void DeleteNotificationHub(string path) =>
            DeleteNotificationHubAsync(path).GetAwaiter().GetResult();

        /// <summary>
        /// Delete the notification hub.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        public Task DeleteNotificationHubAsync(string path)
        {
            return DeleteNotificationHubAsync(path, CancellationToken.None);
        }

        /// <summary>
        /// Delete the notification hub.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <param name="path">The notification hub path.</param>
        public async Task DeleteNotificationHubAsync(string path, CancellationToken cancellationToken)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var requestUri = new UriBuilder(_baseUri)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = path,
                Query = $"api-version={ApiVersion}"
            };

            await _retryPolicy.RunOperation(async (ct) =>
            {
                return await SendAsync(() =>
                {
                    var httpRequestMessage = CreateHttpRequest(HttpMethod.Delete, requestUri.Uri);

                    return httpRequestMessage;
                }, ct).ConfigureAwait(false);
            }, cancellationToken);
        }

        /// <summary>
        /// Updates the notification hub.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>The updated hub object</returns>
        public NotificationHubDescription UpdateNotificationHub(NotificationHubDescription description) =>
            UpdateNotificationHubAsync(description).GetAwaiter().GetResult();

        /// <summary>
        /// Updates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>A task that represents the asynchronous hub update operation</returns>
        public Task<NotificationHubDescription> UpdateNotificationHubAsync(NotificationHubDescription description)
        {
            return UpdateNotificationHubAsync(description, CancellationToken.None);
        }

        /// <summary>
        /// Updates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous hub update operation</returns>
        public Task<NotificationHubDescription> UpdateNotificationHubAsync(NotificationHubDescription description, CancellationToken cancellationToken)
        {
            return CreateOrUpdateNotificationHubAsync(description, true, cancellationToken);
        }

        /// <summary>Checks whether a notifications hub exists.</summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>True if the hub exists</returns>
        public bool NotificationHubExists(string path) =>
            NotificationHubExistsAsync(path).GetAwaiter().GetResult();

        /// <summary>
        /// Checks whether a notification hub exists asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous hub check operation</returns>
        public Task<bool> NotificationHubExistsAsync(string path)
        {
            return NotificationHubExistsAsync(path, CancellationToken.None);
        }

        /// <summary>
        /// Checks whether a notification hub exists asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous hub check operation</returns>
        public async Task<bool> NotificationHubExistsAsync(string path, CancellationToken cancellationToken)
        {
            try
            {
                var hubDescription = await GetNotificationHubAsync(path, cancellationToken).ConfigureAwait(false);
                return String.Equals(hubDescription.Path, path, StringComparison.OrdinalIgnoreCase);
            }
            catch (MessagingEntityNotFoundException)
            {
                return false;
            }
        }

        private async Task<NotificationHubDescription> CreateOrUpdateNotificationHubAsync(NotificationHubDescription description, bool update, CancellationToken cancellationToken)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            var requestUri = new UriBuilder(_baseUri)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = description.Path,
                Query = $"api-version={ApiVersion}"
            };
            var xmlBody = CreateRequestBody(description);

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var response = await SendAsync(() =>
                {
                    var httpRequestMessage = CreateHttpRequest(HttpMethod.Put, requestUri.Uri);
                    httpRequestMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(xmlBody)));

                    if (update)
                    {
                        httpRequestMessage.Headers.Add("If-Match", "*");
                    }

                    return httpRequestMessage;
                }, ct).ConfigureAwait(false))
                {
                    var trackingId = string.Empty;
                    if (response.Headers.TryGetValues(TrackingIdHeaderKey, out var values))
                    {
                        trackingId = values.FirstOrDefault();
                    }
                    var xmlResponse = await GetXmlContent(response, trackingId).ConfigureAwait(false);
                    var model = GetModelFromResponse<NotificationHubDescription>(xmlResponse, trackingId);
                    model.Path = description.Path;
                    return model;
                };
            }, cancellationToken);
        }

        /// <summary>Submits the notification hub job asynchronously.</summary>
        /// <param name="job">The job to submit.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        public Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, string notificationHubPath)
        {
            return SubmitNotificationHubJobAsync(job, notificationHubPath, CancellationToken.None);
        }

        /// <summary>Submits the notification hub job asynchronously.</summary>
        /// <param name="job">The job to submit.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>\
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        public async Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, string notificationHubPath, CancellationToken cancellationToken)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (job.OutputContainerUri == null)
            {
                throw new ArgumentNullException(nameof(job.OutputContainerUri));
            }

            var requestUri = new UriBuilder(_baseUri)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = $"{notificationHubPath}/jobs",
                Query = $"api-version={ApiVersion}"
            };
            var xmlBody = CreateRequestBody(job);

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var response = await SendAsync(() =>
                 {
                     var httpRequestMessage = CreateHttpRequest(HttpMethod.Post, requestUri.Uri);
                     httpRequestMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(xmlBody)));

                     return httpRequestMessage;
                 }, ct).ConfigureAwait(false))
                {
                    var trackingId = string.Empty;
                    if (response.Headers.TryGetValues(TrackingIdHeaderKey, out var values))
                    {
                        trackingId = values.FirstOrDefault();
                    }
                    var xmlResponse = await GetXmlContent(response, trackingId).ConfigureAwait(false);
                    return GetModelFromResponse<NotificationHubJob>(xmlResponse, trackingId);
                };
            }, cancellationToken);
        }

        /// <summary>Gets the notification hub job asynchronously.</summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        public Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, string notificationHubPath)
        {
            return GetNotificationHubJobAsync(jobId, notificationHubPath, CancellationToken.None);
        }

        /// <summary>Gets the notification hub job asynchronously.</summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        public async Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, string notificationHubPath, CancellationToken cancellationToken)
        {
            var requestUri = new UriBuilder(_baseUri)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = $"{notificationHubPath}/jobs/{jobId}",
                Query = $"api-version={ApiVersion}"
            };

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var response = await SendAsync(() =>
                {
                    var httpRequestMessage = CreateHttpRequest(HttpMethod.Get, requestUri.Uri);

                    return httpRequestMessage;
                }, ct).ConfigureAwait(false))
                {
                    var trackingId = string.Empty;
                    if (response.Headers.TryGetValues(TrackingIdHeaderKey, out var values))
                    {
                        trackingId = values.FirstOrDefault();
                    }
                    var xmlResponse = await GetXmlContent(response, trackingId).ConfigureAwait(false);
                    return GetModelFromResponse<NotificationHubJob>(xmlResponse, trackingId);
                };
            }, cancellationToken);
        }


        /// <summary>Gets the notification hub jobs asynchronously.</summary>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get jobs operation</returns>
        public Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(string notificationHubPath)
        {
            return GetNotificationHubJobsAsync(notificationHubPath, CancellationToken.None);
        }

        /// <summary>Gets the notification hub jobs asynchronously.</summary>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A task that represents the asynchronous get jobs operation</returns>
        public async Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(string notificationHubPath, CancellationToken cancellationToken)
        {
            var requestUri = new UriBuilder(_baseUri)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = $"{notificationHubPath}/jobs",
                Query = $"api-version={ApiVersion}"
            };

            return await _retryPolicy.RunOperation(async (ct) =>
            {
                using (var response = await SendAsync(() => CreateHttpRequest(HttpMethod.Get, requestUri.Uri), ct).ConfigureAwait(false))
                {
                    var trackingId = string.Empty;
                    if (response.Headers.TryGetValues(TrackingIdHeaderKey, out var values))
                    {
                        trackingId = values.FirstOrDefault();
                    }
                    var result = new List<NotificationHubJob>();
                    using (var xmlReader = XmlReader.Create(await response.Content.ReadAsStreamAsync().ConfigureAwait(false), new XmlReaderSettings { Async = true }))
                    {
                        await xmlReader.MoveToContentAsync().ConfigureAwait(false);

                        if (!xmlReader.IsStartElement("feed"))
                        {
                            throw new FormatException("Required 'feed' element is missing");
                        }

                        while (xmlReader.ReadToFollowing("entry"))
                        {
                            if (xmlReader.ReadToDescendant("content"))
                            {
                                xmlReader.ReadStartElement();
                                result.Add(GetModelFromResponse<NotificationHubJob>(xmlReader, trackingId));
                            }
                        }
                    }

                    return result;
                }
            }, cancellationToken);
        }

        private static Uri GetBaseUri(KeyValueConfigurationManager manager)
        {
            var endpointString = manager.connectionProperties[KeyValueConfigurationManager.EndpointConfigName];
            return new Uri(endpointString);
        }

        private void SetUserAgent()
        {
            if (!_httpClient.DefaultRequestHeaders.Contains(Constants.HttpUserAgentHeaderName))
            {
                _httpClient.DefaultRequestHeaders.Add(Constants.HttpUserAgentHeaderName,
                    $"NHub/{ManagementStrings.ApiVersion} (api-origin=DotNetSdk;os={Environment.OSVersion.Platform};os-version={Environment.OSVersion.Version})");
            }
        }

        private static string AddHeaderAndFooterToXml(string content) => $"{Header}{content}{Footer}";

        private static string SerializeObject<T>(T model)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                serializer.WriteObject(xmlWriter, model);
            }

            return stringBuilder.ToString();
        }

        private static string CreateRequestBody<T>(T model)
        {
            return AddHeaderAndFooterToXml(SerializeObject(model));
        }

        private static async Task<XmlReader> GetXmlContent(HttpResponseMessage response, string trackingId)
        {
            try
            {
                if (response.Content == null)
                {
                    return XmlReader.Create(new StringReader(string.Empty));
                }

                var xmlReader = XmlReader.Create(await response.Content.ReadAsStreamAsync());
                if (xmlReader.ReadToFollowing("entry"))
                {
                    if (xmlReader.ReadToDescendant("content"))
                    {
                        xmlReader.ReadStartElement();
                    }
                }

                return xmlReader;
            }
            catch (XmlException ex)
            {
                throw ExceptionsUtility.HandleXmlException(ex, trackingId);
            }
        }

        private static T GetModelFromResponse<T>(XmlReader xmlReader, string trackingId) where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));
            try
            {
                using (xmlReader)
                {
                    return (T)serializer.ReadObject(xmlReader);
                }
            } 
            catch (SerializationException ex) when (ex.InnerException is XmlException xmlException)
            {
                throw ExceptionsUtility.HandleXmlException(xmlException, trackingId);
            }
        }

        private string CreateToken(Uri uri)
        {
            return _tokenProvider.GetToken(uri.ToString());
        }

        private HttpRequestMessage CreateHttpRequest(HttpMethod method, Uri uri)
        {
            var httpRequestMessage = new HttpRequestMessage(method, uri);

            httpRequestMessage.Headers.Add("Authorization", CreateToken(uri));
            httpRequestMessage.Headers.Add("x-ms-version", ApiVersion);

            return httpRequestMessage;
        }

        private async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> generateHttpRequestMessage, CancellationToken cancellationToken)
        {
            var trackingId = Guid.NewGuid().ToString();

            var httpRequestMessage = generateHttpRequestMessage();
            httpRequestMessage.Headers.Add(TrackingIdHeaderKey, trackingId);

            try
            {
                var response = await _httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
                response.Headers.Add(TrackingIdHeaderKey, trackingId);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    throw await response.TranslateToMessagingExceptionAsync(trackingId).ConfigureAwait(false);
                }
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
    }
}
