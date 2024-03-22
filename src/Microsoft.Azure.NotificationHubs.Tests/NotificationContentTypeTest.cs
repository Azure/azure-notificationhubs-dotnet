using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Microsoft.Azure.NotificationHubs.DotNetCore.Tests
{
    public class NotificationContentTypeTest
    {
        [Fact]
        public void NotificationContentTypeCorrespondsToApi()
        {
            Assert.Equal($"application/json;charset={Encoding.UTF8.WebName}", new AppleNotification("{\"aps\":{\"alert\":\"alert!\"}}").ContentType);
            Assert.Equal($"application/json;charset={Encoding.UTF8.WebName}", new FcmNotification("{\"data\":{\"message\":\"Message\"}}").ContentType);
            Assert.Equal($"application/json;charset={Encoding.UTF8.WebName}", new FcmV1Notification("{\"message\":{\"data\":{\"message\":\"Message\"}}}").ContentType);
            Assert.Equal($"application/xml;charset={Encoding.UTF8.WebName}", new MpnsNotification("<wp:Notification xmlns:wp=\"WPNotification\" Version=\"2.0\"><wp:Tile Id=\"TileId\" Template=\"IconicTile\"><wp:Title Action=\"Clear\">Title</wp:Title></wp:Tile></wp:Notification>").ContentType);
            Assert.Equal("application/xml", new WindowsNotification("<toast><visual><binding template=\"ToastText01\"><text id=\"1\">bodyText</text></binding>  </visual></toast>").ContentType);
            Assert.Equal($"application/json;charset={Encoding.UTF8.WebName}", new TemplateNotification(new Dictionary<string, string>()).ContentType);
            Assert.Equal("application/json", new AdmNotification("{\"data\":{\"key1\":\"value1\"}}").ContentType);
            Assert.Equal("application/x-www-form-urlencoded", new BaiduNotification("{\"title\":\"Title\",\"description\":\"Description\"}").ContentType);
            Assert.Equal($"application/json;charset={Encoding.UTF8.WebName}", new BrowserNotification("{\"title\": \"Title\", \"message\": \"Hello World!\"}").ContentType);
        }

        [Theory]
        [InlineData("application/json")]
        [InlineData("application/json;charset=utf-8")]
        [InlineData("application/json;charset=UTF-8")]
        [InlineData("application/json; charset = UTF-8")]
        [InlineData("application/json;charset='UTF-8'")]
        [InlineData("application/json;charset=\"UTF-8\"")]
        [InlineData("application/xml")]
        [InlineData("application/x-www-form-urlencoded")]
        [InlineData("application/octet-stream")]
        public void ParseContentTypeSuccessForValidString(string contentType)
        {
            NotificationHubClient.ParseContentType(contentType, out var mediaType, out var encoding);
            Assert.NotEmpty(mediaType);
            Assert.NotNull(encoding);
        }

        [Theory]
        [InlineData(@"123;charset=utf-8")]
        [InlineData(@";charset=utf-8")]
        [InlineData(@"charset=utf-8")]
        [InlineData(@"application")]
        [InlineData(@"application/")]
        [InlineData(@"application/json;charset=123")]
        [InlineData(@"application/json;charset=")]
        [InlineData(@"application/json;charset=utf-8;123")]
        [InlineData(@"application/json;charset=utf - 8")]
        public void ParseContentTypeFailsForInvalidString(string contentType)
        {
            Assert.Throws<ArgumentException>(() => NotificationHubClient.ParseContentType(contentType, out _, out _));
        }
    }
}
