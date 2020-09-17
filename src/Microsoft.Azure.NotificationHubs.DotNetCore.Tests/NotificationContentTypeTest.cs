using System.Collections.Generic;
using Xunit;

namespace Microsoft.Azure.NotificationHubs.DotNetCore.Tests
{
    public class NotificationContentTypeTest
    {
        [Fact]
        public void NotificationContentTypeCorrespondsToApi()
        {
            Assert.Equal("application/json", new AppleNotification("{\"aps\":{\"alert\":\"alert!\"}}").ContentType);
            Assert.Equal("application/json", new FcmNotification("{\"data\":{\"message\":\"Message\"}}").ContentType);
            Assert.Equal("application/xml", new MpnsNotification("<wp:Notification xmlns:wp=\"WPNotification\" Version=\"2.0\"><wp:Tile Id=\"TileId\" Template=\"IconicTile\"><wp:Title Action=\"Clear\">Title</wp:Title></wp:Tile></wp:Notification>").ContentType);
            Assert.Equal("application/xml", new WindowsNotification("<toast><visual><binding template=\"ToastText01\"><text id=\"1\">bodyText</text></binding>  </visual></toast>").ContentType);
            Assert.Equal("application/json", new TemplateNotification(new Dictionary<string, string>()).ContentType);
            Assert.Equal("application/xml", new AdmNotification("{\"data\":{\"key1\":\"value1\"}}").ContentType);
            Assert.Equal("application/x-www-form-urlencoded", new BaiduNotification("{\"title\":\"Title\",\"description\":\"Description\"}").ContentType);
        }
    }
}
