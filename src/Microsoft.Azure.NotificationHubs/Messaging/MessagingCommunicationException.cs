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

    /// <summary>
    /// Exception for signaling general communication errors related to messaging operations.
    /// </summary>
    [Serializable]
    public sealed class MessagingCommunicationException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommunicationException"/> class.
        /// </summary>
        /// <param name="communicationPath">Name of the entity.</param>
        public MessagingCommunicationException(string communicationPath)
            : this(SRClient.MessagingEndpointCommunicationError(communicationPath), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommunicationException"/> class.
        /// </summary>
        /// <param name="message">The string exception message.</param>
        /// <param name="innerException">The inner exception to be propagated with this exception to the caller..</param>
        public MessagingCommunicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        MessagingCommunicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}