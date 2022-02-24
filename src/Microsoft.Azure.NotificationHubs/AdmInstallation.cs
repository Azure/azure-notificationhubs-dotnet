//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents an Amazon Device Messaging (ADM) installation.
    /// </summary>
    public class AdmInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the AdmInstallation class.
        /// </summary>
        public AdmInstallation()
        {
            Platform = NotificationPlatform.Adm;
        }

        /// <summary>
        /// Creates a new instance of the AdmInstallation class.
        /// </summary>
        /// <param name="installationId">The unique identifier for the installation.</param>
        /// <param name="admRegistrationId">The ADM registration ID to use for the PushChannel.</param>
        public AdmInstallation(string installationId, string admRegistrationId) : this()
        {
            InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));
            PushChannel = admRegistrationId ?? throw new ArgumentNullException(nameof(admRegistrationId));
        }
    }
}
