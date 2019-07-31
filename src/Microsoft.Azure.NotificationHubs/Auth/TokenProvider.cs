//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace Microsoft.Azure.NotificationHubs.Auth
{
    /// <summary>
    /// Represents a token provider.
    /// </summary>
    public abstract class TokenProvider : IDisposable
    {
        private static readonly TimeSpan DefaultTokenTimeout = TimeSpan.FromMinutes(20.0);
        private static readonly TimeSpan InitialRetrySleepTime = TimeSpan.FromMilliseconds(50.0);
        private readonly IMemoryCache _tokenCache;
        private readonly bool _cacheTokens;
        private readonly TimeSpan _cacheExpirationTime;
        private readonly bool _isWebTokenSupported;
        private readonly TimeSpan _retrySleepTime;
        private readonly int _cacheSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenProvider"/> class.
        /// </summary>
        /// /// <param name="cacheExpirationTime">Cache expiration time.</param>
        protected TokenProvider(TimeSpan cacheExpirationTime)
            : this(true, cacheExpirationTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenProvider"/> class.
        /// </summary>
        /// <param name="cacheTokens">Cache tokens.</param><param name="cacheExpirationTime">Cache expiration time.</param>
        protected TokenProvider(bool cacheTokens, TimeSpan cacheExpirationTime)
        {
            _tokenCache = new MemoryCache(new MemoryCacheOptions() { Clock = new SystemClock() });
            _cacheTokens = cacheTokens && cacheExpirationTime > TimeSpan.Zero;
            _cacheExpirationTime = cacheExpirationTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.TokenProvider" /> class.
        /// </summary>
        /// <param name="cacheTokens">if set to <c>true</c> [cache tokens].</param>
        /// <param name="supportHttpAuthToken">if set to <c>true</c> [support HTTP authentication token].</param>
        /// <param name="tokenScope">The token scope.</param>
        protected TokenProvider(bool cacheTokens, bool supportHttpAuthToken, TokenScope tokenScope)
            : this(cacheTokens, supportHttpAuthToken, 1000, tokenScope)
        {
        }

        protected TokenProvider(bool cacheTokens, bool supportHttpAuthToken)
            : this(cacheTokens, supportHttpAuthToken, 1000, TokenScope.Entity)
        {
        }

        /// <summary>
        /// Construct a TokenProvider based on the provided issuer name and issuer secret.
        /// </summary>
        /// <param name="issuerName">The issuer name to initialize the TokenProvider with.</param>
        /// <param name="issuerSecret">The issuer name to initialize the TokenProvider with.</param>
        /// <param name="stsUri">The URI of the STS to use.</param>
        /// <returns>A TokenProvider initialized with the provided issuer name and secret.</returns>
        public static TokenProvider CreateSharedSecretTokenProvider(
            string issuerName,
            string issuerSecret,
            Uri stsUri)
        {
            return new SharedSecretTokenProvider(issuerName, issuerSecret, stsUri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.TokenProvider" /> class.
        /// </summary>
        /// <param name="cacheTokens">if set to <c>true</c> [cache tokens].</param>
        /// <param name="supportHttpAuthToken">if set to <c>true</c> [support HTTP authentication token].</param>
        /// <param name="cacheSize">Size of the cache.</param>
        /// <param name="tokenScope">The token scope.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">cacheSize</exception>
        protected TokenProvider(
            bool cacheTokens,
            bool supportHttpAuthToken,
            int cacheSize,
            TokenScope tokenScope)
        {
            if (cacheSize < 1)
                throw new ArgumentOutOfRangeException(nameof(cacheSize), SRClient.ArgumentOutOfRangeLessThanOne);
            TokenScope = tokenScope;
            _cacheSize = cacheSize;
            _cacheTokens = cacheTokens;
            _isWebTokenSupported = supportHttpAuthToken;
            _retrySleepTime = InitialRetrySleepTime;
        }

        /// <summary>Creates a windows token provider.</summary>
        /// <returns>
        /// The <see cref="T:Microsoft.Azure.NotificationHubs.TokenProvider" /> for returning the windows token.
        /// </returns>
        /// <param name="stsUris">The URIs of the Security Token Service (STS).</param>
        public static TokenProvider CreateWindowsTokenProvider(IEnumerable<Uri> stsUris)
        {
            return new WindowsTokenProvider(stsUris, (NetworkCredential)null);
        }

        /// <summary>Creates a windows token provider.</summary>
        /// <returns>
        /// The <see cref="T:Microsoft.Azure.NotificationHubs.TokenProvider" /> for returning the windows token.
        /// </returns>
        /// <param name="stsUris">The URIs of the Security Token Service (STS).</param>
        /// <param name="credential">The user credential.</param>
        public static TokenProvider CreateWindowsTokenProvider(
            IEnumerable<Uri> stsUris,
            NetworkCredential credential)
        {
            return new WindowsTokenProvider(stsUris, credential);
        }

        /// <summary>
        /// Creates an OAuth (open standard for authorization) token provider.
        /// </summary>
        /// <returns>
        /// The <see cref="T:Microsoft.Azure.NotificationHubs.TokenProvider" /> for returning OAuth token.
        /// </returns>
        /// <param name="stsUris">The URIs of the Security Token Service (STS).</param>
        /// <param name="credential">The user credential.</param>
        public static TokenProvider CreateOAuthTokenProvider(
            IEnumerable<Uri> stsUris,
            NetworkCredential credential)
        {
            return new OAuthTokenProvider(stsUris, credential);
        }

        /// <summary>
        /// Construct a TokenProvider based on the provided Key Name and Shared Access Key.
        /// </summary>
        /// <param name="keyName">The key name of the corresponding SharedAccessKeyAuthorizationRule.</param>
        /// <param name="sharedAccessKey">The key associated with the SharedAccessKeyAuthorizationRule</param>
        /// <returns>A TokenProvider initialized with the provided RuleId and Password</returns>
        public static TokenProvider CreateSharedAccessSignatureTokenProvider(
            string keyName,
            string sharedAccessKey)
        {
            return new SharedAccessSignatureTokenProvider(keyName, sharedAccessKey, DefaultTokenTimeout);
        }

        /// <summary>
        /// Construct a TokenProvider based on the provided Key Name and Shared Access Key.
        /// </summary>
        /// <param name="keyName">The key name of the corresponding SharedAccessKeyAuthorizationRule.</param>
        /// <param name="sharedAccessKey">The key associated with the SharedAccessKeyAuthorizationRule</param>
        /// <param name="tokenTimeToLive">The token time to live</param>
        /// <returns>A TokenProvider initialized with the provided RuleId and Password</returns>
        public static TokenProvider CreateSharedAccessSignatureTokenProvider(
            string keyName,
            string sharedAccessKey,
            TimeSpan tokenTimeToLive)
        {
            return new SharedAccessSignatureTokenProvider(keyName, sharedAccessKey, tokenTimeToLive);
        }

        /// <summary>
        /// Gets or sets the token scope associated with the provider.
        /// </summary>
        /// <value>The token scope.</value>
        public TokenScope TokenScope { get; private set; }

        protected abstract string GenerateToken(string appliesTo);

        /// <summary>
        /// Gets whether the token provider strips query parameters.
        /// </summary>
        /// 
        /// <returns>
        /// true if the token provider strips query parameters; otherwise, false.
        /// </returns>
        protected virtual bool StripQueryParameters => true;

        /// <summary>
        /// Asynchronously retrieves the token for the provider.
        /// </summary>
        /// 
        /// <returns>
        /// The result of the asynchronous operation.
        /// </returns>
        /// <param name="appliesTo">The URI which the access token applies to.</param>
        /// <param name="action">The request action.</param>
        /// <param name="bypassCache">true to ignore existing token information in the cache; false to use the token information in the cache.</param>
        public string GetToken(string appliesTo, string action = "Manage", bool bypassCache = false)
        {
            if (string.IsNullOrEmpty(appliesTo))
            {
                throw new ArgumentException(SRClient.NullAppliesTo);
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var normalizedAppliesTo = NormalizeAppliesTo(appliesTo);

            if(TryFetchFromCache(normalizedAppliesTo, action, bypassCache, out string token))
            {
                return token;
            }

            //If token is not found in cache, build signature and return
            token = GenerateToken(normalizedAppliesTo);

            //Add generated token to cache
            TrySetIntoCache(normalizedAppliesTo, action, bypassCache, token);

            return token;
        }

        /// <summary>
        /// Construct a TokenProvider based on the provided issuer name, issuer secret, and the default AccessControl namespace.
        /// </summary>
        /// <param name="issuerName">The issuer name to initialize the TokenProvider with.</param>
        /// <param name="issuerSecret">The issuer name to initialize the TokenProvider with.</param>
        /// <returns>A TokenProvider initialized with the provided issuer name and secret.</returns>
        public static TokenProvider CreateSharedSecretTokenProvider(
            string issuerName,
            string issuerSecret)
        {
            return (TokenProvider)new SharedSecretTokenProvider(issuerName, issuerSecret);
        }

        private bool TryFetchFromCache(string appliesTo, string action, bool bypassCache, out string token)
        {
            token = null;
            var cacheKey = BuildKey(appliesTo, action);
            return !bypassCache && _cacheTokens && _tokenCache.TryGetValue(cacheKey, out token);
        }

        private void TrySetIntoCache(string appliesTo, string action, bool bypassCache, string token)
        {
            if(!bypassCache && _cacheTokens)
            {
                var cacheKey = BuildKey(appliesTo, action);
                _tokenCache.Set(cacheKey, token, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheExpirationTime));
            }
        }

        private string NormalizeAppliesTo(string appliesTo)
        {
            return NormalizeUri(appliesTo, "http", StripQueryParameters, stripPath: false, ensureTrailingSlash: true);
        }

        private string BuildKey(string appliesTo, string action)
        {
            return $"{appliesTo}_{action}";
        }

        public void Dispose()
        {
            _tokenCache?.Dispose();
        }

        private static string NormalizeUri(string uri, string scheme, bool stripQueryParameters = true, bool stripPath = false, bool ensureTrailingSlash = false)
        {
            UriBuilder uriBuilder = new UriBuilder(uri)
            {
                Scheme = scheme,
                Port = -1,
                Fragment = string.Empty,
                Password = string.Empty,
                UserName = string.Empty,
            };

            if (stripPath)
            {
                uriBuilder.Path = string.Empty;
            }

            if (stripQueryParameters)
            {
                uriBuilder.Query = string.Empty;
            }

            if (ensureTrailingSlash)
            {
                if (!uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
                {
                    uriBuilder.Path += "/";
                }
            }

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}
