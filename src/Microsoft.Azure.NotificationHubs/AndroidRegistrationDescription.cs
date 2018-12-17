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
    /// Represents Notification Hub registration description for Google Cloud Messaging
    /// </summary>
    [DataContract(Name = ManagementStrings.GcmRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    [Obsolete("GcmRegistrationDescription is deprecated, please use FcmRegistrationDescription instead.")]
    public class GcmRegistrationDescription : RegistrationDescription
    {
        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.GcmRegistrationDescription"/> class by copying fields from the given instance
        /// </summary>
        /// <param name="sourceRegistration">Another <see cref="T:Microsoft.Azure.NotificationHubs.GcmRegistrationDescription"/> instance fields values are copyed from</param>
        public GcmRegistrationDescription(GcmRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.GcmRegistrationId = sourceRegistration.GcmRegistrationId;
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.GcmRegistrationDescription"/> class using given Google Cloud Messaging registration id
        /// </summary>
        /// <param name="gcmRegistrationId">Registration id obtained from the Google Cloud Messaging service</param>
        public GcmRegistrationDescription(string gcmRegistrationId)
            : this(string.Empty, gcmRegistrationId, null)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.GcmRegistrationDescription"/> class using given Google Cloud Messaging registration id and collection of tags
        /// </summary>
        /// <param name="gcmRegistrationId">Registration id obtained from the Google Cloud Messaging service</param>
        /// <param name="tags">Collection of tags. Tags can be used for audience targeting purposes.</param>
        public GcmRegistrationDescription(string gcmRegistrationId, IEnumerable<string> tags)
            : this(string.Empty, gcmRegistrationId, tags)
        {
        }

        internal GcmRegistrationDescription(string notificationHubPath, string gcmRegistrationId, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (string.IsNullOrWhiteSpace(gcmRegistrationId))
            {
                throw new ArgumentNullException("gcmRegistrationId");
            }

            this.GcmRegistrationId = gcmRegistrationId;
            if (tags != null)
            {
                this.Tags = new HashSet<string>(tags);
            }
        }

        /// <summary>
        /// Registration id obtained from the Google Cloud Messaging service
        /// </summary>
        [DataMember(Name = ManagementStrings.GcmRegistrationId, Order = 2001, IsRequired = true)]
        public string GcmRegistrationId { get; set; }

        internal override string AppPlatForm
        {
            get { return GcmCredential.AppPlatformName; }
        }

        internal override string RegistrationType
        {
            get { return GcmCredential.AppPlatformName; }
        }

        internal override string PlatformType
        {
            get { return GcmCredential.AppPlatformName; }
        }

        internal override string GetPnsHandle()
        {
            return this.GcmRegistrationId;
        }

        internal override void SetPnsHandle(string pnsHandle)
        {
            this.GcmRegistrationId = pnsHandle;
        }

        internal override void OnValidate(ApiVersion version)
        {
            if (string.IsNullOrWhiteSpace(this.GcmRegistrationId))
            {
                throw new InvalidDataContractException(SRClient.GCMRegistrationInvalidId);
            }
        }

        internal override RegistrationDescription Clone()
        {
            return new GcmRegistrationDescription(this);
        }
    }

    /// <summary>
    /// Represents Notification Hub registration description for Firebase Cloud Messaging
    /// </summary>
    [DataContract(Name = ManagementStrings.FcmRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class FcmRegistrationDescription : RegistrationDescription
    {
        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmRegistrationDescription"/> class by copying fields from the given instance
        /// </summary>
        /// <param name="sourceRegistration">Another <see cref="T:Microsoft.Azure.NotificationHubs.FcmRegistrationDescription"/> instance fields values are copyed from</param>
        public FcmRegistrationDescription(FcmRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.FcmRegistrationId = sourceRegistration.FcmRegistrationId;
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmRegistrationDescription"/> class using given Firebase Cloud Messaging registration id
        /// </summary>
        /// <param name="fcmRegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        public FcmRegistrationDescription(string fcmRegistrationId)
            : this(string.Empty, fcmRegistrationId, null)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmRegistrationDescription"/> class using given Firebase Cloud Messaging registration id and collection of tags
        /// </summary>
        /// <param name="fcmRegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        /// <param name="tags">Collection of tags. Tags can be used for audience targeting purposes.</param>
        public FcmRegistrationDescription(string fcmRegistrationId, IEnumerable<string> tags)
            : this(string.Empty, fcmRegistrationId, tags)
        {
        }

        internal FcmRegistrationDescription(string notificationHubPath, string fcmRegistrationId, IEnumerable<string> tags)
            : base(notificationHubPath)
        {
            if (string.IsNullOrWhiteSpace(fcmRegistrationId))
            {
                throw new ArgumentNullException("fcmRegistrationId");
            }

            this.FcmRegistrationId = fcmRegistrationId;
            if (tags != null)
            {
                this.Tags = new HashSet<string>(tags);
            }
        }

        /// <summary>
        /// Registration id obtained from the Firebase Messaging service
        /// </summary>
        [DataMember(Name = ManagementStrings.FcmRegistrationId, Order = 2001, IsRequired = true)]
        public string FcmRegistrationId { get; set; }

        internal override string AppPlatForm
        {
            get { return FcmCredential.AppPlatformName; }
        }

        internal override string RegistrationType
        {
            get { return FcmCredential.AppPlatformName; }
        }

        internal override string PlatformType
        {
            get { return FcmCredential.AppPlatformName; }
        }

        internal override string GetPnsHandle()
        {
            return this.FcmRegistrationId;
        }

        internal override void SetPnsHandle(string pnsHandle)
        {
            this.FcmRegistrationId = pnsHandle;
        }

        internal override void OnValidate(ApiVersion version)
        {
            if (string.IsNullOrWhiteSpace(this.FcmRegistrationId))
            {
                throw new InvalidDataContractException(SRClient.FCMRegistrationInvalidId);
            }
        }

        internal override RegistrationDescription Clone()
        {
            return new FcmRegistrationDescription(this);
        }
    }
}
