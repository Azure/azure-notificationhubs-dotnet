using System;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    static class MessagingUtilities
    {
        public static void ThrowIfNullAddressOrPathExists(Uri address, string paramName)
        {
            if (address == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!string.IsNullOrEmpty(address.AbsolutePath) && address.Segments.Length > 3)
            {
                throw new InvalidArgumentException();
                throw Fx.Exception.Argument(paramName, SRClient.InvalidAddressPath(address.AbsoluteUri));
            }
        }
    }
}
