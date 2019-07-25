using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.NotificationHubs
{
    class RetryPolicy
    {
        internal static readonly TimeSpan ServerBusyBaseSleepTime = TimeSpan.FromSeconds(10);

        object serverBusyLock = new object();
        volatile bool serverBusy;
        volatile string serverBusyExceptionMessage;

        // remarks: constructor is marked internal to prevent
        // external assemblies inheriting from it.
        internal RetryPolicy()
        {
        }

        /// <summary>
        /// Gets a retry policy that performs no retries.
        /// </summary>
        /// 
        /// <returns>
        /// A retry policy that performs no retries.
        /// </returns>
        public static RetryPolicy NoRetry
        {
            get;
        }

        /// <summary>
        /// Gets the default policy associated with the policy.
        /// </summary>
        /// 
        /// <returns>
        /// The default policy associated with the policy.
        /// </returns>
        public static RetryPolicy Default
        {
            get;
        }

        internal bool IsServerBusy
        {
            get
            {
                return this.serverBusy;
            }
        }

        internal string ServerBusyExceptionMessage
        {
            get
            {
                return this.serverBusyExceptionMessage;
            }
        }
    }
}
