//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary> Exception for signalling message store lock lost errors. </summary>
    [Serializable]
    public sealed class MessageStoreLockLostException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSizeExceededException"/> class.
        /// </summary>
        /// <param name="message">The string message that will be propagated to the caller..</param>
        public MessageStoreLockLostException(string message) :
            base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageStoreLockLostException"/> class.
        /// </summary>
        /// <param name="message">The string exception message.</param>
        /// <param name="innerException">The inner exception to be propagated with this exception to the caller..</param>
        public MessageStoreLockLostException(string message, Exception innerException) :
            base(message, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="trackingContext"> The TrackingContext. </param>
        internal MessageStoreLockLostException(MessagingExceptionDetail detail) :
            base(detail)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="trackingContext"> The TrackingContext. </param>
        /// <param name="innerException"> The inner exception. </param>
        internal MessageStoreLockLostException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        MessageStoreLockLostException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
