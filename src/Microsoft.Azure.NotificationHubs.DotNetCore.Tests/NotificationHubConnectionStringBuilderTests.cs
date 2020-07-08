using Xunit;

namespace Microsoft.Azure.NotificationHubs.Tests
{
    public class NotificationHubConnectionStringBuilderTests
    {
        [Fact]
        public void ToStringShouldReturnAllProperties()
        {
            var b = new NotificationHubConnectionStringBuilder("Endpoint=sb://my-namespace.servicebus.windows.net/")
            {
                SharedAccessKeyName = "key-name",
                SharedAccessKey = "secret"
            };
            
            Assert.Equal(
                "Endpoint=sb://my-namespace.servicebus.windows.net/;SharedAccessKeyName=key-name;SharedAccessKey=secret",
                b.ToString());
        }
    }
}
