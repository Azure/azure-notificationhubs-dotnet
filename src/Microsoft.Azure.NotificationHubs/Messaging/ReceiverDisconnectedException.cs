//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This exception is thrown if two or more EventHubReceiver connect
    /// to the same partition with different epoch values.
    /// </summary>
    [Serializable]
    public sealed class ReceiverDisconnectedException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.ReceiverDisconnectedException"/> class with the specified exception message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ReceiverDisconnectedException(string message)
             : base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.ReceiverDisconnectedException"/> class with the specified exception message and inner exception text.
        /// </summary>
        /// <param name="message">The exception message.</param><param name="innerException">The inner exception text.</param>
        public ReceiverDisconnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.IsTransient = false;
        }

        internal ReceiverDisconnectedException(MessagingExceptionDetail detail) :
            base(detail)
        {
            this.IsTransient = false;
        }

        internal ReceiverDisconnectedException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, innerException)
        {
            this.IsTransient = false;
        }

        ReceiverDisconnectedException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
