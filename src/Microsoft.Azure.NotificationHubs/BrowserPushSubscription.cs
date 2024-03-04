using System.Text.Json.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
    public class BrowserPushSubscription
    {
        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; }

        [JsonPropertyName("p256dh")]
        public string P256DH { get; set; }

        [JsonPropertyName("auth")]
        public string Auth { get; set; }
    }
}
