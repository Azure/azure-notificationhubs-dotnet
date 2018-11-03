//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary> Exception for signaling quota exceeded errors. </summary>
    [Serializable]
    public class QuotaExceededException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaExceededException"/> class.
        /// </summary>
        /// <param name="message">The exception message included with the base exception.</param>
        public QuotaExceededException(string message) :
            base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaExceededException"/> class.
        /// </summary>
        /// <param name="message">The exception message included with the base exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public QuotaExceededException(string message, Exception innerException) :
            base(message, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal QuotaExceededException(MessagingExceptionDetail detail) :
            base(detail)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="innerException"> The inner exception. </param>
        internal QuotaExceededException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Exception Constructor for additional details embedded in a serializable stream. </summary>
        /// <param name="info">    The serialization information object. </param>
        /// <param name="context"> The streaming context/source. </param>
        protected QuotaExceededException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
