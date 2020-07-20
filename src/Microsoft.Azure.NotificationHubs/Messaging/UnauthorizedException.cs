//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary> Exception for signaling authorization errors. </summary>
    [Serializable]
    public class UnauthorizedException : MessagingException
    {
        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal UnauthorizedException(MessagingExceptionDetail detail) :
            base(detail, false)
        {            
        }

        /// <summary> Exception Constructor for additional details embedded in a serializable stream. </summary>
        /// <param name="info">    The serialization information object. </param>
        /// <param name="context"> The streaming context/source. </param>
        protected UnauthorizedException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
