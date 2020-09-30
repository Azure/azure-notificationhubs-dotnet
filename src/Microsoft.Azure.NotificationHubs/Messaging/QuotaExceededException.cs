//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary> Exception for signaling quota exceeded errors. </summary>
    [Serializable]
    public class QuotaExceededException : MessagingException
    {
        internal readonly TimeSpan DefaultRetryTimeout = TimeSpan.FromSeconds(10);

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="retryAfter">Retry after value.</param>
        internal QuotaExceededException(MessagingExceptionDetail detail, TimeSpan? retryAfter) :
            base(detail, true)
        {
            RetryAfter = retryAfter ?? DefaultRetryTimeout;
        }

        /// <summary> Exception Constructor for additional details embedded in a serializable stream. </summary>
        /// <param name="info">    The serialization information object. </param>
        /// <param name="context"> The streaming context/source. </param>
        protected QuotaExceededException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
