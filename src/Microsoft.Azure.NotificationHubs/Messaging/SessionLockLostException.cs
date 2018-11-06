//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary> Exception for signalling session lock lost errors. </summary>
    [Serializable]
    public sealed class SessionLockLostException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionLockLostException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SessionLockLostException(string message) :
            base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionLockLostException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SessionLockLostException(string message, Exception innerException) :
            base(message, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        SessionLockLostException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            this.IsTransient = false;
        }
    }
}