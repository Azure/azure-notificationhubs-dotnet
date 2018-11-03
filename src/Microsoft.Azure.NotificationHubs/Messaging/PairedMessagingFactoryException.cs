//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;

    /// <summary>
    /// Represents the exception occurred for the paired messaging factory.
    /// </summary>
    [Serializable]
    public class PairedMessagingFactoryException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.PairedMessagingFactoryException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public PairedMessagingFactoryException(string message)
            : base(message)
        {
            this.IsTransient = false;
        }
    }
}
