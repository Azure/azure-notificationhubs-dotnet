//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents an Windows Phone (MPNS) installation.
    /// </summary>
    public class WindowsPhoneInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the WindowsPhoneInstallation class.
        /// </summary>
        public WindowsPhoneInstallation()
        {
            Platform = NotificationPlatform.Mpns;
        }

        /// <summary>
        /// Creates a new instance of the WindowsPhoneInstallation class.
        /// </summary>
        /// <param name="installationId">The unique identifier for the installation.</param>
        /// <param name="channelUri">The MPNS Channel URI to use for the PushChannel.</param>
        public WindowsPhoneInstallation(string installationId, string channelUri) : this()
        {
            InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));
            PushChannel = channelUri ?? throw new ArgumentNullException(nameof(channelUri));
        }
    }
}
