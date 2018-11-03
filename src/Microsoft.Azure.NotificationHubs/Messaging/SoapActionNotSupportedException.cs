//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;

    [Serializable]
    sealed class SoapActionNotSupportedException : MessagingException
    {
        public SoapActionNotSupportedException(string message, Exception exception) : base(message, exception)
        {
            this.IsTransient = false;
        }
    }
}
