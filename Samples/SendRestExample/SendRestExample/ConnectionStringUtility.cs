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
            foreach (var t in parts)
            {
                if (t.StartsWith("Endpoint"))
                {
                    Endpoint = "https" + t.Substring(11);
                }

                if (t.StartsWith("SharedAccessKeyName"))
                {
                    SasKeyName = t.Substring(20);
                }

                if (t.StartsWith("SharedAccessKey"))
                {
                    SasKeyValue = t.Substring(16);
                }
            }
        }

        public string GetSaSToken(string uri, int minUntilExpire)
        {
            string targetUri = Uri.EscapeDataString(uri.ToLower()).ToLower();

            // Add an expiration in seconds to it.
            long expiresOnDate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            expiresOnDate += minUntilExpire * 60 * 1000;
            long expiresSeconds = expiresOnDate / 1000;
            String toSign = targetUri + "\n" + expiresSeconds;

            // Generate a HMAC-SHA256 hash or the uri and expiration using your secret key.
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(SasKeyValue);
            HMACSHA256 hmacsha256 = new HMACSHA256(keyBytes);
            byte[] hash = hmacsha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toSign));

            // Create the token string using the base64
            string signature = Uri.EscapeDataString(Convert.ToBase64String(hash));

            return "SharedAccessSignature sr=" + targetUri + "&sig=" + signature + "&se=" + expiresSeconds + "&skn=" + SasKeyName;
        }

    }
}
