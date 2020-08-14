//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    ///   An abstract representation of a policy to govern retrying of messaging operations.
    /// </summary>
    ///
    /// <remarks>
    ///   It is recommended that developers without advanced needs not implement custom retry
    ///   policies but instead configure the default policy by specifying the desired set of
    ///   retry options when creating one of the Service Bus clients.
    /// </remarks>
    ///
    /// <seealso cref="NotificationHubRetryOptions"/>
    ///
    public abstract class NotificationHubRetryPolicy
    {
        /// <summary>
        ///   Calculates the amount of time to wait before another attempt should be made.
        /// </summary>
        ///
        /// <param name="lastException">The last exception that was observed for the operation to be retried.</param>
        /// <param name="attemptCount">The number of total attempts that have been made, including the initial attempt before any retries.</param>
        ///
        /// <returns>The amount of time to delay before retrying the associated operation; if <c>null</c>, then the operation is no longer eligible to be retried.</returns>
        ///
        public abstract TimeSpan? CalculateRetryDelay(
            Exception lastException,
            int attemptCount);

        /// <summary>
        ///   Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        ///
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        ///
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        ///
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        ///
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        ///
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        ///   Converts the instance to string representation.
        /// </summary>
        ///
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        ///
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();

        /// <summary>
        ///
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal async Task<T> RunOperation<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken)
        {
            var failedAttemptCount = 0;


            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    return await operation(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // Determine if there should be a retry for the next attempt; if so enforce the delay but do not quit the loop.
                    // Otherwise, throw the translated exception.

                    ++failedAttemptCount;
                    TimeSpan? retryDelay = CalculateRetryDelay(ex, failedAttemptCount);
                    if (retryDelay.HasValue && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(retryDelay.Value, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        ExceptionDispatchInfo.Capture(ex)
                            .Throw();
                    }
                }
            }
            // If no value has been returned nor exception thrown by this point,
            // then cancellation has been requested.
            throw new TaskCanceledException();
        }
    }
}
