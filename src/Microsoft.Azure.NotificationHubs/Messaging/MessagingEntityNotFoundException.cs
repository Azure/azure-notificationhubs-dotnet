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

    /// <summary> Exception for signalling messaging entity not found errors. </summary>
    [Serializable]
    public sealed class MessagingEntityNotFoundException : MessagingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingEntityNotFoundException"/> class.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        public MessagingEntityNotFoundException(string entityName)
            : this(MessagingExceptionDetail.EntityNotFound(SRClient.MessagingEntityCouldNotBeFound(entityName)), null)
        {
            this.IsTransient = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingEntityNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The string exception message.</param>
        /// <param name="innerException">The inner exception to be propagated with this exception to the caller..</param>
        public MessagingEntityNotFoundException(string message, Exception innerException)
            : base(MessagingExceptionDetail.EntityNotFound(message), innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="trackingContext"> The TrackingContext. </param>
        internal MessagingEntityNotFoundException(MessagingExceptionDetail detail) :
            base(detail)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="detail"> Detail about the cause of the exception. </param>
        /// <param name="trackingContext"> The TrackingContext. </param>
        /// <param name="innerException"> The inner exception. </param>
        internal MessagingEntityNotFoundException(MessagingExceptionDetail detail, Exception innerException) :
            base(detail, innerException)
        {
            this.IsTransient = false;
        }

        /// <summary> Constructor. </summary>
        /// <param name="info">    The information. </param>
        /// <param name="context"> The context. </param>
        MessagingEntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsTransient = false;
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