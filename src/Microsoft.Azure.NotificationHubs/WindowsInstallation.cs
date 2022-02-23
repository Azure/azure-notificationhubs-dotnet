//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

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
        /// <param name="channelUri">The WNS Channel URI to use for the PushChannel.</param>
        public WindowsInstallation(string channelUri)
        {
            PushChannel = channelUri;
            Platform = NotificationPlatform.Wns;
        }
    }
}
