//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents the Windows notification.
    /// </summary>
    public sealed class WindowsNotification : Notification, INativeNotification
    {
        const string contentType = "application/xml";
        const string WnsTypeName = "X-WNS-Type";
        const string Raw = "wns/raw";
        const string Badge = "wns/badge";
        const string Tile = "wns/tile";
        const string Toast = "wns/toast";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsNotification"/> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        public WindowsNotification(XmlDocument payLoad)
            : this(payLoad, wnsHeaders: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsNotification"/> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        public WindowsNotification(string payLoad)
            : this(payLoad, wnsHeaders: null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsNotification"/> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param><param name="wnsHeaders">A list of WNS headers.</param>
        public WindowsNotification(XmlDocument payLoad, IDictionary<string, string> wnsHeaders)
            : this(payLoad.InnerXml, wnsHeaders)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsNotification"/> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param><param name="wnsHeaders">A list of WNS headers.</param>
        public WindowsNotification(string payLoad, IDictionary<string, string> wnsHeaders)
            : base(wnsHeaders, null, contentType)
        {
            if (string.IsNullOrWhiteSpace(payLoad))
            {
                throw new ArgumentNullException("payLoad");
            }

            this.Body = payLoad;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsNotification"/> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param><param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public WindowsNotification(XmlDocument payLoad, string tag)
            : this(payLoad.InnerXml, null, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsNotification"/> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param><param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public WindowsNotification(string payLoad, string tag)
            : this(payLoad, null, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsNotification"/> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param><param name="wnsHeaders">A list of WNS headers.</param><param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public WindowsNotification(XmlDocument payLoad, IDictionary<string, string> wnsHeaders, string tag)
            : this(payLoad.InnerXml, wnsHeaders, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WindowsNotification"/> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param><param name="wnsHeaders">A list of WNS headers.</param><param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public WindowsNotification(string payLoad, IDictionary<string, string> wnsHeaders, string tag)
            : base(wnsHeaders, tag, contentType)
        {
            if (string.IsNullOrWhiteSpace(payLoad))
            {
                throw new ArgumentNullException("payLoad");
            }

            this.Body = payLoad;
        }

        /// <summary>
        /// Gets the type of the platform.
        /// </summary>
        /// <value>
        /// The type of the platform.
        /// </value>
        protected override string PlatformType
        {
            get { return WnsCredential.AppPlatformName; }
        }

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected override void OnValidateAndPopulateHeaders()
        {
            if (this.Headers.ContainsKey(WnsTypeName) &&
                this.Headers[WnsTypeName].Equals(Raw, StringComparison.OrdinalIgnoreCase))
            {
                //raw notification
                this.AddNotificationTypeHeader(WindowsTemplateBodyType.Raw);
                this.ContentType = "application/octet-stream";
            }
            else
            {
                //non-raw notification
                AddNotificationTypeHeader(RegistrationSDKHelper.DetectWindowsTemplateRegistationType(this.Body, SRClient.NotSupportedXMLFormatAsPayload));

                // add xml declaration section if necessary
                this.Body = RegistrationSDKHelper.AddDeclarationToXml(this.Body);
            }
        }

        void AddNotificationTypeHeader(WindowsTemplateBodyType bodyType)
        {
            switch (bodyType)
            {
                case WindowsTemplateBodyType.Badge:
                    this.AddOrUpdateHeader(WnsTypeName, Badge);
                    break;
                case WindowsTemplateBodyType.Tile:
                    this.AddOrUpdateHeader(WnsTypeName, Tile);
                    break;
                case WindowsTemplateBodyType.Toast:
                    this.AddOrUpdateHeader(WnsTypeName, Toast);
                    break;
                case WindowsTemplateBodyType.Raw:
                    this.AddOrUpdateHeader(WnsTypeName, Raw);
                    break;
            }
        }
    }
}
