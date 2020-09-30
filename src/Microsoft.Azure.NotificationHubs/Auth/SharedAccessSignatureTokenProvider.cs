//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Text;
using Microsoft.Azure.NotificationHubs.Common;

namespace Microsoft.Azure.NotificationHubs.Auth
{
    /// <summary>
    /// Represents a Token Provider for Shared Access Signatures
    /// </summary>
    public class SharedAccessSignatureTokenProvider : TokenProvider
    {
        private const int MaxKeyNameLength = 256;
        private const int MaxKeyLength = 256;

        internal readonly byte[] _encodedSharedAccessKey;
        internal readonly string _keyName;
        internal readonly TimeSpan _tokenTimeToLive;

        private static readonly TimeSpan DefaultTokenTimeout = TimeSpan.FromMinutes(20);
        private static readonly TimeSpan DefaultTokenRefreshTimeMargin = TimeSpan.FromMinutes(2);

        internal SharedAccessSignatureTokenProvider(string connectionString):
            base(DefaultTokenTimeout - DefaultTokenRefreshTimeMargin)
        {
            var builder = new NotificationHubConnectionStringBuilder(connectionString);
            this._keyName = builder.SharedAccessKeyName;
            this._encodedSharedAccessKey = Encoding.UTF8.GetBytes(builder.SharedAccessKey);
            this._tokenTimeToLive = DefaultTokenTimeout;
        }

        internal SharedAccessSignatureTokenProvider(string keyName, string sharedAccessKey)
            : this(keyName, sharedAccessKey, DefaultTokenTimeout)
        {
        }

        internal SharedAccessSignatureTokenProvider(string keyName, string sharedAccessKey, TimeSpan tokenTimeToLive)
            : base(tokenTimeToLive - DefaultTokenRefreshTimeMargin)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentNullException("keyName");
            }

            if (keyName.Length > MaxKeyNameLength)
            {
                throw new ArgumentOutOfRangeException(
                    "keyName",
                    SRCore.ArgumentStringTooBig("keyName", MaxKeyNameLength));
            }

            if (string.IsNullOrEmpty(sharedAccessKey))
            {
                throw new ArgumentNullException("sharedAccessKey");
            }

            if (sharedAccessKey.Length > MaxKeyLength)
            {
                throw new ArgumentOutOfRangeException(
                    "sharedAccessKey",
                    SRCore.ArgumentStringTooBig("sharedAccessKey", MaxKeyLength));
            }

            this._encodedSharedAccessKey = Encoding.UTF8.GetBytes(sharedAccessKey);
            this._keyName = keyName;
            this._tokenTimeToLive = tokenTimeToLive;
        }

        /// <summary>
        /// Construct a TokenProvider based on a sharedAccessSignature.
        /// </summary>
        /// <param name="connectionString">The connection string to the resource</param>
        /// <returns>A TokenProvider initialized with the shared access signature</returns>
        public static TokenProvider CreateSharedAccessSignatureTokenProvider(string connectionString)
        {
            return new SharedAccessSignatureTokenProvider(connectionString);
        }

        /// <summary>
        /// Construct a TokenProvider based on the provided Key Name and Shared Access Key.
        /// </summary>
        /// <param name="keyName">The key name of the corresponding SharedAccessKeyAuthorizationRule.</param>
        /// <param name="sharedAccessKey">The key associated with the SharedAccessKeyAuthorizationRule</param>
        /// <returns>A TokenProvider initialized with the provided RuleId and Password</returns>
        public static TokenProvider CreateSharedAccessSignatureTokenProvider(string keyName, string sharedAccessKey)
        {
            return new SharedAccessSignatureTokenProvider(keyName, sharedAccessKey, DefaultTokenTimeout);
        }

        /// <summary>
        /// Generates the token based upon the applies to.
        /// </summary>
        /// <param name="appliesTo">The scope of the token to generate.</param>
        /// <returns>The generated token</returns>
        protected override string GenerateToken(string appliesTo)
        {
            return BuildSignature(appliesTo);
        }

        private string BuildSignature(string targetUri)
        {
            return SharedAccessSignatureBuilder.BuildSignature(this._keyName, this._encodedSharedAccessKey, targetUri, this._tokenTimeToLive);
        }
    }
}
