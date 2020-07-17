//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using Microsoft.Azure.NotificationHubs;

    /// <summary> Exception for signalling messaging entity not found errors. </summary>
    [Serializable]
    public sealed class MessagingEntityDisabledException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingEntityDisabledException"/> class.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        public MessagingEntityDisabledException(string entityName)
            : this(SRClient.MessagingEntityIsDisabledException(entityName), null)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingEntityDisabledException"/> class.
        /// </summary>
        /// <param name="message">The string exception message.</param>
        /// <param name="innerException">The inner exception to be propagated with this exception to the caller..</param>
        public MessagingEntityDisabledException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal MessagingEntityDisabledException(MessagingExceptionDetail detail) :
            base(detail)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="innerException"> The inner exception. </param>
        internal MessagingEntityDisabledException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        MessagingEntityDisabledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
