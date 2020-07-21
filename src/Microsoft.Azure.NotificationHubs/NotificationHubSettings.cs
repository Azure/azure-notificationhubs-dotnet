// ----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
// ----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// Notification Hubs client settings
    /// <seealso cref="NotificationHubClient" />
    /// </summary>
    public class NotificationHubSettings
    {
        private NotificationHubRetryOptions _retryOptions = new NotificationHubRetryOptions();

        /// <summary>
        /// Gets or sets the proxy
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets http message handler. If set will override Proxy.
        /// </summary>
        public HttpMessageHandler MessageHandler { get; set; }

        /// <summary>
        /// Gets or sets HttpClient. If set will overwrite Proxy and MessageHandler.
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Gets or sets operation timeout of the HTTP operations.
        /// </summary>
        /// <value>
        ///   <c>Http operation timeout. Defaults to 60 seconds</c>.
        /// </value>
        /// <remarks>
        ///  </remarks>
        public TimeSpan? OperationTimeout { get; set; }

        /// <summary>
        /// The set of options to use for determining whether a failed operation should be retried and,
        /// if so, the amount of time to wait between retry attempts.  These options also control the
        /// amount of time allowed for receiving messages and other interactions with the Service Bus service.
        /// </summary>
        public NotificationHubRetryOptions RetryOptions
        {
            get => _retryOptions;
            set
            {
                _retryOptions = value ?? throw new ArgumentNullException(nameof(RetryOptions));
            }
        }
    }
}
