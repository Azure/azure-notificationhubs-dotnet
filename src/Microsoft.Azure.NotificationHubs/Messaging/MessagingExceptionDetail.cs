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
    }
}
