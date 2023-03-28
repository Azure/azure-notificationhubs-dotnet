//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents a Xiaomi installation.
    /// </summary>
    public class XiaomiInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the XiaomiInstallation class.
        /// </summary>
        public XiaomiInstallation()
        {
            Platform = NotificationPlatform.Xiaomi;
        }

        /// <summary>
        /// Creates a new instance of the XiaomiInstallation class.
        /// </summary>
        /// <param name="installationId">The unique identifier for the installation.</param>
        /// <param name="xiaomiRegistrationId">The Xiaomi registration ID to use for the PushChannel.</param>
        public XiaomiInstallation(string installationId, string xiaomiRegistrationId) : this()
        {
            InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));
            PushChannel = xiaomiRegistrationId ?? throw new ArgumentNullException(nameof(xiaomiRegistrationId));
        }
    }
}
