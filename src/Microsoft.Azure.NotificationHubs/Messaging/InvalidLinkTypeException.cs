//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;

    [Serializable]
    sealed class InvalidLinkTypeException : MessagingException
    {
        public InvalidLinkTypeException(string message, Exception innerException) :
            base(message, innerException)
        {
            this.IsTransient = false;
        }
    }
}
