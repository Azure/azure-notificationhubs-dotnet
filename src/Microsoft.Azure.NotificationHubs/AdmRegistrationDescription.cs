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
    /// Represents the description of the Amazon Device Messaging (ADM) registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.AdmRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class AdmRegistrationDescription : RegistrationDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdmRegistrationDescription"/> class.
        /// </summary>
        /// <param name="admRegistrationId">The ADM registration identifier.</param>
        public AdmRegistrationDescription(string admRegistrationId)
            : this(string.Empty, admRegistrationId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdmRegistrationDescription"/> class.
        /// </summary>
        /// <param name="admRegistrationId">The ADM registration identifier.</param>
        /// <param name="tags">The tags.</param>
        public AdmRegistrationDescription(string admRegistrationId, IEnumerable<string> tags)
            : this(string.Empty, admRegistrationId, tags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdmRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public AdmRegistrationDescription(AdmRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            AdmRegistrationId = sourceRegistration.AdmRegistrationId;
        }

        internal AdmRegistrationDescription(string notificationHubPath, string admRegistrationId, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (admRegistrationId == null)
            {
                throw new ArgumentNullException("admRegistrationId");
            }

            AdmRegistrationId = admRegistrationId;

            if (tags != null)
            {
                Tags = new HashSet<string>(tags);
            }
        }

        /// <summary>
        /// Gets or sets the Amazon Device Messaging registration identifier.
        /// </summary>
        /// 
        /// <returns>
        /// The Amazon Device Messaging registration identifier.
        /// </returns>
        [DataMember(Name = ManagementStrings.AdmRegistrationId, Order = 2001, IsRequired = true)]
        public string AdmRegistrationId { get; set; }

        internal override string AppPlatForm
        {
            get
            {
                return AdmCredential.AppPlatformName;
            }
        }

        internal override string RegistrationType
        {
            get
            {
                return AdmCredential.AppPlatformName;
            }
        }

        internal override string PlatformType
        {
            get
            {
                return AdmCredential.AppPlatformName;
            }
        }

        internal override void OnValidate(ApiVersion version)
        {
            if (string.IsNullOrWhiteSpace(AdmRegistrationId))
            {
                throw new InvalidDataContractException(SRClient.AdmRegistrationIdInvalid);
            }
        }

        internal override string GetPnsHandle()
        {
            return AdmRegistrationId;
        }

        internal override void SetPnsHandle(string pnsHandle)
        {
            AdmRegistrationId = pnsHandle;
        }

        internal override RegistrationDescription Clone()
        {
            return new AdmRegistrationDescription(this);
        }
    }
}