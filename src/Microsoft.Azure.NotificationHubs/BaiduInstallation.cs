//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

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
        /// <param name="baiduUserId">The Baidu User ID.</param>
        /// <param name="baiduChannelId">The Baidu Channel ID.</param>
        public BaiduInstallation(string baiduUserId, string baiduChannelId)
        {
            Platform = NotificationPlatform.Baidu;
            PushChannel = $"{baiduUserId}-{baiduChannelId}";
        }
    }
}

