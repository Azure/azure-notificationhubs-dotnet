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

    internal static class ExceptionsUtility
    {
        private static readonly string ConflictOperationInProgressSubCode =
            string.Format(CultureInfo.InvariantCulture, Constants.HttpErrorSubCodeFormatString, ExceptionErrorCodes.ConflictOperationInProgress.ToString("D"));

        public const string RootTag = "Error";
        public const string HttpStatusCodeTag = "Code";
        public const string DetailTag = "Detail";

        public static Exception HandleXmlException(XmlException exception)
        {
            return new MessagingException(SRClient.InvalidXmlFormat, false, exception);
        }

        public static Exception TranslateToMessagingException(this Exception ex, int timeoutInMilliseconds = 0, string trackingId = null)
        {
            if (ex is XmlException)
            {
                return HandleXmlException((XmlException)ex);
            }

            if (ex is HttpRequestException)
            {
                var innerException = ex.GetBaseException();
                if (innerException is SocketException socketException)
                {
                    return HandleSocketException(socketException, timeoutInMilliseconds, trackingId);
                }
                else
                {
                    return HandleUnexpectedException(ex, trackingId);
                }
            }

            if (ex is TimeoutException)
            {
                return new MessagingCommunicationException(ex.Message, true, ex);
            }

            if (ex is OperationCanceledException)
            {
                return new MessagingCommunicationException(ex.Message, true, ex);
            }

            return HandleUnexpectedException(ex, trackingId);
        }
        public static async Task<Exception> TranslateToMessagingExceptionAsync(this HttpResponseMessage response, string method, int timeoutInMilliseconds, string trackingId)
        {
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var exceptionMessage = FormatExceptionMessage(responseBody, response.StatusCode, response.ReasonPhrase, trackingId);
            var code = response.StatusCode;
            var retryAfter = response.Headers.RetryAfter.Delta;

            if (code == HttpStatusCode.NotFound || code == HttpStatusCode.NoContent)
            {
                return new MessagingEntityNotFoundException(exceptionMessage);
            }
            else if (code == HttpStatusCode.Conflict)
            {
                if (method.Equals(HttpMethod.Delete.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return new MessagingException(exceptionMessage, false);
                }
                if (method.Equals(HttpMethod.Put.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return new MessagingException(exceptionMessage, false);
                }
                else if (exceptionMessage.Contains(ConflictOperationInProgressSubCode))
                {
                    return new MessagingException(exceptionMessage, false);
                }
                else
                {
                    return new MessagingEntityAlreadyExistsException(exceptionMessage);
                }
            }
            else if (code == HttpStatusCode.Unauthorized)
            {
                return new UnauthorizedAccessException(exceptionMessage);
            }
            else if (code == HttpStatusCode.Forbidden || (int)code == 429)
            {
                // TODO: It is not always correct to assume Forbidden
                // equals QuotaExceeded, but Gateway currently has no additional information
                // for us to make better judgment.
                return new QuotaExceededException(MessagingExceptionDetail.UnknownDetail(exceptionMessage), retryAfter, null);
            }
            else if (code == HttpStatusCode.BadRequest)
            {
                return new ArgumentException(exceptionMessage);
            }
            else if (code == HttpStatusCode.ServiceUnavailable)
            {
                return new ServerBusyException(exceptionMessage);
            }
            else if (code == HttpStatusCode.GatewayTimeout)
            {
                return new ServerBusyException(exceptionMessage);
            }

            return new MessagingException(exceptionMessage, false);
        }

        private static string FormatExceptionMessage(string responseBody, HttpStatusCode code, string reasonPhrase, string trackingId)
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
                catch (XmlException)
                {
                    //Ignore this exception
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
                    return new MessagingCommunicationException(exceptionMessage, false, socketException);
                default:
                    exceptionMessage = string.Format(SRClient.TrackableExceptionMessageFormat, string.Format(SRClient.OperationRequestTimedOut, timeoutInMilliseconds), CreateClientTrackingExceptionInfo(trackingId));
                    return new MessagingCommunicationException(exceptionMessage, true, socketException);
            }
        }

        public static Exception HandleUnexpectedException(Exception ex, string trackingId)
        {
            throw new MessagingException($"Unexpected exception encountered {CreateClientTrackingExceptionInfo(trackingId)}", false, ex);
        }

        public static bool IsMessagingException(this Exception e)
        {
            return (e is MessagingException || e is UnauthorizedAccessException || e is ArgumentException);
        }

        internal static string CreateClientTrackingExceptionInfo(string trackingId)
        {
            return $"TrackingId:{trackingId},TimeStamp:{DateTime.UtcNow:o}";
        }
    }
}
