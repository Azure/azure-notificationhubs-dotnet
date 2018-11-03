//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;
    
    /// <summary> Exception for signaling receive request quota exceeded errors. </summary>
    [Serializable]
    class RequestQuotaExceededException : QuotaExceededException
    {
        /// <summary> Constructor. </summary>
        /// <param name="message"> The exception message included with the base exception. </param>
        public RequestQuotaExceededException(string message) :
            base(message)
        {
        }

        /// <summary> Exception Constructor. </summary>
        /// <param name="message"> The exception message included with the base exception. </param>
        /// <param name="innerException"> The inner exception. </param>
        public RequestQuotaExceededException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal RequestQuotaExceededException(MessagingExceptionDetail detail) :
            base(detail)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="innerException"> The inner exception. </param>
        internal RequestQuotaExceededException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, innerException)
        {
        }

        /// <summary> Exception Constructor for additional details embedded in a serializable stream. </summary>
        /// <param name="info">    The serialization information object. </param>
        /// <param name="context"> The streaming context/source. </param>
        protected RequestQuotaExceededException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
