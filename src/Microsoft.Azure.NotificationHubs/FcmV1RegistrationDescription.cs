//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents Notification Hub registration description for Firebase Cloud Messaging
    /// </summary>
    [DataContract(Name = ManagementStrings.FcmV1RegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class FcmV1RegistrationDescription : RegistrationDescription
    {
        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1RegistrationDescription"/> class by copying fields from the given instance
        /// </summary>
        /// <param name="sourceRegistration">Another <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1RegistrationDescription"/> instance fields values are copyed from</param>
        public FcmV1RegistrationDescription(FcmV1RegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.FcmV1RegistrationId = sourceRegistration.FcmV1RegistrationId;
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1RegistrationDescription"/> class using given Firebase Cloud Messaging registration id
        /// </summary>
        /// <param name="fcmV1RegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        public FcmV1RegistrationDescription(string fcmV1RegistrationId)
            : this(string.Empty, fcmV1RegistrationId, null)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1RegistrationDescription"/> class using given Firebase Cloud Messaging registration id and collection of tags
        /// </summary>
        /// <param name="fcmV1RegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        /// <param name="tags">Collection of tags. Tags can be used for audience targeting purposes.</param>
        public FcmV1RegistrationDescription(string fcmV1RegistrationId, IEnumerable<string> tags)
            : this(string.Empty, fcmV1RegistrationId, tags)
        {
        }

        internal FcmV1RegistrationDescription(string notificationHubPath, string fcmV1RegistrationId, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (string.IsNullOrWhiteSpace(fcmV1RegistrationId))
            {
                throw new ArgumentNullException(nameof(fcmV1RegistrationId));
            }

            this.FcmV1RegistrationId = fcmV1RegistrationId;
            if (tags != null)
            {
                Tags = new HashSet<string>(tags);
            }
        }

        /// <summary>
        /// Registration id obtained from the Firebase Messaging service
        /// </summary>
        [DataMember(Name = ManagementStrings.FcmV1RegistrationId, Order = 2001, IsRequired = true)]
        public string FcmV1RegistrationId { get; set; }

        internal override string AppPlatForm
        {
            get { return FcmV1Credential.AppPlatformName; }
        }

        internal override string RegistrationType
        {
            get { return FcmV1Credential.AppPlatformName; }
        }

        internal override string PlatformType
        {
            get { return FcmV1Credential.AppPlatformName; }
        }

        internal override string GetPnsHandle()
        {
            return FcmV1RegistrationId;
        }

        internal override void SetPnsHandle(string pnsHandle)
        {
            FcmV1RegistrationId = pnsHandle;
        }

        internal override void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(FcmV1RegistrationId))
            {
                throw new InvalidDataContractException(SRClient.FCMV1RegistrationInvalidId);
            }
        }

        internal override RegistrationDescription Clone()
        {
            return new FcmV1RegistrationDescription(this);
        }
    }
}
