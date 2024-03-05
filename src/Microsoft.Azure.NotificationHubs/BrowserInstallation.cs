//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Text.Json;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents a browser installation.
    /// </summary>
    public class BrowserInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.BrowserInstallation"/> class.
        /// </summary>
        public BrowserInstallation()
        {
            Platform = NotificationPlatform.Browser;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.BrowserInstallation"/> class.
        /// </summary>
        /// <param name="installationId">The unique identifier for the installation.</param>
        /// <param name="browserPushSubscription">The browser push subscription.</param>
        public BrowserInstallation(string installationId, BrowserPushSubscription browserPushSubscription) : this()
        {
            InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));

            if (string.IsNullOrWhiteSpace(browserPushSubscription.Endpoint))
            {
                throw new ArgumentNullException(nameof(browserPushSubscription.Endpoint));
            }

            if (string.IsNullOrWhiteSpace(browserPushSubscription.P256DH))
            {
                throw new ArgumentNullException(nameof(browserPushSubscription.P256DH));
            }

            if (string.IsNullOrWhiteSpace(browserPushSubscription.Auth))
            {
                throw new ArgumentNullException(nameof(browserPushSubscription.Auth));
            }

            PushChannel = JsonSerializer.Serialize(browserPushSubscription);
        }
    }
}
