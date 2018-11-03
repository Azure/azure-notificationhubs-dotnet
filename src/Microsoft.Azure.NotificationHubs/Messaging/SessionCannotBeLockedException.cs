//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class SessionCannotBeLockedException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionCannotBeLockedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SessionCannotBeLockedException(string message)
            : base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionCannotBeLockedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SessionCannotBeLockedException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.IsTransient = false;
        }

        SessionCannotBeLockedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsTransient = false;
        }
    }
}