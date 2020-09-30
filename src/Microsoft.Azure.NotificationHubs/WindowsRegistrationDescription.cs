//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Provides description for Windows registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.WindowsRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class WindowsRegistrationDescription : RegistrationDescription
    {
        // Request
        internal const string Type = "X-WNS-Type";
        internal const string Raw = "wns/raw";
        internal const string Badge = "wns/badge";
        internal const string Tile = "wns/tile";
        internal const string Toast = "wns/toast";

        internal override string AppPlatForm
        {
            get
            {
                return WnsCredential.AppPlatformName;
            }
        }

        internal override string RegistrationType
        {
            get
            {
                return WnsCredential.AppPlatformName;
            }
        }

        internal override string PlatformType
        {
            get
            {
                return WnsCredential.AppPlatformName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        public WindowsRegistrationDescription(string channelUri)
            : this(string.Empty, new Uri(channelUri), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param><param name="tags">A list of tags.</param>
        public WindowsRegistrationDescription(string channelUri, IEnumerable<string> tags)
            : this(string.Empty, new Uri(channelUri), tags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        public WindowsRegistrationDescription(Uri channelUri)
            : this(string.Empty, channelUri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param><param name="tags">A list of tags.</param>
        public WindowsRegistrationDescription(Uri channelUri, IEnumerable<string> tags)
            : this(string.Empty, channelUri, tags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public WindowsRegistrationDescription(WindowsRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.ChannelUri = sourceRegistration.ChannelUri;
            this.SecondaryTileName = sourceRegistration.SecondaryTileName;
        }

        internal WindowsRegistrationDescription(string notificationHubPath, string channelUri, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (string.IsNullOrWhiteSpace(channelUri))
            {
                throw new ArgumentNullException("channelUri");
            }

            this.ChannelUri = new Uri(channelUri);
            if (tags != null)
            {
                this.Tags = new HashSet<string>(tags);
            }
        }

        internal WindowsRegistrationDescription(string notificationHubPath, Uri channelUri, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (channelUri == null)
            {
                throw new ArgumentNullException("channelUri");
            }

            this.ChannelUri = channelUri;

            if (tags != null)
            {
                this.Tags = new HashSet<string>(tags);
            }
        }

        /// <summary>
        /// Gets or sets the channel URI.
        /// </summary>
        /// <value>
        /// The channel URI.
        /// </value>
        [DataMember(Name = ManagementStrings.ChannelUri, Order = 2001, IsRequired = true)]
        public Uri ChannelUri { get; set; }

        /// <summary>
        /// Gets or sets the name of the secondary tile.
        /// </summary>
        /// <value>
        /// The name of the secondary tile.
        /// </value>
        [DataMember(Name = ManagementStrings.SecondaryTileName, Order = 2002, IsRequired = false, EmitDefaultValue = false)]
        public string SecondaryTileName { get; set; }

        /// <summary>
        /// Called when validate event occurs.
        /// </summary>
        /// <exception cref="System.Runtime.Serialization.InvalidDataContractException"></exception>
        internal override void OnValidate()
        {
            // Channel URI validations
            if (string.IsNullOrWhiteSpace(this.ChannelUri.ToString()) || !this.ChannelUri.IsAbsoluteUri)
            {
                throw new InvalidDataContractException(SRClient.ChannelUriNullOrEmpty);
            }
        }

        internal override string GetPnsHandle()
        {
            return this.ChannelUri.AbsoluteUri;
        }

        internal override void SetPnsHandle(string pnsHandle)
        {
            if (!string.IsNullOrEmpty(pnsHandle))
            {
                this.ChannelUri = new Uri(pnsHandle);
            }
            else
            {
                this.ChannelUri = null;
            }
        }

        internal override RegistrationDescription Clone()
        {
            return new WindowsRegistrationDescription(this);
        }
    }
}
