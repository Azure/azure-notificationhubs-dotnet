//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Abstract class representing a generic notification hub notification, including the target tag.
    /// </summary>
    public abstract class Notification
    {
        Dictionary<string, string> headers; 
        internal const string FormatHeaderName = "ServiceBusNotification-Format";
        internal string tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Notification"/> class.
        /// </summary>
        /// <param name="additionalHeaders">The additional headers.</param><param name="tag">The notification tag.</param>
        protected Notification(IDictionary<string, string> additionalHeaders, string tag)
        {
            this.headers = additionalHeaders != null
                               ? new Dictionary<string, string>(additionalHeaders, StringComparer.OrdinalIgnoreCase)
                               : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            this.tag = tag;
            this.ContentType = "application/xml";
        }

        /// <summary>
        /// Add or updates a header with a given key and value.
        /// </summary>
        /// <param name="key">The header key.</param><param name="value">The value of the header.</param>
        protected void AddOrUpdateHeader(string key, string value)
        {
            if (!this.Headers.ContainsKey(key))
            {
                this.Headers.Add(key, value);
            }
            else
            {
                this.Headers[key] = value;
            }
        }

        internal void ValidateAndPopulateHeaders()
        {
            this.AddOrUpdateHeader(FormatHeaderName, this.PlatformType);
            this.OnValidateAndPopulateHeaders();
        }

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected abstract void OnValidateAndPopulateHeaders();
        /// <summary>
        /// Gets the type of the platform.
        /// </summary>
        /// <value>
        /// The type of the platform.
        /// </value>
        protected abstract string PlatformType { get; }

        /// <summary>
        /// Gets or sets notification headers.
        /// </summary>
        /// <value>
        /// The notification headers.
        /// </value>
        public Dictionary<string, string> Headers
        {
            get { return this.headers; }
            set { this.headers = new Dictionary<string, string>(value, StringComparer.OrdinalIgnoreCase); }
        }

        /// <summary>
        /// Gets or sets notification body.
        /// </summary>
        /// <value>
        /// The notification body.
        /// </value>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the type of the notification content.
        /// </summary>
        /// <value>
        /// The type of the notification content.
        /// </value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the notification tag.
        /// </summary>
        /// <value>
        /// The notification tag.
        /// </value>
        [Obsolete("This property is obsolete.")]
        public string Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }
    }
}