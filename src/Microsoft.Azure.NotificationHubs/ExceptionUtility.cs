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
            if(ex is XmlException)
            {
                return HandleXmlException((XmlException)ex);
            }

            if(ex is HttpRequestException)
            {
                if (ex.InnerException is WebException)
                {
                    return HandleWebException((WebException)ex.InnerException, timeoutInMilliseconds, trackingId);
                }
                else
                {
                    return HandleUnexpectedException(ex, trackingId);
                }
            }

            return HandleUnexpectedException(ex, trackingId);
        }
        public static async Task<Exception> TranslateToMessagingExceptionAsync(this HttpResponseMessage response, string method, int timeoutInMilliseconds, string trackingId)
        {
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var exceptionMessage = FormatExceptionMessage(responseBody, response.StatusCode, response.ReasonPhrase, trackingId);
            var code = response.StatusCode;

            if (code == HttpStatusCode.NotFound || code == HttpStatusCode.NoContent)
            {
                return new MessagingEntityNotFoundException(exceptionMessage);
            }
            else if (code == HttpStatusCode.Conflict)
            {
                if (method.Equals(ManagementStrings.DeleteMethod))
                {
                    return new MessagingException(exceptionMessage);
                }
                if (method.Equals(ManagementStrings.PutMethod))
                {
                    return new MessagingException(exceptionMessage);
                }
                else if (exceptionMessage.Contains(ConflictOperationInProgressSubCode))
                {
                    return new MessagingException(exceptionMessage);
                }
                else
                {
                    return new MessagingEntityAlreadyExistsException(exceptionMessage, null);
                }
            }
            else if (code == HttpStatusCode.Unauthorized)
            {
                return new UnauthorizedAccessException(exceptionMessage);
            }
            else if (code == HttpStatusCode.Forbidden)
            {
                // TODO: It is not always correct to assume Forbidden
                // equals QuotaExceeded, but Gateway currently has no additional information
                // for us to make better judgment.
                return new QuotaExceededException(exceptionMessage);
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
                // mainly a test hook, but also a valid contract by itself
                return new MessagingCommunicationException(exceptionMessage);
            }

            return new MessagingException(exceptionMessage);
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
                exceptionMessage = SRClient.TrackableHttpExceptionMessageFormat((int)code, code.ToString(), reasonPhrase, CreateClientTrackingExceptionInfo(trackingId));
            }

            return exceptionMessage;
        }

        public static Exception HandleWebException(WebException webException, int timeoutInMilliseconds, string trackingId)
        {
            var webResponse = (HttpWebResponse)webException.Response;
            var exceptionMessage = webException.Message;


            if (webResponse == null)
            {
                switch (webException.Status)
                {
                    case WebExceptionStatus.RequestCanceled:
                    case WebExceptionStatus.Timeout:
                        exceptionMessage = SRClient.TrackableExceptionMessageFormat(SRClient.OperationRequestTimedOut(timeoutInMilliseconds), CreateClientTrackingExceptionInfo(trackingId));
                        return new TimeoutException(exceptionMessage, webException);

                    case WebExceptionStatus.ConnectFailure:
                    case WebExceptionStatus.NameResolutionFailure:
                        exceptionMessage = SRClient.TrackableExceptionMessageFormat(exceptionMessage, CreateClientTrackingExceptionInfo(trackingId));
                        return new MessagingCommunicationException(exceptionMessage, webException);
                }
            }
            else
            {
                throw webException;
            }

            return new MessagingException(exceptionMessage, webException);
        }

        public static Exception HandleUnexpectedException(Exception ex, string trackingId)
        {
            throw new MessagingException($"Unexpected exception encountered {CreateClientTrackingExceptionInfo(trackingId)}", ex);
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
