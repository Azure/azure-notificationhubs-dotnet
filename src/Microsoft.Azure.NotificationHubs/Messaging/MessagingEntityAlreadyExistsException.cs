//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs;  

    /// <summary> Exception for signalling messaging entity already exists errors. </summary>
    [Serializable]
    public sealed class MessagingEntityAlreadyExistsException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingEntityAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        public MessagingEntityAlreadyExistsException(string entityName)
            : this(MessagingExceptionDetail.EntityConflict(string.Format(SRClient.MessagingEntityAlreadyExists, entityName)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingEntityAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The string exception message.</param>
        /// <param name="innerException">The inner exception to be propagated with this exception to the caller..</param>
        public MessagingEntityAlreadyExistsException(string message, Exception innerException)
            : base(MessagingExceptionDetail.EntityConflict(message), false, innerException)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal MessagingEntityAlreadyExistsException(MessagingExceptionDetail detail) :
            base(detail, false)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="innerException"> The inner exception. </param>
        internal MessagingEntityAlreadyExistsException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, false, innerException)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        MessagingEntityAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
