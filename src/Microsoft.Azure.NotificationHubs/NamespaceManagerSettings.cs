//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Net.Http;
using Microsoft.Azure.NotificationHubs.Auth;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a namespace manager settings
    /// </summary>
    public sealed class NamespaceManagerSettings : NotificationHubClientSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManagerSettings"/> class.
        /// </summary>
        public NamespaceManagerSettings()
        {
            TokenProvider = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManagerSettings"/> class.
        /// </summary>
        public NamespaceManagerSettings(TokenProvider tokenProvider)
        {
            TokenProvider = tokenProvider;
        }

        /// <summary>
        /// Gets or sets the security token provider.
        /// </summary>
        /// 
        /// <returns>
        /// The security token provider.
        /// </returns>
        public TokenProvider TokenProvider { get; set; }
    }
}
