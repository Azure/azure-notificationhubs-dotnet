// ----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
// ----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Net;

    /// <summary>
    /// Notification Hubs client settings
    /// <seealso cref="NotificationHubClient" />
    /// </summary>
    public class NotificationHubClientSettings
    {
        /// <summary>
        /// Gets or sets the proxy
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets operation timeout of the HTTP operations.
        /// </summary>
        /// <value>
        ///   <c>Http operation timeout. Defaults to 60 seconds</c>.
        /// </value>
        /// <remarks>
        ///  </remarks>
        public TimeSpan? OperationTimeout { get; set; }
    }
}