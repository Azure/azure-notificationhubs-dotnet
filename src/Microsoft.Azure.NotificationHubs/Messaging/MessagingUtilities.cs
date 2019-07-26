using System;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    public static class MessagingUtilities
    {
        public static void ThrowIfNullAddressOrPathExists(Uri address, string paramName)
        {
            if (address == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!string.IsNullOrEmpty(address.AbsolutePath) && address.Segments.Length > 3)
            {
                throw new ArgumentException(SRClient.InvalidAddressPath(address.AbsoluteUri), paramName);
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
                throw new ArgumentNullException("addresses");
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
                ThrowIfNullAddressOrPathExists(uriAddress, "uriAddress");
                uriAddresses.Add(uriAddress);
            }

            if (uriAddresses.Count == 0)
            {
                ThrowIfNullAddressOrPathExists(uriAddress, "uriAddress");
            }

            return uriAddresses;
        }

        public static void ThrowIfNullAddressesOrPathExists(IEnumerable<Uri> addresses, string paramName)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (addresses.Count() == 0)
            {
                throw new ArgumentException(SRClient.NoAddressesFound(addresses), "addresses");
            }

            foreach (var address in addresses)
            {
                ThrowIfNullAddressOrPathExists(address, "address");
            }
        }
    }
}
