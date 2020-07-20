//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using Microsoft.Azure.NotificationHubs.Messaging;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Xml;
    using static Microsoft.Azure.NotificationHubs.Messaging.MessagingExceptionDetail;

    internal static class ExceptionsUtility
    {
        private static readonly string ConflictOperationInProgressSubCode =
            string.Format(CultureInfo.InvariantCulture, Constants.HttpErrorSubCodeFormatString, ExceptionErrorCodes.ConflictOperationInProgress.ToString("D"));

        public const string RootTag = "Error";
        public const string HttpStatusCodeTag = "Code";
        public const string DetailTag = "Detail";

        public static Exception HandleXmlException(XmlException exception, string trackingId)
        {
            var details = new MessagingExceptionDetail(ExceptionErrorCodes.InternalFailure, SRClient.InvalidXmlFormat, ErrorLevelType.ServerError, null, trackingId);
            return new MessagingException(details, false, exception);
        }

        public static async Task<Exception> TranslateToMessagingExceptionAsync(this HttpResponseMessage response, string trackingId)
        {
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var exceptionMessage = FormatExceptionMessage(responseBody, response.StatusCode, response.ReasonPhrase, trackingId);
            var statusCode = response.StatusCode;
            var retryAfter = response.Headers.RetryAfter?.Delta;
            
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                case HttpStatusCode.NoContent:
                    return new MessagingEntityNotFoundException(new MessagingExceptionDetail(ExceptionErrorCodes.EndpointNotFound, exceptionMessage, ErrorLevelType.UserError, statusCode, trackingId));
                case HttpStatusCode.Conflict:
                    if (response.RequestMessage.Method.Equals(HttpMethod.Delete) ||
                        response.RequestMessage.Method.Equals(HttpMethod.Put) ||
                        exceptionMessage.Contains(ConflictOperationInProgressSubCode))
                    {
                        return new MessagingException(new MessagingExceptionDetail(ExceptionErrorCodes.ConflictGeneric, exceptionMessage, ErrorLevelType.UserError, statusCode, trackingId), false);
                    }
                    else
                    {
                        return new MessagingEntityAlreadyExistsException(new MessagingExceptionDetail(ExceptionErrorCodes.ConflictGeneric, exceptionMessage, ErrorLevelType.UserError, statusCode, trackingId));
                    }

                case HttpStatusCode.Unauthorized:
                    return new UnauthorizedException(new MessagingExceptionDetail(ExceptionErrorCodes.UnauthorizedGeneric, exceptionMessage, ErrorLevelType.UserError, statusCode, trackingId));
                case HttpStatusCode.Forbidden:
                case (HttpStatusCode)429:
                    return new QuotaExceededException(new MessagingExceptionDetail(ExceptionErrorCodes.Throttled, exceptionMessage, ErrorLevelType.UserError, statusCode, trackingId), retryAfter);
                case HttpStatusCode.BadRequest:
                    return new BadRequestException(new MessagingExceptionDetail(ExceptionErrorCodes.BadRequest, exceptionMessage, ErrorLevelType.UserError, statusCode, trackingId));
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.RequestTimeout:
                    return new ServerBusyException(new MessagingExceptionDetail(ExceptionErrorCodes.ServerBusy, exceptionMessage, ErrorLevelType.ServerError, statusCode, trackingId));
            }

            return new MessagingException(new MessagingExceptionDetail(ExceptionErrorCodes.UnknownExceptionDetail, exceptionMessage, ErrorLevelType.ServerError, statusCode, trackingId), false);
        }

        internal static string FormatExceptionMessage(string responseBody, HttpStatusCode code, string reasonPhrase, string trackingId)
        {
            var exceptionMessage = string.Empty;
            using(var stringReader = new StringReader(responseBody))
            using (var reader = XmlReader.Create(stringReader))
            {
                try
                {
                    reader.Read();
                    reader.ReadStartElement(RootTag);

                    reader.ReadStartElement(HttpStatusCodeTag);
                    reader.ReadString();
                    reader.ReadEndElement();

                    reader.ReadStartElement(DetailTag);
                    exceptionMessage = string.Format(CultureInfo.InvariantCulture, "{0} {1}", exceptionMessage, reader.ReadString());
                }
                catch (XmlException ex)
                {
                    throw HandleXmlException(ex, trackingId);
                }
            }

            if (string.IsNullOrEmpty(exceptionMessage))
            {
                exceptionMessage = string.Format(SRClient.TrackableHttpExceptionMessageFormat, (int)code, code.ToString(), reasonPhrase, CreateClientTrackingExceptionInfo(trackingId));
            }

            return exceptionMessage;
        }

        public static Exception HandleSocketException(SocketException socketException, int timeoutInMilliseconds, string trackingId)
        {
            var exceptionMessage = socketException.Message;

            switch (socketException.SocketErrorCode)
            {
                case SocketError.AddressNotAvailable:
                case SocketError.ConnectionRefused:
                case SocketError.AccessDenied:
                case SocketError.HostUnreachable:
                case SocketError.HostNotFound:
                    exceptionMessage = string.Format(SRClient.TrackableExceptionMessageFormat, exceptionMessage, CreateClientTrackingExceptionInfo(trackingId));
                    return new MessagingCommunicationException(new MessagingExceptionDetail(ExceptionErrorCodes.ProviderUnreachable, exceptionMessage, ErrorLevelType.ClientConnection, null, trackingId), false, socketException);
                default:
                    exceptionMessage = string.Format(SRClient.TrackableExceptionMessageFormat, string.Format(SRClient.OperationRequestTimedOut, timeoutInMilliseconds), CreateClientTrackingExceptionInfo(trackingId));
                    return new MessagingCommunicationException(new MessagingExceptionDetail(ExceptionErrorCodes.GatewayTimeoutFailure, exceptionMessage, ErrorLevelType.ClientConnection, null, trackingId), true, socketException);
            }
        }

        public static Exception HandleUnexpectedException(Exception ex, string trackingId)
        {
            var exceptionMessage = $"Unexpected exception encountered {CreateClientTrackingExceptionInfo(trackingId)}";
            var details = new MessagingExceptionDetail(ExceptionErrorCodes.UnknownExceptionDetail, exceptionMessage, ErrorLevelType.ClientConnection, null, trackingId);
            throw new MessagingException(details, false, ex);
        }

        internal static string CreateClientTrackingExceptionInfo(string trackingId)
        {
            return $"TrackingId:{trackingId},TimeStamp:{DateTime.UtcNow:o}";
        }
    }
}
