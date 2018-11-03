//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Specifies an enumeration of notification outcome state.
    /// </summary>
    public enum NotificationOutcomeState
    {
        /// <summary>
        /// NotificationOutcome when notification is intially enqued 
        /// </summary>
        Enqueued = 0,

        /// <summary>
        /// NotificationOutcome state during DebugSend
        /// </summary>
        DetailedStateAvailable = 1,

        /// <summary>
        /// Processing sent notification
        /// </summary>
        Processing = 2,

        /// <summary>
        /// NotificationOutcome when the notification sent has been recevied 
        /// </summary>
        Completed = 3,

        /// <summary>
        /// NotificationOutcome when the notification sent has been Abondoned
        /// </summary>
        Abandoned = 4,

        /// <summary>
        /// Unknown state when State not set
        /// </summary>
        Unknown = 5 ,
        /// <summary>
        ///  NotificationOutcome when there are no targets found to send the message
        /// </summary>
        NoTargetFound = 6,
        /// <summary>
        /// NotificationOutcome when the user cancelled the scheduled send request
        /// </summary>
        Cancelled = 7
    }
}