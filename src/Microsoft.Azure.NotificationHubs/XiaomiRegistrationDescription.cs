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
    /// Represents the description of the Xiaomi registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.XiaomiRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class XiaomiRegistrationDescription : RegistrationDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XiaomiRegistrationDescription"/> class.
        /// </summary>
        /// <param name="xiaomiRegistrationId">The Xiaomi registration identifier.</param>
        public XiaomiRegistrationDescription(string xiaomiRegistrationId)
            : this(string.Empty, xiaomiRegistrationId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XiaomiRegistrationDescription"/> class.
        /// </summary>
        /// <param name="xiaomiRegistrationId">The Xiaomi registration identifier.</param>
        /// <param name="tags">The tags.</param>
        public XiaomiRegistrationDescription(string xiaomiRegistrationId, IEnumerable<string> tags)
            : this(string.Empty, xiaomiRegistrationId, tags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XiaomiRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public XiaomiRegistrationDescription(XiaomiRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            XiaomiRegistrationId = sourceRegistration.XiaomiRegistrationId;
        }

        internal XiaomiRegistrationDescription(string notificationHubPath, string xiaomiRegistrationId, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (xiaomiRegistrationId == null)
            {
                throw new ArgumentNullException("xiaomiRegistrationId");
            }

            XiaomiRegistrationId = xiaomiRegistrationId;

            if (tags != null)
            {
                Tags = new HashSet<string>(tags);
            }
        }

        /// <summary>
        /// Gets or sets the Xiaomi registration identifier.
        /// </summary>
        /// 
        /// <returns>
        /// The Xiaomi registration identifier.
        /// </returns>
        [DataMember(Name = ManagementStrings.XiaomiRegistrationId, Order = 2001, IsRequired = true)]
        public string XiaomiRegistrationId { get; set; }

        internal override string AppPlatForm
        {
            get
            {
                return "xiaomi"; 
            }
        }

        internal override string RegistrationType
        {
            get
            {
                return "xiaomi"; 
            }
        }

        internal override string PlatformType
        {
            get
            {
                return "xiaomi"; 
            }
        }

        internal override void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(XiaomiRegistrationId))
            {
                throw new InvalidDataContractException(SRClient.XiaomiRegistrationIdInvalid);
            }
        }

        internal override string GetPnsHandle()
        {
            return XiaomiRegistrationId;
        }

        internal override void SetPnsHandle(string pnsHandle)
        {
            XiaomiRegistrationId = pnsHandle;
        }

        internal override RegistrationDescription Clone()
        {
            return new XiaomiRegistrationDescription(this);
        }
    }
}
