using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.Azure.NotificationHubs.Auth
{
    public class WindowsTokenProvider : TokenProvider
    {
        internal readonly List<Uri> stsUris;
        internal readonly NetworkCredential credential;

        internal WindowsTokenProvider(IEnumerable<Uri> stsUris)
            : this(stsUris, (NetworkCredential)null)
        {
        }

        internal WindowsTokenProvider(IEnumerable<Uri> stsUris, NetworkCredential credential)
            : base(true, true, 100, TokenScope.Namespace)
        {
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
