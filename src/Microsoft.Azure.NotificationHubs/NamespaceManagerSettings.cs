//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Net.Http;
using Microsoft.Azure.NotificationHubs.Auth;
using Polly;
using Polly.Retry;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a namespace manager settings
    /// </summary>
    public sealed class NamespaceManagerSettings
    {
        private AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManagerSettings"/> class.
        /// </summary>
        public NamespaceManagerSettings()
        {
            TokenProvider = null;
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManagerSettings"/> class.
        /// </summary>
        /// <param name="retryPolicy">Retry policy.</param><param name="tokenProvider">Token provider.</param>
        public NamespaceManagerSettings(AsyncRetryPolicy<HttpResponseMessage> retryPolicy, TokenProvider tokenProvider)
        {
            _retryPolicy = retryPolicy;
            TokenProvider = tokenProvider;
        }

        /// <summary>
        /// Gets or sets the Retry policy.
        /// </summary>
        /// <value>
        /// The Retry olicy.
        /// </value>
        public AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
        {
            get
            {
                return _retryPolicy;
            } 

            set
            {
                _retryPolicy = value ?? throw new ArgumentNullException("RetryPolicy");
            }
        }

        /// <summary>
        /// Gets or sets the security token provider.
        /// </summary>
        /// 
        /// <returns>
        /// The security token provider.
        /// </returns>
        public TokenProvider TokenProvider { get; set; }
    }
}
