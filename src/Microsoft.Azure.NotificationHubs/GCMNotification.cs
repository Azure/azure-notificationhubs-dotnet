//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a Google Cloud Messaging notification.
    /// </summary>
    [Obsolete("GcmNotification is deprecated, please use FcmNotification instead.")]
    internal sealed class GcmNotification : Notification, INativeNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.GcmNotification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param>
        public GcmNotification(string jsonPayload)
            : base(null, null)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.Body = jsonPayload;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.GcmNotification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param><param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public GcmNotification(string jsonPayload, string tag)
            : base(null, tag)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.Body = jsonPayload;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.GcmNotification"/> class from <see cref="T:Microsoft.Azure.NotificationHubs.FcmNotification"/> object.
        /// </summary>
        /// <param name="fcmNotification">The FcmNotification object to create a new GcmNotifications from.</param>
        [Obsolete("This method is obsolete.")]
        public GcmNotification(FcmNotification fcmNotification)
            : base(fcmNotification.Headers, fcmNotification.Tag)
        {
            this.Body = fcmNotification.Body;
        }

        /// <summary>
        /// Gets the type of the platform.
        /// </summary>
        /// <value>
        /// The type of the platform.
        /// </value>
        protected override string PlatformType
        {
            get { return GcmCredential.AppPlatformName; }
        }

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected override void OnValidateAndPopulateHeaders()
        {
        }
    }
}
