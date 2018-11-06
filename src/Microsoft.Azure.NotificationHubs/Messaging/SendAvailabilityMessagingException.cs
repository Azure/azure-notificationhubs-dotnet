//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using Microsoft.Azure.NotificationHubs;
    /// <summary>
    /// Represents the exceptions occurred during the sending the availability for the messaging.
    /// </summary>
    [Serializable]
    public class SendAvailabilityMessagingException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.SendAvailabilityMessagingException"/> class.
        /// </summary>
        /// <param name="innerException">The error that caused the exception.</param>
        public SendAvailabilityMessagingException(Exception innerException)
            : base(SRClient.PairedNamespacePrimaryEntityUnreachable, innerException)
        {
            this.IsTransient = false;
        }
    }
}
