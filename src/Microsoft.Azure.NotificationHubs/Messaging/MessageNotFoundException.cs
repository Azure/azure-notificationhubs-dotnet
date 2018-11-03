//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary> Exception for signalling message not found errors. </summary>
    [Serializable]
    public sealed class MessageNotFoundException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The string message that will be propagated to the caller..</param>
        public MessageNotFoundException(string message) :
            base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The string exception message.</param>
        /// <param name="innerException">The inner exception to be propagated with this exception to the caller..</param>
        public MessageNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Holds all the data needed to serialize or deserialize an object.</param>
        /// <param name="context">The stream context providing exception details.</param>
        MessageNotFoundException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
