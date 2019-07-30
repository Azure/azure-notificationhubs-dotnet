using System;

namespace Microsoft.Azure.NotificationHubs.Auth
{
    public class SharedSecretTokenProvider : TokenProvider
    {
        private readonly Uri stsUri;

        internal SharedSecretTokenProvider(string issuerName, string issuerSecret)
            : this(issuerName, SharedSecretTokenProvider.DecodeSecret(issuerSecret), TokenScope.Entity)
        {
        }

        internal SharedSecretTokenProvider(
            string issuerName,
            string issuerSecret,
            TokenScope tokenScope)
            : this(issuerName, SharedSecretTokenProvider.DecodeSecret(issuerSecret), tokenScope)
        {
        }

        internal SharedSecretTokenProvider(string issuerName, byte[] issuerSecret)
            : this(issuerName, issuerSecret, TokenScope.Entity)
        {
        }

        internal SharedSecretTokenProvider(
            string issuerName,
            byte[] issuerSecret,
            TokenScope tokenScope)
            : base(true, true, tokenScope)
        {
            if (string.IsNullOrEmpty(issuerName))
                throw new ArgumentException(SRClient.NullIssuerName, nameof(issuerName));
            if (issuerSecret == null || issuerSecret.Length == 0)
                throw new ArgumentException(SRClient.NullIssuerSecret, nameof(issuerSecret));
            this.IssuerName = issuerName;
            this.IssuerSecret = issuerSecret;
            this.stsUri = (Uri)null;
        }

        internal SharedSecretTokenProvider(string issuerName, string issuerSecret, Uri stsUri)
            : this(issuerName, issuerSecret, stsUri, TokenScope.Entity)
        {
        }

        internal SharedSecretTokenProvider(
            string issuerName,
            string issuerSecret,
            Uri stsUri,
            TokenScope tokenScope)
            : this(issuerName, SharedSecretTokenProvider.DecodeSecret(issuerSecret), stsUri, tokenScope)
        {
        }

        internal SharedSecretTokenProvider(string issuerName, byte[] issuerSecret, Uri stsUri)
            : this(issuerName, issuerSecret, stsUri, TokenScope.Entity)
        {
        }

        internal SharedSecretTokenProvider(
            string issuerName,
            byte[] issuerSecret,
            Uri stsUri,
            TokenScope tokenScope)
            : base(true, true, tokenScope)
        {
            if (string.IsNullOrEmpty(issuerName))
                throw new ArgumentException(SRClient.NullIssuerName, nameof(issuerName));
            if (issuerSecret == null || issuerSecret.Length == 0)
                throw new ArgumentException(SRClient.NullIssuerSecret, nameof(issuerSecret));
            if (stsUri == (Uri)null)
                throw new ArgumentNullException(nameof(stsUri));
            if (!stsUri.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
                throw new ArgumentException(SRClient.STSURIFormat, nameof(stsUri));
            this.IssuerName = issuerName;
            this.IssuerSecret = issuerSecret;
            this.stsUri = stsUri;
        }

        internal string IssuerName { get; }

        internal byte[] IssuerSecret { get; }

        internal static byte[] DecodeSecret(string issuerSecret)
        {
            try
            {
                return Convert.FromBase64String(issuerSecret);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException(SRClient.InvalidIssuerSecret, nameof(issuerSecret));
            }
        }

        protected override string GenerateToken(string appliesTo)
        {
            throw new NotImplementedException();
        }
    }
}
