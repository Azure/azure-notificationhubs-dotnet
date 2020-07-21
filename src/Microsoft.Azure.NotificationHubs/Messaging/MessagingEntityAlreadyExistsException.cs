//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary> Exception for signalling messaging entity already exists errors. </summary>
    [Serializable]
    public sealed class MessagingEntityAlreadyExistsException : MessagingException
    {
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
