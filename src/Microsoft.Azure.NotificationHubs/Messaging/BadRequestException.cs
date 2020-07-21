//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary> Exception for signaling bad request data errors. </summary>
    [Serializable]
    public class BadRequestException : MessagingException
    {
        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal BadRequestException(MessagingExceptionDetail detail) :
            base(detail, false)
        {            
        }

        /// <summary> Exception Constructor for additional details embedded in a serializable stream. </summary>
        /// <param name="info">    The serialization information object. </param>
        /// <param name="context"> The streaming context/source. </param>
        protected BadRequestException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
