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
    /// Represents a Baidu registration description.
    /// </summary>
    [DataContract(Name = ManagementStrings.BaiduRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class BaiduRegistrationDescription : RegistrationDescription
    {
        internal const string MessageTypeHeader = "X-Baidu-Message-Type";

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduRegistrationDescription"/> class.
        /// </summary>
        /// <param name="pnsHandle">The PNS handle.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when BaiduUserId is null</exception>
        public BaiduRegistrationDescription(string pnsHandle)
            : base(string.Empty)
        {
            int index = pnsHandle.IndexOf('-');
            this.BaiduUserId = pnsHandle.Substring(0, index);
            this.BaiduChannelId = pnsHandle.Substring(index + 1, pnsHandle.Length - index - 1);
            if (string.IsNullOrWhiteSpace(BaiduUserId))
            {
                throw new ArgumentNullException("baiduRegistrationId");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public BaiduRegistrationDescription(BaiduRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.BaiduUserId = sourceRegistration.BaiduUserId;
            this.BaiduChannelId = sourceRegistration.BaiduChannelId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduRegistrationDescription"/> class.
        /// </summary>
        /// <param name="baiduUserId">The Baidu user identifier.</param>
        /// <param name="baiduChannelId">The Baidu channel identifier.</param>
        /// <param name="tags">The tags.</param>
        public BaiduRegistrationDescription(string baiduUserId, string baiduChannelId, IEnumerable<string> tags)
            : this(string.Empty, baiduUserId, baiduChannelId, tags)
        {
        }

        internal BaiduRegistrationDescription(string notificationHubPath, string baiduUserId, string baiduChannelId, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (string.IsNullOrWhiteSpace(baiduUserId))
            {
                throw new ArgumentNullException("baiduRegistrationId");
            }

            this.BaiduUserId = baiduUserId;
            this.BaiduChannelId = baiduChannelId;
            if (tags != null)
            {
                this.Tags = new HashSet<string>(tags);
            }
        }

        /// <summary>
        /// Gets or sets the Baidu user identifier.
        /// </summary>
        /// <value>
        /// The Baidu user identifier.
        /// </value>
        [DataMember(Name = ManagementStrings.BaiduUserId, Order = 2001, IsRequired = true)]
        public string BaiduUserId { get; set; }

        /// <summary>
        /// Gets or sets the Baidu channel identifier.
        /// </summary>
        /// <value>
        /// The Baidu channel identifier.
        /// </value>
        [DataMember(Name = ManagementStrings.BaiduChannelId, Order = 2002, IsRequired = true)]
        public string BaiduChannelId { get; set; }

        internal override string AppPlatForm
        {
            get { return BaiduCredential.AppPlatformName; }
        }

        internal override string RegistrationType
        {
            get { return BaiduCredential.AppPlatformName; }
        }

        internal override string PlatformType
        {
            get { return BaiduCredential.AppPlatformName; }
        }

        internal override string GetPnsHandle()
        {
            return this.BaiduUserId + "-" + this.BaiduChannelId;
        }

        internal override void SetPnsHandle(string pnsHandle)
        {
            if (!string.IsNullOrEmpty(pnsHandle))
            {
                int index = pnsHandle.IndexOf('-');
                this.BaiduUserId = pnsHandle.Substring(0, index);
                this.BaiduChannelId = pnsHandle.Substring(index + 1, pnsHandle.Length - index - 1);
            }
        }

        internal override void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(this.BaiduChannelId))
            {
                throw new InvalidDataContractException(SRClient.BaiduRegistrationInvalidId);
            }

            // @TODO: Verify Channel Id
        }

        internal override RegistrationDescription Clone()
        {
            return new BaiduRegistrationDescription(this);
        }
    }
}
