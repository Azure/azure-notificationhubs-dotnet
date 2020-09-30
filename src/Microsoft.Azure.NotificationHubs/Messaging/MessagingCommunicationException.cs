//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary>
    /// Exception for signaling general communication errors related to messaging operations.
    /// </summary>
    [Serializable]
    public sealed class MessagingCommunicationException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommunicationException"/> class.
        /// </summary>
        /// <param name="message">The string exception message.</param>
        /// <param name="innerException">The inner exception to be propagated with this exception to the caller..</param>
        /// <param name="isTransientError">If set to <c>true</c>, indicates it is a transient error.</param>
        public MessagingCommunicationException(MessagingExceptionDetail message, bool isTransientError, Exception innerException)
            : base(message, isTransientError, innerException)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        MessagingCommunicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
