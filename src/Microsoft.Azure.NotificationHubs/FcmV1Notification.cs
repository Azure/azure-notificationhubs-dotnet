//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;
using System.Text;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a Firebase Cloud Messaging V1 notification.
    /// </summary>
    public sealed class FcmV1Notification : Notification, INativeNotification
    {
        static string contentType = $"application/json;charset={Encoding.UTF8.WebName}";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1Notification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param>
        public FcmV1Notification(string jsonPayload)
            : base(null, null, contentType)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            Body = jsonPayload;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1Notification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param><param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public FcmV1Notification(string jsonPayload, string tag)
            : base(null, tag, contentType)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            Body = jsonPayload;
        }

        /// <summary>
        /// Gets the type of the platform.
        /// </summary>
        /// <value>
        /// The type of the platform.
        /// </value>
        protected override string PlatformType
        {
            get { return FcmV1Credential.AppPlatformName; }
        }

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected override void OnValidateAndPopulateHeaders()
        {
        }
    }
}
