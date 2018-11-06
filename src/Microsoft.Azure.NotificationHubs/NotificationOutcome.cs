//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using Microsoft.Azure.NotificationHubs.Messaging;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a notification outcome.
    /// </summary>
    [DataContract(Name = ManagementStrings.NotificationOutcome, Namespace = ManagementStrings.Namespace)]
    public sealed class NotificationOutcome
    {
        internal bool DelayedRetry { get; set; }

        internal bool ReleaseSession { get; set; }

        /// <summary>
        /// Gets the state of this notification outcome.
        /// </summary>
        /// 
        /// <returns>
        /// The state of this notification outcome.
        /// </returns>
        public NotificationOutcomeState State { get; internal set; }

        /// <summary>
        /// Gets or sets the success value of the outcome.
        /// </summary>
        /// 
        /// <returns>
        /// The success value of the outcome.
        /// </returns>
        [DataMember(Name = ManagementStrings.Success, IsRequired = true, Order = 1001, EmitDefaultValue = true)]
        public long Success { get; set; }

        /// <summary>
        /// Gets or sets the failure value of the outcome.
        /// </summary>
        /// 
        /// <returns>
        /// The failure value of the outcome.
        /// </returns>
        [DataMember(Name = ManagementStrings.Failure, IsRequired = true, Order = 1002, EmitDefaultValue = true)]
        public long Failure { get; set; }

        /// <summary>
        /// Gets or sets the list of notification outcome results.
        /// </summary>
        /// 
        /// <returns>
        /// The list of notification outcome results.
        /// </returns>
        [DataMember(Name = ManagementStrings.Results, IsRequired = true, Order = 1003, EmitDefaultValue = true)]
        public List<RegistrationResult> Results { get; set; }

        /// <summary>
        /// Gets or sets the notification ID.
        /// </summary>
        /// 
        /// <returns>
        /// notification ID.
        /// </returns>
        public string NotificationId { get; set; }

        /// <summary>
        /// Gets the tracking ID.
        /// </summary>
        /// 
        /// <returns>
        /// The tracking ID.
        /// </returns>
        public string TrackingId { get; internal set; }

        internal static NotificationOutcome GetUnknownOutCome()
        {
            RegistrationResult result = new RegistrationResult()
            {
                ApplicationPlatform = "Unknown",
                PnsHandle = "Unknown",
                RegistrationId = "Unknown",
                Outcome = "UnknownError"
            };

            return new NotificationOutcome()
            {
                Failure = 1,
                Success = 0,
                Results = new List<RegistrationResult>() { result }
            };
        }
    }
}