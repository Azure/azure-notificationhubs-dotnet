﻿//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary> Exception for signalling messaging entity not found errors. </summary>
    [Serializable]
    public sealed class MessagingEntityNotFoundException : MessagingException
    {
        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        internal MessagingEntityNotFoundException(MessagingExceptionDetail detail) :
            base(detail, false)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="innerException"> The inner exception. </param>
        internal MessagingEntityNotFoundException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, false, innerException)
        {
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        MessagingEntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public override string ToString()
        {
            return this.Message;
        }
    }
}
