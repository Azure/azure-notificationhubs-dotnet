//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents an Windows Notification Services (WNS) installation.
    /// </summary>
    public class WindowsInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the WnsInstallation class.
        /// </summary>
        public WindowsInstallation()
        {
            Platform = NotificationPlatform.Wns;
        }

        /// <summary>
        /// Creates a new instance of the WnsInstallation class.
        /// </summary>
        /// <param name="installationId">The unique identifier for the installation.</param>
        /// <param name="channelUri">The WNS Channel URI to use for the PushChannel.</param>
        public WindowsInstallation(string installationId, string channelUri) : this()
        {
            InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));
            PushChannel = channelUri ?? throw new ArgumentNullException(nameof(channelUri));
        }
    }
}
