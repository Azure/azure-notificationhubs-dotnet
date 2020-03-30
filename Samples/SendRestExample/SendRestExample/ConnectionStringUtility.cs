using System;
using System.Security.Cryptography;


namespace SendRestExample
{
    class ConnectionStringUtility
    {
        public string Endpoint { get; private set; }
        public string SasKeyName { get; private set; }
        public string SasKeyValue { get; private set; }

        public ConnectionStringUtility(string connectionString)
        {
            //Parse Connectionstring
            char[] separator = { ';' };
            string[] parts = connectionString.Split(separator);
            foreach (var part in parts)
            {
                if (part.StartsWith("Endpoint"))
                {
                    Endpoint = "https" + part.Substring(11);
                }

                if (part.StartsWith("SharedAccessKeyName"))
                {
                    SasKeyName = part.Substring(20);
                }

                if (part.StartsWith("SharedAccessKey"))
                {
                    SasKeyValue = part.Substring(16);
                }
            }
        }

        public string GetSaSToken(string uri, int minUntilExpire)
        {
            string targetUri = Uri.EscapeDataString(uri.ToLower()).ToLower();

            // Add an expiration in seconds to it.
            var expiresOnDate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            expiresOnDate += minUntilExpire * 60 * 1000;
            var expiresSeconds = expiresOnDate / 1000;
            var toSign = targetUri + "\n" + expiresSeconds;

            // Generate a HMAC-SHA256 hash or the uri and expiration using your secret key.
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(SasKeyValue);
            var hmacsha256 = new HMACSHA256(keyBytes);
            var hash = hmacsha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toSign));

            // Create the token string using the base64
            var signature = Uri.EscapeDataString(Convert.ToBase64String(hash));

            return "SharedAccessSignature sr=" + targetUri + "&sig=" + signature + "&se=" + expiresSeconds + "&skn=" + SasKeyName;
        }

    }
}
