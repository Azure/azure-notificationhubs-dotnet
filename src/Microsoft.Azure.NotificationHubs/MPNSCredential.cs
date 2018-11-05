//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.Azure.NotificationHubs.Common;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Provides credential of Microsoft Push Notification Service (MPNS).
    /// </summary>
    [DataContract(Name = ManagementStrings.MpnsCredential, Namespace = ManagementStrings.Namespace)]
    public class MpnsCredential : PnsCredential
    {
        internal const string AppPlatformName = "windowsphone";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsCredential"/> class.
        /// </summary>
        public MpnsCredential()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsCredential"/> class.
        /// </summary>
        /// <param name="mpnsCertificate">The MPNS certificate.</param><param name="certificateKey">The certificate key.</param>
        public MpnsCredential(X509Certificate mpnsCertificate, string certificateKey)
            : this(MpnsCredential.ExportCertificateBytes(mpnsCertificate, certificateKey), certificateKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsCredential"/> class.
        /// </summary>
        /// <param name="certificateBuffer">The certificate buffer.</param><param name="certificateKey">The certificate key.</param>
        public MpnsCredential(byte[] certificateBuffer, string certificateKey)
            : this()
        {
            try
            {
                this.MpnsCertificate = Convert.ToBase64String(certificateBuffer);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("certificateBuffer", ex);
            }

            this.CertificateKey = certificateKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsCredential"/> class.
        /// </summary>
        /// <param name="certificatePath">The certificate path.</param><param name="certificateKey">The certificate key.</param>
        public MpnsCredential(string certificatePath, string certificateKey)
            : this()
        {
            try
            {
                this.MpnsCertificate = this.GetMpnsClientCertificate(certificatePath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("certificatePath", ex);
            }

            this.CertificateKey = certificateKey;
        }

        internal override string AppPlatform
        {
            get { return MpnsCredential.AppPlatformName; }
        }

        /// <summary>
        /// Gets or sets the MPNS certificate.
        /// </summary>
        /// <value>
        /// The MPNS certificate.
        /// </value>
        public string MpnsCertificate
        {
            get { return base["MpnsCertificate"]; }
            set { base["MpnsCertificate"] = value; }
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

        internal X509Certificate2 NativeCertificate { get; set; }

        private string GetMpnsClientCertificate(string certPath)
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
        /// Specifies whether the credential is the same with a specific object.
        /// </summary>
        /// 
        /// <returns>
        /// true if the credential is the same with a specific object; otherwise, false.
        /// </returns>
        /// <param name="other">The other object to compare.</param>
        public override bool Equals(object other)
        {
            MpnsCredential otherCredential = other as MpnsCredential;
            if (otherCredential == null)
            {
                return false;
            }

            return (otherCredential.CertificateKey == this.CertificateKey && otherCredential.MpnsCertificate == this.MpnsCertificate);
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
            if (string.IsNullOrWhiteSpace(this.CertificateKey) || string.IsNullOrWhiteSpace(this.MpnsCertificate))
            {
                return base.GetHashCode();
            }

            if (string.IsNullOrWhiteSpace(this.CertificateKey))
            {
                return this.MpnsCertificate.GetHashCode();
            }

            return unchecked(this.CertificateKey.GetHashCode() ^ this.MpnsCertificate.GetHashCode());
        }

        private static byte[] ExportCertificateBytes(X509Certificate mpnsCertificate, string certificateKey)
        {
            if (mpnsCertificate == null)
            {
                throw new ArgumentNullException("mpnsCertificate");
            }

            if (string.IsNullOrEmpty(certificateKey))
            {
                throw new ArgumentNullException("certificateKey");
            }

            return mpnsCertificate.Export(X509ContentType.Pkcs12, certificateKey);
        }
    }
}
