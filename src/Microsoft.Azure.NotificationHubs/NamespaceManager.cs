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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.NotificationHubs.Auth;
using Microsoft.Azure.NotificationHubs.Messaging;

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
        private readonly NamespaceManagerSettings _settings;
        private readonly IEnumerable<Uri> _addresses;
        
        /// <summary> Gets the first namespace base address. </summary>
        /// <value> The namespace base address. </value>
        public Uri Address => _addresses.First();

        /// <summary> Gets the namespace manager settings. </summary>
        /// <value> The namespace manager settings. </value>
        public NamespaceManagerSettings Settings => _settings;

        /// <summary>
        /// Creates an instance of the <see cref="NamespaceManager"/> class based on key-value configuration connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>An instance of the <see cref="NamespaceManager"/> class</returns>
        public static NamespaceManager CreateFromConnectionString(string connectionString)
        {
            KeyValueConfigurationManager manager = new KeyValueConfigurationManager(connectionString);
            return manager.CreateNamespaceManager();
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address to access your namespace. Anonymous credentials are assumed.</summary>
        /// <param name="address">The full address of the namespace.</param>
        /// <exception cref="ArgumentNullException">Thrown when address is null. </exception>
        public NamespaceManager(string address)
            : this(new Uri(address), (TokenProvider)null)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base addresses to access your namespace. Anonymous credentials are assumed.</summary>
        /// <param name="addresses">The full addresses of the namespace.</param>
        /// <exception cref="ArgumentNullException">Thrown when addresses field is null. </exception>
        /// <exception cref="ArgumentException"> Thrown when addresses list is null or empty. </exception>
        /// <exception cref="UriFormatException"> Thrown when address is not correctly formed. </exception>
        public NamespaceManager(IList<string> addresses)
            : this(addresses, (TokenProvider)null)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address to access your namespace. Anonymous credentials are assumed.</summary>
        /// <param name="address">The full address of the namespace.</param>
        /// <exception cref="ArgumentNullException">Thrown when address is null. </exception>
        public NamespaceManager(Uri address)
            : this(address, (TokenProvider)null)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address to access your namespace. Anonymous credentials are assumed.</summary>
        /// <param name="addresses">The full addresses of the namespace.</param>
        /// <exception cref="ArgumentNullException">Thrown when addresses field is null. </exception>
        /// <exception cref="ArgumentException"> Thrown when addresses list is null or empty. </exception>
        public NamespaceManager(IEnumerable<Uri> addresses)
            : this(addresses, (TokenProvider)null)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address and proper credentials to access your namespace. </summary>
        /// <param name="address">The full address of the namespace.</param>
        /// <param name="tokenProvider"> The namespace access credentials. </param>
        /// <remarks>Even though it is not allowed to include paths in the namespace address, you can specify a credential that authorizes you to perform actions only on
        ///           some sublevels off of the base address.</remarks>
        /// <exception cref="ArgumentNullException"> Thrown when address is null. </exception>
        public NamespaceManager(string address, TokenProvider tokenProvider)
            : this(new Uri(address), tokenProvider)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address and proper credentials to access your namespace. </summary>
        /// <param name="addresses">The full addresses of the namespace.</param>
        /// <param name="tokenProvider"> The namespace access credentials. </param>
        /// <remarks>Even though it is not allowed to include paths in the namespace addresses, you can specify a credential that authorizes you to perform actions only on
        ///           some sublevels off of the base addresses.</remarks>
        /// <exception cref="ArgumentNullException"> Thrown when addresses field is null. </exception>
        /// <exception cref="ArgumentException"> Thrown when addresses list is null or empty. </exception>
        /// <exception cref="UriFormatException"> Thrown when address is not correctly formed. </exception>

        public NamespaceManager(IList<string> addresses, TokenProvider tokenProvider)
            : this(MessagingUtilities.GetUriList(addresses), tokenProvider)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address and proper credentials to access your namespace. </summary>
        /// <param name="address">The full address of the namespace. </param>
        /// <param name="tokenProvider">A TokenProvider for the namespace </param>
        /// <remarks>Even though it is not allowed to include paths in the namespace address, you can specify a credential that authorizes you to perform actions only on
        ///           some sublevels off of the base address, i.e. it is not a must that the credentials you specify be to the base adress itself</remarks>
        /// <exception cref="ArgumentNullException"> Thrown if address is null.  </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the type is not a supported Credential type. 
        ///                                      See <see cref="NamespaceManagerSettings.TokenProvider "/> to know more about the supported types.</exception>
        public NamespaceManager(Uri address, TokenProvider tokenProvider)
        {
            MessagingUtilities.ThrowIfNullAddressOrPathExists(address);

            _addresses = new List<Uri> { address };
            _settings = new NamespaceManagerSettings()
            {
                TokenProvider = tokenProvider,
            };
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address and proper credentials to access your namespace. </summary>
        /// <param name="addresses">The full address of the namespace. </param>
        /// <param name="tokenProvider">A TokenProvider for the namespace </param>
        /// <remarks>Even though it is not allowed to include paths in the namespace addresses, you can specify a credential that authorizes you to perform actions only on
        ///           some sublevels off of the base addresses, i.e. it is not a must that the credentials you specify be to the base adresses itself</remarks>
        /// <exception cref="ArgumentNullException"> Thrown if addresses is null.  </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the type is not a supported Credential type. 
        ///                                      See <see cref="NamespaceManagerSettings.TokenProvider "/> to know more about the supported types.</exception>
        /// <exception cref="ArgumentException"> Thrown when addresses list is null or empty. </exception>
        public NamespaceManager(IEnumerable<Uri> addresses, TokenProvider tokenProvider)
        {
            MessagingUtilities.ThrowIfNullAddressesOrPathExists(addresses);

            _addresses = addresses.ToList();
            _settings = new NamespaceManagerSettings()
            {
                TokenProvider = tokenProvider,
            };
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address and proper credentials to access your namespace. </summary>
        /// <param name="address"> The full address of the namespace.  </param>
        /// <param name="settings"> Contains the ServiceBusCredential as well as an OperationTimeout property.</param>
        /// <remarks>Even though it is not allowed to include paths in the namespace address, you can specify a credential that authorizes you to perform actions only on
        ///           some sublevels off of the base address, i.e. it is not a must that the credentials you specify be to the base adress itself</remarks>
        /// <exception cref="ArgumentNullException"> Thrown when address or settings is null. </exception>
        public NamespaceManager(string address, NamespaceManagerSettings settings)
            : this(new Uri(address), settings)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address and proper credentials to access your namespace. </summary>
        /// <param name="addresses"> The full addresses of the namespace.  </param>
        /// <param name="settings"> Contains the ServiceBusCredential as well as an OperationTimeout property.</param>
        /// <remarks>Even though it is not allowed to include paths in the namespace address, you can specify a credential that authorizes you to perform actions only on
        ///           some sublevels off of the base addresses, i.e. it is not a must that the credentials you specify be to the base adresses itself</remarks>
        /// <exception cref="ArgumentNullException"> Thrown when address or settings is null. </exception>
        /// <exception cref="ArgumentException"> Thrown when addresses list is null or empty. </exception>
        /// <exception cref="UriFormatException"> Thrown when address is not correctly formed. </exception>
        public NamespaceManager(IList<string> addresses, NamespaceManagerSettings settings)
            : this(MessagingUtilities.GetUriList(addresses), settings)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address and proper credentials to access your namespace. </summary>
        /// <param name="address"> The full address of the namespace.  </param>
        /// <param name="settings"> Contains the ServiceBusCredential as well as an OperationTimeout property.</param>
        /// <remarks>Even though it is not allowed to include paths in the namespace address, you can specify a credential that authorizes you to perform actions only on
        ///           some sublevels off of the base address, i.e. it is not a must that the credentials you specify be to the base adress itself</remarks>
        /// <exception cref="ArgumentNullException"> Thrown when address or settings is null. </exception>
        public NamespaceManager(Uri address, NamespaceManagerSettings settings)
            : this(new List<Uri>() { address }, settings)
        {
        }

        /// <summary> ServiceBusNamespaceClient Constructor. You must supply your base address and proper credentials to access your namespace. </summary>
        /// <param name="addresses"> The full address of the namespace.  </param>
        /// <param name="settings"> Contains the ServiceBusCredential as well as an OperationTimeout property.</param>
        /// <remarks>Even though it is not allowed to include paths in the namespace addresses, you can specify a credential that authorizes you to perform actions only on
        ///           some sublevels off of the base addresses, i.e. it is not a must that the credentials you specify be to the base adresses itself</remarks>
        /// <exception cref="ArgumentNullException"> Thrown when addresses or settings is null. </exception>
        /// <exception cref="ArgumentException"> Thrown when addresses list is null or empty. </exception>
        public NamespaceManager(IEnumerable<Uri> addresses, NamespaceManagerSettings settings)
        {
            MessagingUtilities.ThrowIfNullAddressesOrPathExists(addresses);

            _addresses = addresses.ToList();
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Gets the version information.
        /// </summary>
        /// <returns>The version information</returns>
        public string GetVersionInfo() => 
            GetVersionInfoAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Gets the version information asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous version information extraction operation</returns>
        public async Task<string> GetVersionInfoAsync()
        {
            var requestUri = new UriBuilder(Address)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Path = "/$protocol-version"
                };

            using(var response = await SendAsync(() => 
            {
                var httpRequestMessage = CreateHttpRequest(HttpMethod.Get, requestUri.Uri);
                httpRequestMessage.Headers.Add("X-PROCESS-AT", "ServiceBus");

                return httpRequestMessage;
            }).ConfigureAwait(false))
            {
                if (response.Headers.TryGetValues("MaxProtocolVersion", out var values)) {
                    return values.FirstOrDefault();
                }

                return string.Empty;    
            };
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
        /// Creates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>A task that represents the asynchronous create hub operation</returns>
        public Task<NotificationHubDescription> CreateNotificationHubAsync(NotificationHubDescription description)
        {
            return CreateOrUpdateNotificationHubAsync(description, false);
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
        public async Task<NotificationHubDescription> GetNotificationHubAsync(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var requestUri = new UriBuilder(Address)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Path = path,  
                    Query = $"?api-version={ApiVersion}"
                };

            using(var response = await SendAsync(() => 
            {
                var httpRequestMessage = CreateHttpRequest(HttpMethod.Get, requestUri.Uri);

                return httpRequestMessage;
            }).ConfigureAwait(false))
            {
                var xmlResponse = await GetXmlContent(response).ConfigureAwait(false);
                if (xmlResponse.NodeType != XmlNodeType.None)
                {
                    var model = GetModelFromResponse<NotificationHubDescription>(xmlResponse);
                    model.Path = path;
                    return model;
                }
                else
                {
                    throw new MessagingEntityNotFoundException("Notification Hub not found");
                }    
            };
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
        public async Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync()
        {
            var requestUri = new UriBuilder(Address)
            {
                Scheme = Uri.UriSchemeHttps
            };

            using(var response = await SendAsync(() => 
            {
                var httpRequestMessage = CreateHttpRequest(HttpMethod.Get, requestUri.Uri);

                return httpRequestMessage;
            }).ConfigureAwait(false))
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
                            
                            result.Add(await GetNotificationHubAsync(hubName).ConfigureAwait(false));
                        }
                    }
                }

                return result;    
            };
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
        public async Task DeleteNotificationHubAsync(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var requestUri = new UriBuilder(Address)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = path,
                Query = $"?api-version={ApiVersion}"
            };

            await SendAsync(() => 
            {
                var httpRequestMessage = CreateHttpRequest(HttpMethod.Delete, requestUri.Uri);

                return httpRequestMessage; 
            }).ConfigureAwait(false);
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
            return CreateOrUpdateNotificationHubAsync(description, true);
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
        public async Task<bool> NotificationHubExistsAsync(string path)
        {
            try
            {
                var hubDescription = await GetNotificationHubAsync(path).ConfigureAwait(false);
                return String.Equals(hubDescription.Path, path, StringComparison.OrdinalIgnoreCase);
            }
            catch (MessagingEntityNotFoundException)
            {
                return false;
            }
        }

        private async Task<NotificationHubDescription> CreateOrUpdateNotificationHubAsync(NotificationHubDescription description, bool update)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }       

            var requestUri = new UriBuilder(Address)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = description.Path,
                Query = $"api-version={ApiVersion}"
            };
            var xmlBody = CreateRequestBody(description);

            using(var response = await SendAsync(() => 
            {
                var httpRequestMessage = CreateHttpRequest(HttpMethod.Put, requestUri.Uri);
                httpRequestMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(xmlBody)));

                if (update) 
                {
                    httpRequestMessage.Headers.Add("If-Match", "*");        
                }

                return httpRequestMessage;
            }).ConfigureAwait(false))
            {
                var xmlResponse = await GetXmlContent(response).ConfigureAwait(false);
                var model = GetModelFromResponse<NotificationHubDescription>(xmlResponse);
                model.Path = description.Path;
                return model;    
            };
        }

        /// <summary>Submits the notification hub job asynchronously.</summary>
        /// <param name="job">The job to submit.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        public async Task<NotificationHubJob> SubmitNotificationHubJobAsync(NotificationHubJob job, string notificationHubPath)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (job.OutputContainerUri == null)
            {
                throw new ArgumentNullException(nameof(job.OutputContainerUri));
            }

            var requestUri = new UriBuilder(Address)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = $"{notificationHubPath}/jobs",
                Query = $"?api-version={ApiVersion}"
            };  
            var xmlBody = CreateRequestBody(job);

            using(var response = await SendAsync(() => 
            {
                var httpRequestMessage = CreateHttpRequest(HttpMethod.Post, requestUri.Uri);                                            
                httpRequestMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(xmlBody)));

                return httpRequestMessage;      
            }).ConfigureAwait(false))
            {
                var xmlResponse = await GetXmlContent(response).ConfigureAwait(false);
                return GetModelFromResponse<NotificationHubJob>(xmlResponse);
            };
        }

        /// <summary>Gets the notification hub job asynchronously.</summary>
        /// <param name="jobId">The job identifier.</param>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get job operation</returns>
        public async Task<NotificationHubJob> GetNotificationHubJobAsync(string jobId, string notificationHubPath)
        {
            var requestUri = new UriBuilder(Address)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = $"{notificationHubPath}/jobs/{jobId}",
                Query = $"api-version={ApiVersion}"
            };

            using(var response = await SendAsync(() => 
            {
                var httpRequestMessage = CreateHttpRequest(HttpMethod.Get, requestUri.Uri);

                return httpRequestMessage;
            }).ConfigureAwait(false))
            {
                var xmlResponse = await GetXmlContent(response).ConfigureAwait(false);
                return GetModelFromResponse<NotificationHubJob>(xmlResponse);    
            };
        }

        /// <summary>Gets the notification hub jobs asynchronously.</summary>
        /// <param name="notificationHubPath">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get jobs operation</returns>
        public async Task<IEnumerable<NotificationHubJob>> GetNotificationHubJobsAsync(string notificationHubPath)
        {
            var requestUri = new UriBuilder(Address)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = $"{notificationHubPath}/jobs",
                Query = $"?api-version={ApiVersion}"
            };

            using (var response = await SendAsync(() => CreateHttpRequest(HttpMethod.Get, requestUri.Uri)).ConfigureAwait(false))
            {
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
                            result.Add(GetModelFromResponse<NotificationHubJob>(xmlReader));
                        }
                    }
                }

                return result;
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

        private static async Task<XmlReader> GetXmlContent(HttpResponseMessage response)
        {
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

        private static async Task<XmlReader> GetXmlError(HttpResponseMessage response) => 
            XmlReader.Create(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));

        private static T GetModelFromResponse<T>(XmlReader xmlReader) where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));

            using (xmlReader)
            {
                return (T)serializer.ReadObject(xmlReader);
            }
        }

        private string CreateToken(Uri uri)
        {
            return _settings.TokenProvider.GetToken(uri.ToString());
        }

        private HttpRequestMessage CreateHttpRequest(HttpMethod method, Uri uri)
        {
            var httpRequestMessage = new HttpRequestMessage(method, uri);

            httpRequestMessage.Headers.Add("Authorization", CreateToken(uri));
            httpRequestMessage.Headers.Add("x-ms-version", ApiVersion);

            return httpRequestMessage;
        }
        private async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> generateHttpRequestMessage) 
        {
            using (var client = new HttpClient())
            {
                var response = await _settings.RetryPolicy
                    .ExecuteAsync(async () => 
                    {
                        var trackingId = Guid.NewGuid().ToString();

                        var httpRequestMessage = generateHttpRequestMessage(); 
                        httpRequestMessage.Headers.Add(TrackingIdHeaderKey, trackingId);   

                        var httpResponseMessage = await client.SendAsync(httpRequestMessage).ConfigureAwait(false);
                        httpResponseMessage.Headers.Add(TrackingIdHeaderKey, trackingId);

                        return httpResponseMessage;
                    })
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    var xmlError = await GetXmlError(response).ConfigureAwait(false);
                    var error = GetModelFromResponse<ErrorResponse>(xmlError);
                    var trackingId = string.Empty;

                    if (response.Headers.TryGetValues(TrackingIdHeaderKey, out var values))
                    {
                        trackingId = values.FirstOrDefault();
                    }
                    
                    var innerException = new WebException($"The remote server returned an error: {error.Code}\nTrackingId: {trackingId}");

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            throw new MessagingEntityNotFoundException(error.Detail, innerException);
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedAccessException(error.Detail, innerException);
                        case HttpStatusCode.BadRequest:
                            throw new MessagingCommunicationException(error.Detail, innerException);
                        case HttpStatusCode.Conflict:
                            throw new MessagingEntityAlreadyExistsException(error.Detail, innerException);
                        default:
                            throw new Exception(error.Detail, innerException);
                    }
                }
            }    
        }
    }
}
