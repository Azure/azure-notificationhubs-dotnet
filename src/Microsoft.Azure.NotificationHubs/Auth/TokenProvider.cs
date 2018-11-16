//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Auth
{
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Internal;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Threading.Tasks;

    internal abstract class TokenProvider : IDisposable
    {
        private readonly IMemoryCache _tokenCache;
        private readonly bool _cacheTokens;
        private readonly TimeSpan _cacheExpirationTime;


        protected TokenProvider(TimeSpan cacheExpirationTime)
            : this(true, cacheExpirationTime)
        {
        }

        protected TokenProvider(bool cacheTokens, TimeSpan cacheExpirationTime)
        {
            this._tokenCache = new MemoryCache(new MemoryCacheOptions() { Clock = new SystemClock() });
            this._cacheTokens = cacheTokens && cacheExpirationTime > TimeSpan.Zero;
            this._cacheExpirationTime = cacheExpirationTime;
        }

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

            var normalizedAppliesTo = this.NormalizeAppliesTo(appliesTo);

            string token = null;
            if(TryFetchFromCache(normalizedAppliesTo, action, bypassCache, out token))
            {
                return token;
            }

            //If token is not found in cache, build signature and return
            token = this.GenerateToken(normalizedAppliesTo);

            //Add generated token to cache
            TrySetIntoCache(normalizedAppliesTo, action, bypassCache, token);

            return token;
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
            return NormalizeUri(appliesTo, "http", this.StripQueryParameters, stripPath: false, ensureTrailingSlash: true);
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
