//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Provides description for Windows registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.WindowsRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class WindowsRegistrationDescription : RegistrationDescription
    {
        // Request
        internal const string WnsHeaderPrefix = "X-WNS-";
        internal const string Type = "X-WNS-Type";
        internal const string Raw = "wns/raw";
        internal const string Badge = "wns/badge";
        internal const string Tile = "wns/tile";
        internal const string Toast = "wns/toast";

        // Valid Channel URI Parts
        internal const string ProdChannelUriPart = @"notify.windows.com";
        internal const string MockChannelUriPart = @"localhost:8450/WNS/Mock";
        internal const string MockRunnerChannelUriPart = @"pushtestservice.cloudapp.net";
        internal const string MockIntChannelUriPart = @"pushtestservice4.cloudapp.net";
        internal const string MockPerformanceChannelUriPart = @"pushperfnotificationserver.cloudapp.net";
        internal const string MockEnduranceChannelUriPart = @"pushstressnotificationserver.cloudapp.net";
        internal const string MockEnduranceChannelUriPart1 = @"pushnotificationserver.cloudapp.net";

        internal static HashSet<string> SupportedWnsTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Raw,
            Badge,
            Toast,
            Tile
        };

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

        internal bool IsMockWns()
        {
            return this.ChannelUri.Host.ToUpperInvariant().Contains("CLOUDAPP.NET");
        }

        /// <summary>
        /// Called when validate event occurs.
        /// </summary>
        /// <param name="allowLocalMockPns">if set to <c>true</c> [allow local mock PNS].</param>
        /// <param name="version">The version.</param>
        /// <exception cref="System.Runtime.Serialization.InvalidDataContractException"></exception>
        internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
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