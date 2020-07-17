//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs;

    [Serializable]
    sealed class MessagingEntityMovedException : MessagingException
    {
        public MessagingEntityMovedException(string entityName)
            : base(string.Format(SRClient.MessagingEntityMoved, entityName), null)
        {
            this.IsTransient = false;
        }

        public MessagingEntityMovedException(string mesage, Exception innerException)
            : base(mesage, innerException)
        {
            this.IsTransient = false;
        }
        
        internal MessagingEntityMovedException(MessagingExceptionDetail detail) :
            base(detail)
        {
            this.IsTransient = false;
        }

        internal MessagingEntityMovedException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, innerException)
        {
            this.IsTransient = false;
        }

        MessagingEntityMovedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsTransient = false;
        }
    }
}
