//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents an Apple Push Notification service (APNs) installation.
    /// </summary>
    public class AppleInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the AppleInstallation class.
        /// </summary>
        public AppleInstallation()
        {
            Platform = NotificationPlatform.Apns;
        }

        /// <summary>
        /// Creates a new instance of the AppleInstallation class.
        /// </summary>
        /// <param name="deviceToken">The APNs device token to use for the PushChannel.</param>
        public AppleInstallation(string deviceToken)
        {
            PushChannel = deviceToken;
            Platform = NotificationPlatform.Apns;
        }
    }
}

