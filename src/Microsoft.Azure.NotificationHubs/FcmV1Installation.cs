//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents a Firebase Cloud Messaging (FCM) V1 installation.
    /// </summary>
    public class FcmV1Installation : Installation
    {
        /// <summary>
        /// Creates a new instance of the FcmV1Installation class.
        /// </summary>
        public FcmV1Installation()
        {
            Platform = NotificationPlatform.FcmV1;
        }

        /// <summary>
        /// Creates a new instance of the FcmV1Installation class.
        /// </summary>
        /// <param name="installationId">The unique identifier for the installation.</param>
        /// <param name="registrationId">The Firebase registration ID to use for the PushChannel.</param>
        public FcmV1Installation(string installationId, string registrationId) : this()
        {
            InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));
            PushChannel = registrationId ?? throw new ArgumentNullException(nameof(registrationId));
        }
    }
}

