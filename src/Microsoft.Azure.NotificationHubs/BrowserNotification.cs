//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a browser notification.
    /// </summary>
    public class BrowserNotification : Notification, INativeNotification
    {
        private static string contentType = $"application/json;charset={Encoding.UTF8.WebName}";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.BrowserNotification"/> class.
        /// </summary>
        /// <param name="payload">The notification payload.</param>
        /// <param name="additionalHeaders">Additional headers for P256DH and Auth.</param>
        public BrowserNotification(string payload, IDictionary<string, string> additionalHeaders = null) : base(additionalHeaders, null, contentType)
        {
            Body = payload;
        }

        /// <summary>
        /// The platform type.
        /// </summary>
        protected override string PlatformType => BrowserCredential.AppPlatformName;

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected override void OnValidateAndPopulateHeaders()
        {
        }
    }
}
