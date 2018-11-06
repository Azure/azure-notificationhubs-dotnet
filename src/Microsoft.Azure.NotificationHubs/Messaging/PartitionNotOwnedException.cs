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
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary> Exception for signalling partition not owned errors. </summary>
    [Serializable]
    public sealed class PartitionNotOwnedException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionNotOwnedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public PartitionNotOwnedException(string message)
            : base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionNotOwnedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PartitionNotOwnedException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionNotOwnedException"/> class.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        PartitionNotOwnedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsTransient = false;
        }
    }
}

