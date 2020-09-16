//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents the Apple notification.
    /// </summary>
    public sealed class AppleNotification : Notification, INativeNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleNotification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param>
        public AppleNotification(string jsonPayload)
            : this(jsonPayload, expiry: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleNotification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="apnsHeaders">The APNS headers.</param>
        public AppleNotification(string jsonPayload, IDictionary<string, string> apnsHeaders)
            : base(apnsHeaders, null)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.Body = jsonPayload;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleNotification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public AppleNotification(string jsonPayload, string tag)
            : base(null, tag)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.Body = jsonPayload;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleNotification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="expiry">The expiration of the notification.</param>
        public AppleNotification(string jsonPayload, DateTime? expiry)
            : base(null, null)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.Expiry = expiry;
            this.Body = jsonPayload;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleNotification"/> class.
        /// </summary>
        /// <param name="jsonPayload">The JSON payload.</param>
        /// <param name="expiry">The expiration of the notification.</param><param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public AppleNotification(string jsonPayload, DateTime? expiry, string tag)
            : base(null, tag)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.Expiry = expiry;
            this.Body = jsonPayload;
        }

        /// <summary>
        /// Gets or sets the expiry date.
        /// </summary>
        /// <value>
        /// The expiry date.
        /// </value>
        public DateTime? Expiry { get; set; }

        /// <summary>
        /// Gets or sets the notification priority.
        /// </summary>
        /// <value>
        /// The notification priority.
        /// </value>
        public int? Priority { get; set; }

        /// <summary>
        /// Gets the type of the platform.
        /// </summary>
        /// <value>
        /// The type of the platform.
        /// </value>
        protected override string PlatformType
        {
            get { return ApnsCredential.AppPlatformName; }
        }

        /// <summary>
        /// Gets content type.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public override string ContentType
        {
            get { return "application/json"; }
        }

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected override void OnValidateAndPopulateHeaders()
        {
            if (this.Expiry != null)
            {
                this.AddOrUpdateHeader(AppleRegistrationDescription.ExpiryHeader, this.Expiry.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (this.Priority != null)
            {
                this.AddOrUpdateHeader(AppleRegistrationDescription.PriorityHeader, this.Priority.Value.ToString(CultureInfo.InvariantCulture));
            }

            // Validate apns-expiration header is in right format.
            if (this.Headers.ContainsKey(AppleRegistrationDescription.ApnsExpiryHeader))
            {
                int expiry;
                if (!int.TryParse(this.Headers[AppleRegistrationDescription.ApnsExpiryHeader], out expiry))
                {
                    throw new InvalidDataContractException(SRClient.ApnsExpiryHeaderDeserializationError);
                }
            }

            //  Validate apns-priority is in right format.
            if (this.Headers.ContainsKey(AppleRegistrationDescription.ApnsPriorityHeader))
            {
                byte priority;
                if (!byte.TryParse(this.Headers[AppleRegistrationDescription.ApnsPriorityHeader], out priority))
                {
                    throw new InvalidDataContractException(SRClient.PriorityDeserializationError);
                }
            }
        }           
    }
}
