//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    ///   The set of extension methods for the <see cref="NotificationHubRetryOptions" />
    ///   class.
    /// </summary>
    ///
    public static class NotificationHubRetryOptionsExtensions
    {
        /// <summary>
        ///   Converts the options into a retry policy for use.
        /// </summary>
        ///
        /// <param name="instance">The instance that this method was invoked on.</param>
        ///
        /// <returns>The <see cref="NotificationHubRetryPolicy" /> represented by the options.</returns>
        public static NotificationHubRetryPolicy ToRetryPolicy(this NotificationHubRetryOptions instance) =>
            instance.CustomRetryPolicy ?? new BasicRetryPolicy(instance);
    }
}
