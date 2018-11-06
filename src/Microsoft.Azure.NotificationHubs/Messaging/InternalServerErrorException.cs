//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using Microsoft.Azure.NotificationHubs;

    [Serializable]
    sealed class InternalServerErrorException : MessagingException
    {
        public InternalServerErrorException() : this((Exception)null)
        {
        }

        public InternalServerErrorException(string message)
            : base(message)
        {
            this.Initialize();
        }

        public InternalServerErrorException(Exception innerException)
            : base(SRClient.InternalServerError, true, innerException)
        {
            this.Initialize();
        }

        public InternalServerErrorException(MessagingExceptionDetail detail)
            : base(detail)
        {
            this.Initialize();
        }

        void Initialize()
        {
            this.IsTransient = true;
        }
    }
}
