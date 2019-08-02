//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.Azure.NotificationHubs.Auth
{
    /// <summary>
    /// Represents a OAuth token provider
    /// </summary>
    public class OAuthTokenProvider : TokenProvider
    {
        private readonly List<Uri> _stsUris;
        private readonly NetworkCredential _credential;

        internal OAuthTokenProvider(IEnumerable<Uri> stsUris, NetworkCredential credential)
            : base(true, true, 100, TokenScope.Namespace)
        {
            if (stsUris == null)
                throw new ArgumentNullException(nameof(stsUris));
            _stsUris = stsUris.ToList<Uri>();
            if (_stsUris.Count == 0)
                throw new ArgumentNullException(nameof(stsUris));
            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
        }

        protected override string GenerateToken(string appliesTo)
        {
            throw new NotImplementedException();
        }
    }
}
