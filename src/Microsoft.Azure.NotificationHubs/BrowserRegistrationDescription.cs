//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    [DataContract(Name = ManagementStrings.BrowserRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class BrowserRegistrationDescription : RegistrationDescription
    {
        private const string PlatformName = "browser";
        private BrowserPushSubscription _browserPushSubscription;

        /// <summary>
        /// Browser push endpoint from PNS.
        /// </summary>
        [DataMember(Name = ManagementStrings.BrowserEndpoint, Order = 2001, IsRequired = true)]
        public string Endpoint
        {
            get
            {
                return _browserPushSubscription?.Endpoint;
            }
            set
            {
                if (_browserPushSubscription != null)
                {
                    _browserPushSubscription.Endpoint = value;
                }
            }
        }

        /// <summary>
        /// Browser push P256DH key from PNS.
        /// </summary>
        [DataMember(Name = ManagementStrings.BrowserP256DH, Order = 2002, IsRequired = true)]
        public string P256DH
        {
            get
            {
                return _browserPushSubscription?.P256DH;
            }
            set
            {
                if (_browserPushSubscription != null)
                {
                    _browserPushSubscription.P256DH = value;
                }
            }
        }

        /// <summary>
        /// Browser push authentication secret from PNS.
        /// </summary>
        [DataMember(Name = ManagementStrings.BrowserAuth, Order = 2003, IsRequired = true)]
        public string Auth
        {
            get
            {
                return _browserPushSubscription?.Auth;
            }
            set
            {
                if (_browserPushSubscription != null)
                {
                    _browserPushSubscription.Auth = value;
                }
            }
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.BrowserRegistrationDescription"/> class copying fields from another instance.
        /// </summary>
        /// <param name="sourceRegistration">Another <see cref="T:Microsoft.Azure.NotificationHubs.BrowserRegistrationDescription"/> instance whose fields values will be copied.</param>
        public BrowserRegistrationDescription(BrowserRegistrationDescription sourceRegistration) : base(sourceRegistration)
        {
            var browserPushSubscription = new BrowserPushSubscription
            {
                Endpoint = sourceRegistration.Endpoint,
                P256DH = sourceRegistration.P256DH,
                Auth = sourceRegistration.Auth,
            };

            ValidateBrowserPushSubscription(browserPushSubscription);

            _browserPushSubscription = browserPushSubscription;
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.BrowserRegistrationDescription"/> class.
        /// </summary>
        /// <param name="browserPushSubscription">The browser push subscription.</param>
        public BrowserRegistrationDescription(BrowserPushSubscription browserPushSubscription) : this(browserPushSubscription, null)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.BrowserRegistrationDescription"/> class.
        /// </summary>
        /// <param name="browserPushSubscription">The browser push subscription.</param>
        /// <param name="tags">Collection of tags. Tags can be used for audience targeting purposes.</param>
        public BrowserRegistrationDescription(BrowserPushSubscription browserPushSubscription, IEnumerable<string> tags)
            : this(string.Empty, browserPushSubscription, tags)
        {
        }

        internal BrowserRegistrationDescription(string notificationHubPath, BrowserPushSubscription browserPushSubscription, IEnumerable<string> tags) : base(notificationHubPath)
        {
            ValidateBrowserPushSubscription(browserPushSubscription);

            _browserPushSubscription = new BrowserPushSubscription
            {
                Endpoint = browserPushSubscription.Endpoint,
                P256DH = browserPushSubscription.P256DH,
                Auth = browserPushSubscription.Auth,
            };

            if (tags != null)
            {
                Tags = new HashSet<string>(tags);
            }
        }

        internal override string AppPlatForm
        {
            get { return BrowserCredential.AppPlatformName; }
        }

        internal override string RegistrationType
        {
            get { return BrowserCredential.AppPlatformName; }
        }

        internal override string PlatformType
        {
            get { return BrowserCredential.AppPlatformName; }
        }

        internal override string GetPnsHandle() => JsonSerializer.Serialize(_browserPushSubscription);

        internal override void SetPnsHandle(string pnsHandle)
        {
            var browserPushSubscription = JsonSerializer.Deserialize<BrowserPushSubscription>(pnsHandle);
            Endpoint = browserPushSubscription.Endpoint;
            P256DH = browserPushSubscription.P256DH;
            Auth = browserPushSubscription.Auth;

            _browserPushSubscription = browserPushSubscription;
        }

        internal override RegistrationDescription Clone()
        {
            return new BrowserRegistrationDescription(this);
        }

        private void ValidateBrowserPushSubscription(BrowserPushSubscription browserPushSubscription)
        {
            if (string.IsNullOrWhiteSpace(browserPushSubscription.Endpoint))
            {
                throw new ArgumentNullException(nameof(browserPushSubscription.Endpoint));
            }

            if (string.IsNullOrWhiteSpace(browserPushSubscription.P256DH))
            {
                throw new ArgumentNullException(nameof(browserPushSubscription.P256DH));
            }

            if (string.IsNullOrWhiteSpace(browserPushSubscription.Auth))
            {
                throw new ArgumentNullException(nameof(browserPushSubscription.Auth));
            }
        }
    }
}
