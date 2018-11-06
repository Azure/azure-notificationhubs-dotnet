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
    /// Exception for signalling message size excess
    /// </summary>
    [Serializable]
    public sealed class MessageSizeExceededException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSizeExceededException"/> class.
        /// </summary>
        /// <param name="message">The string message that will be propagated to the caller..</param>
        public MessageSizeExceededException(string message) :
            base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSizeExceededException"/> class.
        /// </summary>
        /// <param name="message">The string exception message.</param>
        /// <param name="innerException">The inner exception to be propagated with this exception to the caller..</param>
        public MessageSizeExceededException(string message, Exception innerException) :
            base(message, innerException)
        {
            this.IsTransient = false;
        }

        MessageSizeExceededException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
