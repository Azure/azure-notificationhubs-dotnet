//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Runtime.Serialization;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    [DataContract(Name = ManagementStrings.BrowserCredential, Namespace = ManagementStrings.Namespace)]
    public class BrowserCredential : PnsCredential
    {
        internal const string AppPlatformName = "browser";

        /// <summary>
        /// Gets or sets web push subject
        /// </summary>
        public string Subject
        {
            get { return base[nameof(Subject)]; }
            set { base[nameof(Subject)] = value; }
        }

        /// <summary>
        /// Gets or sets VAPID public key.
        /// </summary>
        public string VapidPublicKey
        {
            get { return base[nameof(VapidPublicKey)]; }
            set { base[nameof(VapidPublicKey)] = value; }
        }

        /// <summary>
        /// Gets or sets VAPID private key.
        /// </summary>
        public string VapidPrivateKey
        {
            get { return base[nameof(VapidPrivateKey)]; }
            set { base[nameof(VapidPrivateKey)] = value; }
        }

        internal override string AppPlatform => AppPlatformName;

        /// <summary>
        /// Specifies whether the credential is equal with the specific object.
        /// </summary>
        /// <returns>
        /// true if the credential is equal with the specific object; otherwise, false.
        /// </returns>
        /// <param name="other">The other object to compare.</param>
        public override bool Equals(object other)
        {
            var otherCredential = other as BrowserCredential;
            if (otherCredential == null)
            {
                return false;
            }

            return (otherCredential.Subject == Subject && otherCredential.VapidPublicKey == VapidPublicKey && otherCredential.VapidPrivateKey == VapidPrivateKey);
        }

        /// <summary>
        /// Retrieves the hash code for the credentials.
        /// </summary>
        /// <returns>
        /// The hash code for the credentials.
        /// </returns>
        public override int GetHashCode()
        {
            return unchecked(Subject.GetHashCode() ^ VapidPublicKey.GetHashCode() ^ VapidPrivateKey.GetHashCode());
        }

        /// <summary>Validates the browser credential.</summary>
        /// <param name="allowLocalMockPns">true to allow local mock PNS; otherwise, false.</param>
        protected override void OnValidate(bool allowLocalMockPns)
        {
            if (Properties == null || Properties.Count > 3)
            {
                throw new InvalidDataContractException(SRClient.BrowserRequiredProperties);
            }

            if (string.IsNullOrWhiteSpace(Subject))
            {
                throw new InvalidDataContractException(SRClient.BrowserSubjectNotSpecified);
            }

            if (string.IsNullOrWhiteSpace(VapidPublicKey))
            {
                throw new InvalidDataContractException(SRClient.BrowserVapidPublicKeyNotSpecified);
            }

            if (string.IsNullOrWhiteSpace(VapidPrivateKey))
            {
                throw new InvalidDataContractException(SRClient.BrowserVapidPrivateKeyNotSpecified);
            }
        }
    }
}
