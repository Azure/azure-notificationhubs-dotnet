//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents the PNS Feedback
    /// </summary>
    [DataContract(Name = ManagementStrings.PnsFeedback, Namespace = ManagementStrings.Namespace)]

    public sealed class PnsFeedback
    {
        /// <summary>
        /// Gets or sets the feedback time.
        /// </summary>
        /// <value>
        /// The feedback time.
        /// </value>
        [DataMember(Name = ManagementStrings.FeedbackTime, IsRequired = true, Order = 1000)]
        public DateTime FeedbackTime { get; set; }

        /// <summary>
        /// Gets or sets the notification system error.
        /// </summary>
        /// <value>
        /// The notification system error.
        /// </value>
        [DataMember(Name = ManagementStrings.NotificationSystemError, IsRequired = true, Order = 1001)]
        public string NotificationSystemError { get; set; }

        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        [DataMember(Name = ManagementStrings.Platform, IsRequired = true, Order = 1002)]
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the PNS handle.
        /// </summary>
        /// <value>
        /// The PNS handle.
        /// </value>
        [DataMember(Name = ManagementStrings.PnsHandle, IsRequired = true, Order = 1003)]
        public string PnsHandle { get; set; }

        /// <summary>
        /// Gets or sets the registration identifier.
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        [DataMember(Name = ManagementStrings.RegistrationId, IsRequired = false, EmitDefaultValue = false, Order = 1004)]
        public string RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the installation identifier.
        /// </summary>
        /// <value>
        /// The installation identifier.
        /// </value>
        [DataMember(Name = ManagementStrings.InstallationId, IsRequired = false, EmitDefaultValue = false, Order = 1005)]
        public string InstallationId { get; set; }

        /// <summary>
        /// Gets or sets the notification identifier.
        /// </summary>
        /// <value>
        /// The notification identifier.
        /// </value>
        [DataMember(Name = ManagementStrings.NotificationId, IsRequired = false, EmitDefaultValue = false, Order = 1006)]
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the new PNS handle.
        /// </summary>
        /// <value>
        /// The PNS handle.
        /// </value>
        [DataMember(Name = ManagementStrings.NewPnsHandle, IsRequired = false, EmitDefaultValue = false, Order = 1007)]
        public string NewPnsHandle { get; set; }

        internal int PushOutcome { get; set; }
    }
}

