using System;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    public static class MessagingUtilities
    {
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

        /// <summary>
        /// Create a list of uri addresses from a given list of string addresses
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
        public static IEnumerable<Uri> GetUriList(IEnumerable<string> addresses)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            List<Uri> uriAddresses = new List<Uri>();

            Uri uriAddress = null;
            foreach (string address in addresses)
            {
                try
                {
                    uriAddress = new Uri(address);
                }
                catch (UriFormatException ex)
                {
                    throw new UriFormatException(SRClient.BadUriFormat(address), ex);
                }
                ThrowIfNullAddressOrPathExists(uriAddress);
                uriAddresses.Add(uriAddress);
            }

            if (uriAddresses.Count == 0)
            {
                ThrowIfNullAddressOrPathExists(uriAddress);
            }

            return uriAddresses;
        }

        public static void ThrowIfNullAddressesOrPathExists(IEnumerable<Uri> addresses)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            if (addresses.Count() == 0)
            {
                throw new ArgumentException(SRClient.NoAddressesFound(addresses), "addresses");
            }

            foreach (var address in addresses)
            {
                ThrowIfNullAddressOrPathExists(address);
            }
        }
    }
}
