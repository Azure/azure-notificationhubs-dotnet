//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using Xunit;

namespace Microsoft.Azure.NotificationHubs.Tests
{
    public class InstallationTests
    {
        [Fact]
        public void CanCreateAppleInstallation()
        {
            var installation = new AppleInstallation();

            Assert.Equal(NotificationPlatform.Apns, installation.Platform);
        }

        [Fact]
        public void CanCreateAppleInstallationWithToken()
        {
            const string AppleDeviceToken = "00fc13adff785122b4ad28809a3420982341241421348097878e577c991de8f0";

            var installation = new AppleInstallation(AppleDeviceToken);

            Assert.Equal(AppleDeviceToken, installation.PushChannel);
            Assert.Equal(NotificationPlatform.Apns, installation.Platform);
        }

        [Fact]
        public void CanCreateAdmInstallation()
        {
            var installation = new AdmInstallation();

            Assert.Equal(NotificationPlatform.Adm, installation.Platform);
        }

        [Fact]
        public void CanCreateAdmInstallationWithRegistrationId()
        {
            const string AdmRegistrationId = "00fc13adff785122b4ad28809a3420982341241421348097878e577c991de8f0";

            var installation = new AdmInstallation(AdmRegistrationId);

            Assert.Equal(AdmRegistrationId, installation.PushChannel);
            Assert.Equal(NotificationPlatform.Adm, installation.Platform);
        }

        [Fact]
        public void CanCreateBaiduInstallation()
        {
            var installation = new BaiduInstallation();

            Assert.Equal(NotificationPlatform.Baidu, installation.Platform);
        }

        [Fact]
        public void CanCreateBaiduInstallationWithUserIdAndChannelId()
        {
            const string UserId = "baiduuser";
            const string ChannelId = "baiduchannel";

            var installation = new BaiduInstallation(UserId, ChannelId);

            Assert.Equal($"{UserId}-{ChannelId}", installation.PushChannel);
            Assert.Equal(NotificationPlatform.Baidu, installation.Platform);
        }

        [Fact]
        public void CanCreateFcmInstallation()
        {
            var installation = new FcmInstallation();

            Assert.Equal(NotificationPlatform.Fcm, installation.Platform);
        }

        [Fact]
        public void CanCreateFcmInstallationWithRegistrationId()
        {
            const string FcmRegistrationId = "00fc13adff785122b4ad28809a3420982341241421348097878e577c991de8f0";

            var installation = new FcmInstallation(FcmRegistrationId);

            Assert.Equal(FcmRegistrationId, installation.PushChannel);
            Assert.Equal(NotificationPlatform.Fcm, installation.Platform);
        }

        [Fact]
        public void CanCreateWindowsInstallation()
        {
            var installation = new WindowsInstallation();

            Assert.Equal(NotificationPlatform.Wns, installation.Platform);
        }

        [Fact]
        public void CanCreateWindowsInstallationWithChannelUri()
        {
            const string ChannelUri = "https://notify.windows.net";

            var installation = new WindowsInstallation(ChannelUri);

            Assert.Equal(ChannelUri, installation.PushChannel);
            Assert.Equal(NotificationPlatform.Wns, installation.Platform);
        }

        [Fact]
        public void CanCreateWindowsPhoneInstallation()
        {
            var installation = new WindowsPhoneInstallation();

            Assert.Equal(NotificationPlatform.Mpns, installation.Platform);
        }

        [Fact]
        public void CanCreateWindowsPhoneInstallationWithChannelUri()
        {
            const string ChannelUri = "https://notify.windows.net";

            var installation = new WindowsPhoneInstallation(ChannelUri);

            Assert.Equal(ChannelUri, installation.PushChannel);
            Assert.Equal(NotificationPlatform.Mpns, installation.Platform);
        }
    }
}

