//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// This class represents an Baidu installation.
    /// </summary>
    public class BaiduInstallation : Installation
    {
        /// <summary>
        /// Creates a new instance of the BaiduInstallation class.
        /// </summary>
        public BaiduInstallation()
        {
            Platform = NotificationPlatform.Baidu;
        }

        /// <summary>
        /// Creates a new instance of the BaiduInstallation class with the Baidu User ID and Channel ID
        /// to set on the PushChannel.
        /// </summary>
        /// <param name="installationId">The unique identifier for the installation.</param>
        /// <param name="baiduUserId">The Baidu User ID.</param>
        /// <param name="baiduChannelId">The Baidu Channel ID.</param>
        public BaiduInstallation(string installationId, string baiduUserId, string baiduChannelId) : this()
        {
            InstallationId = installationId ?? throw new ArgumentNullException(nameof(installationId));
            if (string.IsNullOrWhiteSpace(baiduUserId))
            {
                throw new ArgumentNullException(nameof(baiduUserId));
            }
            if (string.IsNullOrWhiteSpace(baiduChannelId))
            {
                throw new ArgumentNullException(nameof(baiduChannelId));
            }
                
            PushChannel = $"{baiduUserId}-{baiduChannelId}";
        }
    }
}
