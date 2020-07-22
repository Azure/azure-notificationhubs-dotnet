//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    ///   The type of approach to apply when calculating the delay
    ///   between retry attempts.
    /// </summary>
    ///
    public enum NotificationHubRetryMode
    {
        /// <summary>Retry attempts happen at fixed intervals; each delay is a consistent duration.</summary>
        Fixed,

        /// <summary>Retry attempts will delay based on a back-off strategy, where each attempt will increase the duration that it waits before retrying.</summary>
        Exponential
    }
}
