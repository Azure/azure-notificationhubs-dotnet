//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a Baidu notification hub notification, including the target tag
    /// </summary>
    public sealed class BaiduNotification : Notification, INativeNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduNotification"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BaiduNotification(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduNotification"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when a Baidu notification message.description is null</exception>
        public BaiduNotification(string message, int? messageType)
            : base(null, null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("baidu notification message.description");
            }

            this.Body = message;
            this.MessageType = messageType;
        }

        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public int? MessageType { get; set; }

        /// <summary>
        /// Gets the type of the platform.
        /// </summary>
        /// <value>
        /// The type of the platform.
        /// </value>
        protected override string PlatformType
        {
            get { return BaiduCredential.AppPlatformName; }
        }

        /// <summary>
        /// Gets content type.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public override string ContentType
        {
            get { return "application/x-www-form-urlencoded"; }
        }

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected override void OnValidateAndPopulateHeaders()
        {
            if (this.MessageType != null)
            {
                this.AddOrUpdateHeader(BaiduRegistrationDescription.MessageTypeHeader, this.MessageType.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
     }
}
