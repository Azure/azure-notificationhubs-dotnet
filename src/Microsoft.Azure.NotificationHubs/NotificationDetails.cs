//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents notification details
    /// </summary>
    [DataContract(Name = ManagementStrings.NotificationDetails, Namespace = ManagementStrings.Namespace)]
    public sealed class NotificationDetails : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the notification identifier.
        /// </summary>
        /// <value>
        /// The notification identifier.
        /// </value>
        [DataMember(Name = ManagementStrings.NotificationId, IsRequired = false, Order = 1000, EmitDefaultValue = false)]
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location URI.
        /// </value>
        [DataMember(Name = ManagementStrings.Location, IsRequired = false, Order = 1002, EmitDefaultValue = false)]
        public Uri Location { get; set; }

        [DataMember(Name = ManagementStrings.State, IsRequired = false, Order = 1003, EmitDefaultValue = true)]
        string NotificationState
        {
            get { return this.State.ToString(); }
            set
            {
                NotificationOutcomeState state;
                this.State = Enum.TryParse(value, true, out state) ? state : NotificationOutcomeState.Unknown;
            }
        }

        /// <summary>
        /// Gets or sets the notification state.
        /// </summary>
        /// <value>
        /// The notification state.
        /// </value>
        public NotificationOutcomeState State { get; set; }

        /// <summary>
        /// Gets or sets the notification enqueue time.
        /// </summary>
        /// <value>
        /// The notification enqueue time.
        /// </value>
        [DataMember(Name = ManagementStrings.EnqueueTime, IsRequired = false, Order = 1004, EmitDefaultValue = false)]
        public DateTime? EnqueueTime { get; set; }

        /// <summary>
        /// Gets or sets the notification start time.
        /// </summary>
        /// <value>
        /// The notification start time.
        /// </value>
        [DataMember(Name = ManagementStrings.StartTime, IsRequired = false, Order = 1005, EmitDefaultValue = false)]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the notification end time.
        /// </summary>
        /// <value>
        /// The notification end time.
        /// </value>
        [DataMember(Name = ManagementStrings.EndTime, IsRequired = false, Order = 1006, EmitDefaultValue = false)]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the notification body.
        /// </summary>
        /// <value>
        /// The notification body.
        /// </value>
        [DataMember(Name = ManagementStrings.NotificationBody, IsRequired = false, Order = 1007, EmitDefaultValue = false)]
        public string NotificationBody { get; set; }

        /// <summary>
        /// Gets or sets the notification tags.
        /// </summary>
        /// <value>
        /// The notification tags.
        /// </value>
        [DataMember(Name = ManagementStrings.Tags, IsRequired = false, Order = 1008, EmitDefaultValue = false)]
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the notification target platforms.
        /// </summary>
        /// <value>
        /// The notification target platforms.
        /// </value>
        [DataMember(Name = ManagementStrings.TargetPlatforms, IsRequired = false, Order = 1009, EmitDefaultValue = false)]
        public string TargetPlatforms { get; set; }

        /// <summary>
        /// Gets or sets the notification apns outcome counts.
        /// </summary>
        /// <value>
        /// The notification apns outcome counts.
        /// </value>
        [DataMember(Name = ManagementStrings.ApnsOutcomeCounts, IsRequired = false, Order = 1010, EmitDefaultValue = false)]
        public NotificationOutcomeCollection ApnsOutcomeCounts { get; set; }

        /// <summary>
        /// Gets or sets the notification MPNS outcome counts.
        /// </summary>
        /// <value>
        /// The notification MPNS outcome counts.
        /// </value>
        [DataMember(Name = ManagementStrings.MpnsOutcomeCounts, IsRequired = false, Order = 1011, EmitDefaultValue = false)]
        public NotificationOutcomeCollection MpnsOutcomeCounts { get; set; }

        /// <summary>
        /// Gets or sets the notification WNS outcome counts.
        /// </summary>
        /// <value>
        /// The notification WNS outcome counts.
        /// </value>
        [DataMember(Name = ManagementStrings.WnsOutcomeCounts, IsRequired = false, Order = 1012, EmitDefaultValue = false)]
        public NotificationOutcomeCollection WnsOutcomeCounts { get; set; }

        /// <summary>
        /// Gets or sets the notification GCM outcome counts.
        /// </summary>
        /// <value>
        /// The notification GCM outcome counts.
        /// </value>
        [DataMember(Name = ManagementStrings.GcmOutcomeCounts, IsRequired = false, Order = 1013, EmitDefaultValue = false)]
        public NotificationOutcomeCollection GcmOutcomeCounts { get; set; }

        /// <summary>
        /// Gets or sets the notification FCM outcome counts.
        /// </summary>
        /// <value>
        /// The notification FCM outcome counts.
        /// </value>
        [DataMember(Name = ManagementStrings.FcmOutcomeCounts, IsRequired = false, Order = 1016, EmitDefaultValue = false)]
        public NotificationOutcomeCollection FcmOutcomeCounts { get; set; }

        /// <summary>
        /// Gets or sets the notification ADM outcome counts.
        /// </summary>
        /// <value>
        /// The notification ADM outcome counts.
        /// </value>
        [DataMember(Name = ManagementStrings.AdmOutcomeCounts, IsRequired = false, Order = 1014, EmitDefaultValue = false)]
        public NotificationOutcomeCollection AdmOutcomeCounts { get; set; }

        /// <summary>
        /// Gets the URI to blob containing errors returned by PNSes.
        /// </summary>
        /// <value>
        /// The blob URI containing error details from PNSes
        /// </value>
        [DataMember(Name = ManagementStrings.PnsErrorDetailsUri, IsRequired = false, Order = 1015, EmitDefaultValue = false)]
        public string PnsErrorDetailsUri { get; set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <value>
        /// Information describing the extension.
        /// </value>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
