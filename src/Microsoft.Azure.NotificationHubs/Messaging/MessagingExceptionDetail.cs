//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using Microsoft.Azure.NotificationHubs;

    /// <summary>
    /// Details about the cause of a Messaging Exception that map errors to specific exceptions.
    /// </summary>
    [Serializable]
    public sealed class MessagingExceptionDetail
    {
        private MessagingExceptionDetail(int errorCode, string message)
            : this(errorCode, message, ErrorLevelType.UserError)
        {
        }

        private MessagingExceptionDetail(int errorCode, string message, ErrorLevelType errorLevel)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
            this.ErrorLevel = errorLevel;
        }

        /// <summary>
        /// Error level type enum
        /// </summary>
        public enum ErrorLevelType
        {
            /// <summary>
            /// The user error
            /// </summary>
            UserError,

            /// <summary>
            /// The server error
            /// </summary>
            ServerError,
        }

        /// <summary>
        /// A machine-readable subcode that gives more detail about the cause of this error.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// A human-readable message that gives more detail about the cause of this error.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// An enumerated value indicating the type of error.
        /// </summary>
        public ErrorLevelType ErrorLevel { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with UnknownExceptionDetail error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail UnknownDetail(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.UnknownExceptionDetail, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with EntityGone error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail EntityGone(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.EntityGone, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with EndpointNotFound error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail EntityNotFound(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.EndpointNotFound, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with ConflictGeneric error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail EntityConflict(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.ConflictGeneric, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with ServerBusy error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail ServerBusy(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.ServerBusy, message, ErrorLevelType.ServerError);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with StoreLockLost error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail StoreLockLost(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.StoreLockLost, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with UnspecifiedInternalError error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail UnspecifiedInternalError(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.UnspecifiedInternalError, message, ErrorLevelType.ServerError);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with SqlFiltersExceeded error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail SqlFiltersExceeded(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.SqlFiltersExceeded, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with CorrelationFiltersExceeded error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail CorrelationFiltersExceeded(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.CorrelationFiltersExceeded, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with SubscriptionsExceeded error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail SubscriptionsExceeded(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.SubscriptionsExceeded, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail"/> class with EventHubAtFullCapacity error code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <returns>The exception class instance</returns>
        public static MessagingExceptionDetail EventHubAtFullCapacity(string message)
        {
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.EventHubAtFullCapacity, message, ErrorLevelType.ServerError);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail" /> class with UpdateConflict error code.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>
        /// The exception class instance
        /// </returns>
        public static MessagingExceptionDetail EntityUpdateConflict(string entityName)
        {
            string message = SRClient.MessagingEntityUpdateConflict(entityName);
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.UpdateConflict, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail" /> class with ConflictOperationInProgress error code.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns>
        /// The exception class instance
        /// </returns>
        public static MessagingExceptionDetail EntityConflictOperationInProgress(string entityName)
        {
            string message = SRClient.MessagingEntityRequestConflict(entityName);
            return new MessagingExceptionDetail((int)ExceptionErrorCodes.ConflictOperationInProgress, message);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessagingExceptionDetail" /> class with a custom error code.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <returns>
        /// The exception class instance
        /// </returns>
        public static MessagingExceptionDetail ReconstructExceptionDetail(int errorCode, string message, ErrorLevelType errorLevel)
        {
            return new MessagingExceptionDetail(errorCode, message, errorLevel);
        }
    }
}
