using System;
using System.Net.Http;
using Polly;
using Polly.Retry;

namespace Microsoft.Azure.NotificationHubs
{
    class RetryPolicy
    {
        public static AsyncRetryPolicy<HttpResponseMessage> Default
        {
            get
            {
                return Policy
                    .HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                    .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(2));
            }
        }

        public static AsyncRetryPolicy<HttpResponseMessage> NoRetry
        {
            get
            {
                return Policy
                    .HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                    .RetryAsync(0);
            }
        }

        public static AsyncRetryPolicy<HttpResponseMessage> GetWaitAndRetryPolicy(Func<HttpResponseMessage, bool> predicate, int retryCount, Func<int, TimeSpan> sleepDurationProvider)
        {
            return Policy
                .HandleResult(predicate)
                .WaitAndRetryAsync(retryCount, sleepDurationProvider);
        }

        public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(Func<HttpResponseMessage, bool> predicate, int retryCount)
        {
            return Policy
                .HandleResult(predicate)
                .RetryAsync(retryCount);
        }
    }
}
