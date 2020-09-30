//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents the scheduled <see cref="T:Microsoft.Azure.NotificationHubs.Notification"/>.
    /// </summary>
    [DataContract(Name = ManagementStrings.ScheduledNotification, Namespace = ManagementStrings.Namespace)]    
    public class ScheduledNotification
    {
        /// <summary>
        /// Gets or sets the payload of this scheduled notification.
        /// </summary>
        /// 
        /// <returns>
        /// The payload of this scheduled notification.
        /// </returns>
        [DataMember(Name = ManagementStrings.ScheduledNotificationPayload, IsRequired = true, Order = 1001, EmitDefaultValue = true)]
        public Notification Payload { get; internal set; }

        /// <summary>
        /// Gets or sets the notification identifier.
        /// </summary>
        /// 
        /// <returns>
        /// The notification identifier.
        /// </returns>
        [DataMember(Name = ManagementStrings.ScheduledNotificationId, IsRequired = true, Order = 1002, EmitDefaultValue = true)]        
        public string ScheduledNotificationId { get; internal set; }

        /// <summary>
        /// Gets or sets the scheduled time.
        /// </summary>
        /// 
        /// <returns>
        /// The scheduled time.
        /// </returns>
        [DataMember(Name = ManagementStrings.ScheduledNotificationTime, IsRequired = true, Order = 1003, EmitDefaultValue = true)]                
        public DateTimeOffset ScheduledTime { get; internal set; }

        /// <summary>
        /// Gets or sets the notification tag.
        /// </summary>
        /// 
        /// <returns>
        /// The notification tag.
        /// </returns>
        [DataMember(Name = ManagementStrings.ScheduledNotificationTags, IsRequired = true, Order = 1004, EmitDefaultValue = true)]
        public string Tags { get; internal set; }

        /// <summary>
        /// Gets or sets the tracking identifier.
        /// </summary>
        /// 
        /// <returns>
        /// The tracking identifier.
        /// </returns>
        [DataMember(Name = ManagementStrings.TrackingId, IsRequired = true, Order = 1005, EmitDefaultValue = true)]
        public string TrackingId { get; internal set; }
    }
}