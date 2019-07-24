//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents a WNS credential.
    /// </summary>
    [DataContract(Name = ManagementStrings.WnsCredential, Namespace = ManagementStrings.Namespace)]
    public class WnsCredential : PnsCredential
    {
        internal const string AppPlatformName = "windows";
        internal const string ProdAccessTokenServiceUrl = @"https://login.live.com/accesstoken.srf";

        /// <summary>
        /// Initializes a new instance of the <see cref="WnsCredential"/> class.
        /// </summary>
        public WnsCredential()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WnsCredential"/> class.
        /// </summary>
        /// <param name="packageSid">The package sid.</param>
        /// <param name="secretKey">The secret key.</param>
        public WnsCredential(string packageSid, string secretKey)
        {
            this.PackageSid = packageSid;
            this.SecretKey = secretKey;
        }

        internal override string AppPlatform
        {
            get
            {
                return WnsCredential.AppPlatformName;
            }
        }

        /// <summary>
        /// Gets or sets the package ID for this credential.
        /// </summary>
        /// 
        /// <returns>
        /// The package ID for this credential.
        /// </returns>
        public string PackageSid
        {
            get { return base["PackageSid"]; }
            set { base["PackageSid"] = value; }
        }

        /// <summary>
        /// Gets or sets the secret key.
        /// </summary>
        /// 
        /// <returns>
        /// The secret key.
        /// </returns>
        public string SecretKey
        {
            get { return base["SecretKey"]; }
            set { base["SecretKey"] = value; }
        }

        /// <summary>
        /// Gets or sets the Windows Live endpoint.
        /// </summary>
        /// 
        /// <returns>
        /// The Windows Live endpoint.
        /// </returns>
        public string WindowsLiveEndpoint
        {
            get { return base["WindowsLiveEndpoint"] ?? WnsCredential.ProdAccessTokenServiceUrl; }
            set { base["WindowsLiveEndpoint"] = value; }
        }

        /// <summary>
        /// Specifies whether this instance os the same as the specified object.
        /// </summary>
        /// 
        /// <returns>
        /// Returns <see cref="T:System.Boolean"/>.
        /// </returns>
        /// <param name="other">The other object to compare.</param>
        public override bool Equals(object other)
        {
            WnsCredential otherCredential = other as WnsCredential;
            if (otherCredential == null)
            {
                return false;
            }
            
            return (otherCredential.PackageSid == this.PackageSid && otherCredential.SecretKey == this.SecretKey);
        }

        /// <summary>
        /// Retrieves the hash code for the credentials.
        /// </summary>
        /// 
        /// <returns>
        /// The hash code for the credentials.
        /// </returns>
        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(this.PackageSid) || string.IsNullOrWhiteSpace(this.SecretKey))
            {
                return base.GetHashCode();
            }

            return unchecked(this.PackageSid.GetHashCode() ^ this.SecretKey.GetHashCode());
        }

        /// <summary>Validates the credential.</summary>
        /// <param name="allowLocalMockPns">true to allow local mock PNS; otherwise, false.</param>
        protected override void OnValidate(bool allowLocalMockPns)
        {
            if (this.Properties == null || this.Properties.Count > 3)
            {
                throw new InvalidDataContractException(SRClient.PackageSidAndSecretKeyAreRequired);
            }

            if (this.Properties.Count < 2 || string.IsNullOrWhiteSpace(this.PackageSid) ||
                string.IsNullOrWhiteSpace(this.SecretKey))
            {
                throw new InvalidDataContractException(SRClient.PackageSidOrSecretKeyInvalid);
            }

            if (this.Properties.Count == 3 && string.IsNullOrEmpty(this["WindowsLiveEndpoint"]))
            {
                throw new InvalidDataContractException(SRClient.PackageSidAndSecretKeyAreRequired);
            }

            if (!Uri.TryCreate(this.WindowsLiveEndpoint, UriKind.Absolute, out Uri result))
            {
                throw new InvalidDataContractException(SRClient.InvalidWindowsLiveEndpoint);
            }
        }
    }
}
