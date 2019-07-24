//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Security.Cryptography;
using Microsoft.Azure.NotificationHubs.Messaging;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents an Apple Push Notification Service (APNS) credential.
    /// </summary>
    [DataContract(Name = ManagementStrings.ApnsCredential, Namespace = ManagementStrings.Namespace)]
    public class ApnsCredential : PnsCredential
    {
        internal const string AppPlatformName = "apple";
        internal const string ApnsGatewayEndpoint = "gateway.push.apple.com";

		/// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.ApnsCredential"/> class.
        /// </summary>
        public ApnsCredential()
            : base()
        {
            this.Endpoint = ApnsGatewayEndpoint;
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.ApnsCredential"/> class.
        /// </summary>
        /// <param name="certificateBuffer">The certificate buffer.</param><param name="certificateKey">The certificated key.</param>
        public ApnsCredential(byte[] certificateBuffer, string certificateKey)
            : this()
        {
            try
            {
                this.ApnsCertificate = Convert.ToBase64String(certificateBuffer);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("certificateBuffer", ex);
            }

            this.CertificateKey = certificateKey;
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.ApnsCredential"/> class.
        /// </summary>
        /// <param name="certificatePath">The certificate path.</param><param name="certificateKey">The certificate key.</param>
        public ApnsCredential(string certificatePath, string certificateKey)
            : this()
        {
            try
            {
                this.ApnsCertificate = this.GetApnsClientCertificate(certificatePath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("certificatePath", ex);
            }

            this.CertificateKey = certificateKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.ApnsCredential"/> class.
        /// </summary>
        /// <param name="token">The authentication token.</param><param name="keyId">The key Id from the APNs Auth Key Page.</param><param name="appId">The application prefix from the iOS App IDs Page.</param><param name="appName">The application id from the iOS App IDs Page.</param>
        public ApnsCredential(string token, string keyId, string appId, string appName)
            : this()
        {
            this.Token = token;
            this.KeyId = keyId;
            this.AppId = appId;
            this.AppName = appName;
        }

        internal override string AppPlatform
        {
            get
            {
                return ApnsCredential.AppPlatformName;
            }
        }

		/// <summary>
        /// Gets or sets the APNS certificate.
        /// </summary>
        /// <value>
        /// The APNS certificate.
        /// </value>
        public string ApnsCertificate
        {
            get { return base["ApnsCertificate"]; }
            set { base["ApnsCertificate"] = value; }
        }

		/// <summary>
        /// Gets or sets the certificate key.
        /// </summary>
        /// <value>
        /// The certificate key.
        /// </value>
        public string CertificateKey
        {
            get { return base["CertificateKey"]; }
            set { base["CertificateKey"] = value; }
        }

		/// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint
        {
            get { return base["Endpoint"]; }
            set { base["Endpoint"] = value; }
        }

		/// <summary>
        /// Gets or sets the Thumbprint.
        /// </summary>
        /// <value>
        /// The Thumbprint.
        /// </value>
        public string Thumbprint
        {
            get { return base["Thumbprint"]; }
            internal set { base["Thumbprint"] = value; }
        }

		/// <summary>
        /// Gets or sets the Token.
        /// </summary>
        /// <value>
        /// The Token.
        /// </value>
        public string Token
        {
            get { return base["Token"]; }
            set { base["Token"] = value; }
        }

		/// <summary>
        /// Gets or sets the KeyId.
        /// </summary>
        /// <value>
        /// The KeyId.
        /// </value>
        public string KeyId
        {
            get { return base["KeyId"]; }
            set { base["KeyId"] = value; }
        }

		/// <summary>
        /// Gets or sets the AppName.
        /// </summary>
        /// <value>
        /// The AppName.
        /// </value>
        public string AppName
        {
            get { return base["AppName"]; }
            set { base["AppName"] = value; }
        }

		/// <summary>
        /// Gets or sets the AppId.
        /// </summary>
        /// <value>
        /// The AppId.
        /// </value>
        public string AppId
        {
            get { return base["AppId"]; }
            set { base["AppId"] = value; }
        }

        internal X509Certificate2 NativeCertificate
        {
            get;
            set;
        }

        string GetApnsClientCertificate(string certPath)
        {
            using (FileStream fs = File.OpenRead(certPath))
            {
                byte[] certificateBuffer;
                using (var memoryStream = new MemoryStream())
                {
                    fs.CopyTo(memoryStream);
                    certificateBuffer = memoryStream.ToArray();
                }

                return Convert.ToBase64String(certificateBuffer);
            }
        }

		/// <summary>
        /// Specifies whether the credential is the same as the specific object.
        /// </summary>
        /// 
        /// <returns>
        /// true if the credential is the same as the specific object; otherwise, false.
        /// </returns>
        /// <param name="other">The other object to compare.</param>
        public override bool Equals(object other)
        {
            ApnsCredential otherCredential = other as ApnsCredential;
            if (otherCredential == null)
            {
                return false;
            }

            return (otherCredential.Endpoint == this.Endpoint && otherCredential.CertificateKey == this.CertificateKey && otherCredential.ApnsCertificate == this.ApnsCertificate
                && otherCredential.Token == this.Token && otherCredential.KeyId == this.KeyId && otherCredential.AppName == this.AppName && otherCredential.AppId == this.AppId);
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
            if ((string.IsNullOrWhiteSpace(this.CertificateKey) || string.IsNullOrWhiteSpace(this.ApnsCertificate)) && string.IsNullOrWhiteSpace(this.Token))
            {
                return base.GetHashCode();
            }

            if (string.IsNullOrWhiteSpace(this.CertificateKey) && string.IsNullOrWhiteSpace(this.Token))
            {
                return this.ApnsCertificate.GetHashCode();
            }

            if (!string.IsNullOrWhiteSpace(this.Token))
            {
                return this.Token.GetHashCode();
            }

            return unchecked(this.CertificateKey.GetHashCode() ^ this.ApnsCertificate.GetHashCode());
        }

        /// <summary>Validates the APNS credential.</summary>
        /// <param name="allowLocalMockPns">true to allow local mock PNS; otherwise, false.</param>
        protected override void OnValidate(bool allowLocalMockPns)
        {
            if (this.Properties == null)
            {
                throw new InvalidDataContractException(SRClient.ApnsRequiredPropertiesError);
            }

            if (string.IsNullOrWhiteSpace(this.Endpoint))
            {
                throw new InvalidDataContractException(SRClient.ApnsEndpointNotSpecified);
            }

            if (string.IsNullOrWhiteSpace(this.Token) && string.IsNullOrWhiteSpace(this.ApnsCertificate))
            {
                throw new InvalidDataContractException(SRClient.ApnsPropertiesNotSpecified);
            }

            if (!string.IsNullOrWhiteSpace(this.Token) && !string.IsNullOrWhiteSpace(this.ApnsCertificate))
            {
                throw new InvalidDataContractException(SRClient.ApnsProvideOnlyOneCredentialType);
            }

            if (!string.IsNullOrWhiteSpace(this.Token) && (string.IsNullOrWhiteSpace(this.KeyId) ||
                                                           string.IsNullOrWhiteSpace(this.AppId) ||
                                                           string.IsNullOrWhiteSpace(this.AppName)))
            {
                throw new InvalidDataContractException(SRClient.ApnsTokenPropertiesMissing);
            }

            if (!string.IsNullOrWhiteSpace(this.Token))
            {
                return;
            }
            try
            {
                this.NativeCertificate = this.CertificateKey == null ? new X509Certificate2(Convert.FromBase64String(this.ApnsCertificate)) : new X509Certificate2(Convert.FromBase64String(this.ApnsCertificate), this.CertificateKey);
                if (!this.NativeCertificate.HasPrivateKey)
                {
                    throw new InvalidDataContractException(SRClient.ApnsCertificatePrivatekeyMissing);
                }

                if (DateTime.UtcNow > this.NativeCertificate.NotAfter)
                {
                    throw new InvalidDataContractException(SRClient.ApnsCertificateExpired);
                }

                if (DateTime.UtcNow < this.NativeCertificate.NotBefore)
                {
                    throw new InvalidDataContractException(SRClient.ApnsCertificateNotValid);
                }
            }
            catch (CryptographicException ex)
            {
                throw new InvalidDataContractException(SRClient.ApnsCertificateNotUsable((object)ex.Message));
            }
            catch (FormatException ex)
            {
                throw new InvalidDataContractException(SRClient.ApnsCertificateNotUsable((object)ex.Message));
            }
        }
    }
}
