//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;

    /// <summary>
    /// The exception that is thrown when subscription matching resulted no match.
    /// </summary>
    [Serializable]
    public sealed class NoMatchingSubscriptionException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.NoMatchingSubscriptionException"/> class with error message.
        /// </summary>
        /// <param name="message">The error message about the exception.</param>
        public NoMatchingSubscriptionException(string message)
            : base(message)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.NoMatchingSubscriptionException"/> class with error message and inner exception.
        /// </summary>
        /// <param name="message">The error message about the exception.</param><param name="innerException">The inner exception that is the cause of the current exception.</param>
        public NoMatchingSubscriptionException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Returns the string representation of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.NoMatchingSubscriptionException"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The string representation of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.NoMatchingSubscriptionException"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Message;
        }
    }
}