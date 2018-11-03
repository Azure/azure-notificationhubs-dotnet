//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary> Exception for signalling that a duplicate message was appended. </summary>
    [Serializable]
    public sealed class DuplicateMessageException : MessagingException
    {
        /// <summary> Constructor. </summary>
        /// <param name="message"> The message. </param>
        public DuplicateMessageException(string message) :
            base(message)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="message">        The message. </param>
        /// <param name="innerException"> The inner exception. </param>
        public DuplicateMessageException(string message, Exception innerException) :
            base(message, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal DuplicateMessageException(MessagingExceptionDetail detail) :
            base(detail)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="innerException"> The inner exception. </param>
        internal DuplicateMessageException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        DuplicateMessageException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
