﻿//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown when a server is busy.
    /// </summary>
    [Serializable]
    public sealed class ServerBusyException : MessagingException
    {
        private static readonly TimeSpan ServerBusyBaseSleepTime = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerBusyException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ServerBusyException(string message)
            : this(MessagingExceptionDetail.ServerBusy(message), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerBusyException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServerBusyException(string message, Exception innerException)
            : this(MessagingExceptionDetail.ServerBusy(message), innerException)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="innerException">The inner exception.</param>
        internal ServerBusyException(MessagingExceptionDetail detail, Exception innerException)
            : base(detail, true, innerException)
        {
            RetryAfter = ServerBusyBaseSleepTime;
        }

        ServerBusyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
