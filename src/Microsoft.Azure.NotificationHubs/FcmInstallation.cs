﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents a Firebase Cloud Messaging (FCM) installation.
    /// </summary>
    public class FcmInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the FcmInstallation class.
        /// </summary>
        public FcmInstallation()
        {
            Platform = NotificationPlatform.Fcm;
        }

        /// <summary>
        /// Creates a new instance of the FcmInstallation class.
        /// </summary>
        /// <param name="registrationId">The Firebase registration ID to use for the PushChannel.</param>
        public FcmInstallation(string registrationId)
        {
            PushChannel = registrationId;
            Platform = NotificationPlatform.Fcm;
        }
    }
}

