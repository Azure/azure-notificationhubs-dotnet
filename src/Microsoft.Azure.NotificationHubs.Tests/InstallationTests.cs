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
        private const string InstallationId = "13EADDC4-00DE-46A5-955F-E200E25CA66C";

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

            var installation = new AppleInstallation(InstallationId, AppleDeviceToken);

            Assert.Equal(InstallationId, installation.InstallationId);
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

            var installation = new AdmInstallation(InstallationId, AdmRegistrationId);

            Assert.Equal(InstallationId, installation.InstallationId);
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

            var installation = new BaiduInstallation(InstallationId, UserId, ChannelId);

            Assert.Equal(InstallationId, installation.InstallationId);
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

            var installation = new FcmInstallation(InstallationId, FcmRegistrationId);

            Assert.Equal(InstallationId, installation.InstallationId);
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

            var installation = new WindowsInstallation(InstallationId, ChannelUri);

            Assert.Equal(InstallationId, installation.InstallationId);
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

            var installation = new WindowsPhoneInstallation(InstallationId, ChannelUri);

            Assert.Equal(ChannelUri, installation.PushChannel);
            Assert.Equal(NotificationPlatform.Mpns, installation.Platform);
        }
    }
}

