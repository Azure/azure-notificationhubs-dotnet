//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    internal static class MessagingUtilities
    {
        /// <summary>
        /// Create a list of uri addresses from a given list of string addresses
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public static IList<Uri> GetUriList(IList<string> addresses)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            var uriAddresses = new List<Uri>();

            foreach (string address in addresses)
            {
                try
                {
                    var uriAddress = new Uri(address);
                    ThrowIfNullAddressOrPathExists(uriAddress);
                    uriAddresses.Add(uriAddress);
                }
                catch (UriFormatException ex)
                {
                    throw new UriFormatException(SRClient.BadUriFormat(address), ex);
                }
            }

            if (uriAddresses.Count == 0)
            {
                throw new ArgumentException(SRClient.NoAddressesFound(uriAddresses), nameof(uriAddresses));
            }

            return uriAddresses;
        }

        public static void ThrowIfNullAddressOrPathExists(Uri address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!string.IsNullOrEmpty(address.AbsolutePath) && address.Segments.Length > 3)
            {
                throw new ArgumentException(SRClient.InvalidAddressPath(address.AbsoluteUri), nameof(address));
            }
        }

        public static void ThrowIfNullAddressesOrPathExists(IEnumerable<Uri> addresses)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            if (!addresses.Any())
            {
                throw new ArgumentException(SRClient.NoAddressesFound(addresses), nameof(addresses));
            }

            foreach (var address in addresses)
            {
                ThrowIfNullAddressOrPathExists(address);
            }
        }
    }
}
