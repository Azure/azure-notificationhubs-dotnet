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
    /// The exception that is thrown when a server is busy.
    /// </summary>
    [Serializable]
    public sealed class ServerBusyException : MessagingException
    {
        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal ServerBusyException(MessagingExceptionDetail detail)
            : base(detail, true)
        {
        }

        ServerBusyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
