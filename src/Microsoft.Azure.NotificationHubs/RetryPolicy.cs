//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Net.Http;
using Polly;
using Polly.Retry;

namespace Microsoft.Azure.NotificationHubs
{
    internal static class RetryPolicy
    {
        public static AsyncRetryPolicy<HttpResponseMessage> Default =>
            Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode).WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(2));

        public static AsyncRetryPolicy<HttpResponseMessage> NoRetry => 
            Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode).RetryAsync(0);

        public static AsyncRetryPolicy<HttpResponseMessage> GetWaitAndRetryPolicy(
            Func<HttpResponseMessage, bool> predicate, int retryCount, Func<int, TimeSpan> sleepDurationProvider) => 
            Policy.HandleResult(predicate).WaitAndRetryAsync(retryCount, sleepDurationProvider);

        public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(
            Func<HttpResponseMessage, bool> predicate, int retryCount) => 
            Policy.HandleResult(predicate).RetryAsync(retryCount);
    }
}
