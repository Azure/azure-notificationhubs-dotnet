//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Net.Http;
using Microsoft.Azure.NotificationHubs.Auth;
using Polly.Retry;

namespace Microsoft.Azure.NotificationHubs
{
    internal sealed class NamespaceManagerSettings
    {
        private AsyncRetryPolicy<HttpResponseMessage> retryPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManagerSettings"/> class.
        /// </summary>
        public NamespaceManagerSettings()
        {
            TokenProvider = null;
            retryPolicy = NotificationHubs.RetryPolicy.Default;
        }

        public AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
        {
            get
            {
                return retryPolicy;
            } 

            set
            {
                retryPolicy = value ?? throw new ArgumentNullException("RetryPolicy");
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
