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
    /// Provides notification for Microsoft Push Notification Service (MPNS).
    /// </summary>
    public sealed class MpnsNotification : Notification, INativeNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsNotification" /> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        public MpnsNotification(XmlDocument payLoad)
            : this(payLoad, mpnsHeaders: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsNotification" /> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        /// <param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public MpnsNotification(XmlDocument payLoad, string tag)
            : this(payLoad, null, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsNotification" /> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        public MpnsNotification(string payLoad)
            : this(payLoad, mpnsHeaders:null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsNotification" /> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        /// <param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public MpnsNotification(string payLoad, string tag)
            : this(payLoad, null, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsNotification" /> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        /// <param name="mpnsHeaders">The MPNS headers.</param>
        public MpnsNotification(XmlDocument payLoad, IDictionary<string, string> mpnsHeaders)
            : this(payLoad.InnerXml, mpnsHeaders)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsNotification" /> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        /// <param name="mpnsHeaders">The MPNS headers.</param>
        /// <exception cref="System.ArgumentNullException">payLoad</exception>
        public MpnsNotification(string payLoad, IDictionary<string, string> mpnsHeaders)
            : base(mpnsHeaders, null)
        {
            if (string.IsNullOrWhiteSpace(payLoad))
            {
                throw new ArgumentNullException("payLoad");
            }

            this.Body = payLoad;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsNotification" /> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        /// <param name="mpnsHeaders">The MPNS headers.</param>
        /// <param name="tag">The notification tag.</param>
        [Obsolete("This method is obsolete.")]
        public MpnsNotification(XmlDocument payLoad, IDictionary<string, string> mpnsHeaders, string tag)
            : this(payLoad.InnerXml, mpnsHeaders, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsNotification" /> class.
        /// </summary>
        /// <param name="payLoad">The payload.</param>
        /// <param name="mpnsHeaders">The MPNS headers.</param>
        /// <param name="tag">The notification tag.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the payload is null or empty</exception>
        [Obsolete("This method is obsolete.")]
        public MpnsNotification(string payLoad, IDictionary<string, string> mpnsHeaders, string tag)
            : base(mpnsHeaders, tag)
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
            get { return MpnsCredential.AppPlatformName; }
        }

        /// <summary>
        /// Gets content type.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public override string ContentType
        {
            get { return "application/xml"; }
        }

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected override void OnValidateAndPopulateHeaders()
        {
            if (!this.Headers.ContainsKey(MpnsRegistrationDescription.NotificationClass))
            {
                AddNotificationTypeHeader(RegistrationSDKHelper.DetectMpnsTemplateRegistationType(this.Body, SRClient.NotSupportedXMLFormatAsPayloadForMpns));

                // add xml declaration section if necessary
                this.Body = RegistrationSDKHelper.AddDeclarationToXml(this.Body);
            }
        }

        void AddNotificationTypeHeader(MpnsTemplateBodyType bodyType)
        {
            switch (bodyType)
            {
                case MpnsTemplateBodyType.Tile:
                    this.AddOrUpdateHeader(MpnsRegistrationDescription.Type, MpnsRegistrationDescription.Tile);
                    this.AddOrUpdateHeader(MpnsRegistrationDescription.NotificationClass, MpnsRegistrationDescription.TileClass);
                    break;
                case MpnsTemplateBodyType.Toast:
                    this.AddOrUpdateHeader(MpnsRegistrationDescription.Type, MpnsRegistrationDescription.Toast);
                    this.AddOrUpdateHeader(MpnsRegistrationDescription.NotificationClass, MpnsRegistrationDescription.ToastClass);
                    break;
                case MpnsTemplateBodyType.Raw:
                    this.AddOrUpdateHeader(MpnsRegistrationDescription.NotificationClass, MpnsRegistrationDescription.RawClass);
                    break;
            }
        }
    }
}
