//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.NotificationHubs.Auth;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    public sealed class NamespaceManager
    {
        const string ApiVersion = "2017-04";

        readonly NamespaceManagerSettings _settings;
        readonly IEnumerable<Uri> _addresses;
        
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
        public NamespaceManager(IEnumerable<string> addresses)
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

        public NamespaceManager(IEnumerable<string> addresses, TokenProvider tokenProvider)
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
        public NamespaceManager(IEnumerable<string> addresses, NamespaceManagerSettings settings)
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
        /// Creates a notification hub.
        /// </summary>
        /// <param name="hubName">The notification hub description name.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        public NotificationHubDescription CreateNotificationHub(string hubName)
        {
            return CreateNotificationHubAsync(hubName).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        public NotificationHubDescription CreateNotificationHub(NotificationHubDescription description)
        {
            return CreateNotificationHubAsync(description).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a notification hub.
        /// </summary>
        /// <param name="hubName">The notification hub description name.</param>
        /// <returns>An instance of the <see cref="NotificationHubDescription"/> class</returns>
        public async Task<NotificationHubDescription> CreateNotificationHubAsync(string hubName)
        {
            return await CreateNotificationHubAsync(new NotificationHubDescription(hubName));
        }

        /// <summary>
        /// Creates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>A task that represents the asynchronous create hub operation</returns>
        public async Task<NotificationHubDescription> CreateNotificationHubAsync(NotificationHubDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            var client = new HttpClient();

            var xmlRequest = SerializeObject(description);
            var xmlBody = AddHeaderAndFooterToXml(xmlRequest);

            var uriBuilder = new UriBuilder(Address)
            {
                Scheme = Uri.UriSchemeHttps,
                Path = description.Path,
                Query = $"?api-version={ApiVersion}"
            };

            var token = _settings.TokenProvider.GetToken(uriBuilder.Uri.ToString());
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, uriBuilder.Uri);

            httpRequestMessage.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(xmlBody)));
            httpRequestMessage.Headers.Add("Authorization", token);
            httpRequestMessage.Headers.Add("Content-Type", "application/xml;type=entry;charset=utf-8");
            httpRequestMessage.Headers.Add("x-ms-version", ApiVersion);

            var response = await _settings.RetryPolicy
                .ExecuteAsync(() => client.SendAsync(httpRequestMessage));

            var xmlResponse = await GetXmlContent(response);

            return GetModelFromResponse<NotificationHubDescription>(xmlResponse);
        }

        /// <summary>
        /// Gets the notification hub.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>A notification hub description object.</returns>
        public NotificationHubDescription GetNotificationHub(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the notification hub asynchronously.
        /// </summary>
        /// <param name="path">The notification hub path.</param>
        /// <returns>A task that represents the asynchronous get hub operation</returns>
        public Task<NotificationHubDescription> GetNotificationHubAsync(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the notification hubs.
        /// </summary>
        /// <returns>A collection of notification hubs</returns>
        public IEnumerable<NotificationHubDescription> GetNotificationHubs()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the notification hubs asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous get hubs operation</returns>
        public Task<IEnumerable<NotificationHubDescription>> GetNotificationHubsAsync()
        {
            throw new NotImplementedException();
        }

        public void DeleteNotificationHub(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the notification hub.
        /// </summary>
        /// <param name="description">The notification hub description.</param>
        /// <returns>The updated hub object</returns>
        public NotificationHubDescription UpdateNotificationHub(NotificationHubDescription description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the notification hub asynchronously.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>A task that represents the asynchronous hub update operation</returns>
        public Task<NotificationHubDescription> UpdateNotificationHubAsync(NotificationHubDescription description)
        {
            throw new NotImplementedException();
        }

        private string AddHeaderAndFooterToXml(string content)
        {
            var header = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns = \"http://www.w3.org/2005/Atom\"><content type = \"application/xml\">";
            var footer = "</content></entry>";

            return $"{header}{content}{footer}";
        }

        private string SerializeObject<T>(T model)
        {
            var serializer = new DataContractSerializer(typeof(T));

            var stringBuilder = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                serializer.WriteObject(xmlWriter, model);
            }

            return stringBuilder.ToString();
        }

        private async Task<XmlReader> GetXmlContent(HttpResponseMessage response)
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

        private T GetModelFromResponse<T>(XmlReader xmlReader) where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(xmlReader);
        }
    }
}
