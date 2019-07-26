using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.NotificationHubs.Auth;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    public sealed class NamespaceManager
    {
        private readonly NamespaceManagerSettings settings;
        private readonly IEnumerable<Uri> addresses;

        /// <summary> Gets the first namespace base address. </summary>
        /// <value> The namespace base address. </value>
        public Uri Address => addresses.First();

        /// <summary> Gets the namespace manager settings. </summary>
        /// <value> The namespace manager settings. </value>
        public NamespaceManagerSettings Settings => settings;

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
            MessagingUtilities.ThrowIfNullAddressOrPathExists(address, "address");

            this.addresses = new List<Uri> { address };
            this.settings = new NamespaceManagerSettings()
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
            MessagingUtilities.ThrowIfNullAddressesOrPathExists(addresses, "addresses");

            this.addresses = addresses.ToList();
            this.settings = new NamespaceManagerSettings()
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
        ////public NamespaceManager(IEnumerable<string> addresses, NamespaceManagerSettings settings)
        ////    : this(MessagingUtilities.GetUriList(addresses), settings)
        ////{
        ////}

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
            MessagingUtilities.ThrowIfNullAddressesOrPathExists(addresses, "addresses");

            this.addresses = addresses.ToList();
            this.settings = this.settings ?? throw new ArgumentNullException("settings");
        }
    }
}
