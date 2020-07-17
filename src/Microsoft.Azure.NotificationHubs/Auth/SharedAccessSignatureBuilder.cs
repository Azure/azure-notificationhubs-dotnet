﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using Microsoft.IdentityModel.Tokens;

    internal static class SharedAccessSignatureBuilder
    {
        private const string SharedAccessSignature = "SharedAccessSignature";
        private const string SignedResource = "sr";
        private const string Signature = "sig";
        private const string SignedKeyName = "skn";
        private const string SignedExpiry = "se";

        public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static string BuildSignature(
            string keyName,
            byte[] encodedSharedAccessKey,
            string targetUri,
            TimeSpan timeToLive)
        {
            // Normalize the target URI to use the SB scheme.

            string expiresOn = BuildExpiresOn(timeToLive);
            string audienceUri = HttpUtility.UrlEncode(targetUri.ToLowerInvariant());
            List<string> fields = new List<string>();
            fields.Add(audienceUri);
            fields.Add(expiresOn);

            // Example string to be signed:
            // http://mynamespace.servicebus.windows.net/a/b/c?myvalue1=a
            // <Value for ExpiresOn>
            string signature = Sign(string.Join("\n", fields), encodedSharedAccessKey);

            // Example returned string:
            // SharedAccessKeySignature
            // sr=ENCODED(http://mynamespace.servicebus.windows.net/a/b/c?myvalue1=a)&sig=<Signature>&se=<ExpiresOnValue>&skn=<KeyName>

            return string.Format(CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}&{7}={8}",
                SharedAccessSignature,
                SignedResource, audienceUri,
                Signature, HttpUtility.UrlEncode(signature),
                SignedExpiry, HttpUtility.UrlEncode(expiresOn),
                SignedKeyName, HttpUtility.UrlEncode(keyName));
        }

        private static string BuildExpiresOn(TimeSpan timeToLive)
        {
            var expiresOn = DateTime.UtcNow.Add(timeToLive);
            var secondsFromBaseTime = expiresOn.Subtract(EpochTime);
            var seconds = Convert.ToInt64(secondsFromBaseTime.TotalSeconds, CultureInfo.InvariantCulture);
            return Convert.ToString(seconds, CultureInfo.InvariantCulture);
        }

        private static string Sign(string requestString, byte[] encodedSharedAccessKey)
        {
            using (var hmac = new HMACSHA256(encodedSharedAccessKey))
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
            }
        }
    }
}
