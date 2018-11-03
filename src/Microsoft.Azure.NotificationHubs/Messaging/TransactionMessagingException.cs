//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;

    [Serializable]
    sealed class TransactionMessagingException : MessagingException
    {
        public TransactionMessagingException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.Initialize();
        }

        public TransactionMessagingException(string message)
            : base(message)
        {
            this.Initialize();
        }

        void Initialize()
        {
            this.IsTransient = false;
        }
    }
}
