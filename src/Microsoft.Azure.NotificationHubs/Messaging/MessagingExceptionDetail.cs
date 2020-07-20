//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Net;

    /// <summary>
    /// Details about the cause of a Messaging Exception that map errors to specific exceptions.
    /// </summary>
    [Serializable]
    public sealed class MessagingExceptionDetail
    {
        internal MessagingExceptionDetail(ExceptionErrorCodes errorCode, string message, ErrorLevelType errorLevel, HttpStatusCode? httpStatusCode, string trackingId)
        {
            this.ErrorCode = (int)errorCode;
            this.Message = message;
            this.ErrorLevel = errorLevel;
            this.HttpStatusCode = httpStatusCode;
            this.TrackingId = trackingId;
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

            /// <summary>
            /// Error related to client connectivity
            /// </summary>
            ClientConnection
        }

        /// <summary>
        /// A machine-readable subcode that gives more detail about the cause of this error.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Http status code of the response.
        /// </summary>
        public HttpStatusCode? HttpStatusCode { get; private set; }

        /// <summary>
        /// Tracking ID of the request.
        /// </summary>
        public string TrackingId { get; private set; }

        /// <summary>
        /// A human-readable message that gives more detail about the cause of this error.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// An enumerated value indicating the type of error.
        /// </summary>
        public ErrorLevelType ErrorLevel { get; private set; }
    }
}
