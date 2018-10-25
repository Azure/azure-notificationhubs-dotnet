// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.

using System;
using System.Xml.Serialization;

namespace ParseFeedbackSample
{
    [Serializable]
    [XmlRoot(ElementName = "PnsFeedback", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public class PnsFeedback
    {
        /// <summary>
        /// Gets or sets the feedback time.
        /// </summary>
        /// <value>
        /// The feedback time.
        /// </value>
        public DateTime FeedbackTime { get; set; }

        /// <summary>
        /// Gets or sets the notification system error.
        /// </summary>
        /// <value>
        /// The notification system error.
        /// </value>
        public PnsError NotificationSystemError { get; set; }

        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the PNS handle.
        /// </summary>
        /// <value>
        /// The PNS handle.
        /// </value>
        public string PnsHandle { get; set; }

        /// <summary>
        /// Gets or sets the registration identifier.
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        public string RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the installation identifier.
        /// </summary>
        /// <value>
        /// The installation identifier.
        /// </value>
        public string InstallationId { get; set; }

        /// <summary>
        /// Gets or sets the notification identifier.
        /// </summary>
        /// <value>
        /// The notification identifier.
        /// </value>
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the new PNS handle.
        /// </summary>
        /// <value>
        /// The PNS handle.
        /// </value>
        public string NewPnsHandle { get; set; }
    }
}
