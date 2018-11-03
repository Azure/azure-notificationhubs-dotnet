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
    using Microsoft.Azure.NotificationHubs;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents the description of the MPNS registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.MpnsRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class MpnsRegistrationDescription : RegistrationDescription
    {
        internal const string MpnsHeaderPrefix = "X-";
        internal const string NotificationClass = "X-NotificationClass";
        internal const string Type = "X-WindowsPhone-Target";
        internal const string Tile = "token";
        internal const string Toast = "toast";

        internal const string TileClass = "1";
        internal const string ToastClass = "2";
        internal const string RawClass = "3";

        internal const string NamespaceName = "WPNotification";
        internal const string NotificationElementName = "Notification";

        // Valid Channel URI Parts
        internal const string ProdChannelUriPart = @".notify.live.net";
        internal const string MockChannelUriPart = @"localhost:8450/MPNS/Mock";
        internal const string MockSSLChannelUriPart = @"localhost:8451/MPNS/Mock";
        internal const string MockRunnerChannelUriPart = @"pushtestservice.cloudapp.net";
        internal const string MockIntChannelUriPart = @"pushtestservice4.cloudapp.net";
        internal const string MockPerformanceChannelUriPart = @"pushperfnotificationserver.cloudapp.net";
        internal const string MockEnduranceChannelUriPart = @"pushstressnotificationserver.cloudapp.net";
        internal const string MockEnduranceChannelUriPart1 = @"pushnotificationserver.cloudapp.net";

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public MpnsRegistrationDescription(MpnsRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.ChannelUri = sourceRegistration.ChannelUri;
            this.SecondaryTileName = sourceRegistration.SecondaryTileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        public MpnsRegistrationDescription(string channelUri)
            : this(string.Empty, new Uri(channelUri), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        public MpnsRegistrationDescription(Uri channelUri)
            : this(string.Empty, channelUri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="tags">Collection of tags.</param>
        public MpnsRegistrationDescription(string channelUri, IEnumerable<string> tags)
            : this(string.Empty, new Uri(channelUri), tags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="tags">Collection of tags.</param>
        public MpnsRegistrationDescription(Uri channelUri, IEnumerable<string> tags)
            : this(string.Empty, channelUri, tags)
        {
        }

        internal MpnsRegistrationDescription(string notificationHubPath, string channelUri, IEnumerable<string> tags)
            : this(notificationHubPath, new Uri(channelUri), tags)
        {
        }

        internal MpnsRegistrationDescription(string notificationHubPath, Uri channelUri, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            this.ChannelUri = channelUri;
            if (tags != null)
            {
                this.Tags = new HashSet<string>(tags);
            }
        }

        internal override string AppPlatForm
        {
            get { return MpnsCredential.AppPlatformName; }
        }

        internal override string RegistrationType
        {
            get { return MpnsCredential.AppPlatformName; }
        }

        internal override string PlatformType
        {
            get { return MpnsCredential.AppPlatformName; }
        }

        internal bool IsMockMpns()
        {
            return this.ChannelUri.Host.ToUpperInvariant().Contains("CLOUDAPP.NET");
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
            return new MpnsRegistrationDescription(this);
        }
    }
}