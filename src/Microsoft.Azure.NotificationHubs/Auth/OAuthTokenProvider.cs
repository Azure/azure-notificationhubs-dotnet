using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.Azure.NotificationHubs.Auth
{
    public class OAuthTokenProvider: TokenProvider
    {
        private readonly List<Uri> stsUris;
        private readonly NetworkCredential credential;

        internal OAuthTokenProvider(IEnumerable<Uri> stsUris, NetworkCredential credential)
            : base(true, true, 100, TokenScope.Namespace)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));
            if (stsUris == null)
                throw new ArgumentNullException(nameof(stsUris));
            this.stsUris = stsUris.ToList<Uri>();
            if (this.stsUris.Count == 0)
                throw new ArgumentNullException(nameof(stsUris));
            this.credential = credential;
        }

        protected override string GenerateToken(string appliesTo)
        {
            throw new NotImplementedException();
        }
    }
}
