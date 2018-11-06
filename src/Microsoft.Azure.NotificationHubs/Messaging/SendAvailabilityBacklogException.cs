//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;


    /// <summary>
    /// Represents the exception occurred during the sending of availability backlogs.
    /// </summary>
    [Serializable]
    public class SendAvailabilityBacklogException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.SendAvailabilityBacklogException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        public SendAvailabilityBacklogException(string message) : base(message)
        {
            
        }
    }
}
