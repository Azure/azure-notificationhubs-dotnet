//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------
using System;
using System.Runtime.Serialization;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a Xiaomi credential.
    /// </summary>
    [DataContract(Name = ManagementStrings.XiaomiCredential, Namespace = ManagementStrings.Namespace)]
    public class XiaomiCredential : PnsCredential
    {
        internal const string AppPlatformName = "xiaomi";
        internal const string XiaomiEndpointName = "Endpoint";
        internal const string ProdChineseMainlandEndpoint = @"https://api.xmpush.xiaomi.com";
        internal const string ProdOtherEndpoint = @"https://api.xmpush.global.xiaomi.com";
        internal const string SandboxEndpoint = @"https://sandbox.xmpush.xiaomi.com";
        internal readonly string[] ValidEndpoints = new string[] { ProdChineseMainlandEndpoint, ProdOtherEndpoint, SandboxEndpoint };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.XiaomiCredential"/> class.
        /// </summary>
        public XiaomiCredential()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.XiaomiCredential"/> class.
        /// </summary>
        /// <param name="appSecret">The Xiaomi app secret key.</param>
        public XiaomiCredential(string appSecret)
        {
            this.AppSecret = appSecret;
        }

        /// <summary>
        /// Gets or sets the app secret
        /// </summary>
        /// 
        /// <returns>
        /// The app secret
        /// </returns>
        public string AppSecret
        {
            get { return base[nameof(AppSecret)]; }
            set { base[nameof(AppSecret)] = value; }
        }

        /// <summary>
        /// Gets or sets the endpoint
        /// </summary>
        /// 
        /// <returns>
        /// The endpoint
        /// </returns>
        public string Endpoint
        {
            get
            {
                if (base[XiaomiEndpointName] != null)
                {
                    return base[XiaomiEndpointName];
                }

                return ProdChineseMainlandEndpoint;
            }
            set { base[XiaomiEndpointName] = value; }
        }

        internal override string AppPlatform
        {
            get
            {
                return XiaomiCredential.AppPlatformName;
            }
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
            XiaomiCredential otherCredential = other as XiaomiCredential;
            if (otherCredential == null)
            {
                return false;
            }

            return (otherCredential.AppSecret == this.AppSecret && otherCredential.Endpoint == this.Endpoint);
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
            if (string.IsNullOrWhiteSpace(this.AppSecret))
            {
                return base.GetHashCode();
            }

            return AppSecret.GetHashCode();
        }

        /// <summary>Validates the MPNS credential.</summary>
        /// <param name="allowLocalMockPns">true to allow local mock PNS; otherwise, false.</param>
        protected override void OnValidate(bool allowLocalMockPns)
        {
            if (this.Properties == null || this.Properties.Count != 2)
            {
                throw new InvalidDataContractException(SRClient.AppSecretOrEndpointInvalid);
            }

            if (string.IsNullOrWhiteSpace(this.AppSecret) || string.IsNullOrWhiteSpace(this.Endpoint))
            {
                throw new InvalidDataContractException(SRClient.AppSecretOrEndpointInvalid);
            }

            if (!Uri.TryCreate(this.Endpoint, UriKind.Absolute, out Uri result) ||
                (!string.Equals(this.Endpoint, ProdChineseMainlandEndpoint,
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(this.Endpoint, ProdOtherEndpoint,
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(this.Endpoint, SandboxEndpoint,
                     StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidDataContractException(SRClient.AppSecretOrEndpointInvalid);
            }
        }
    }
}
