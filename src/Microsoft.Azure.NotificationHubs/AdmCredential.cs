//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.NotificationHubs
{
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents an ADM credential
    /// </summary>
    [DataContract(Name = ManagementStrings.AdmCredential, Namespace = ManagementStrings.Namespace)]
    public class AdmCredential : PnsCredential
    {
        internal const string AppPlatformName = "adm";
        internal const string ProdAuthTokenUrl = @"https://api.amazon.com/auth/O2/token";
        internal const string ProdSendUrlTemplate = @"https://api.amazon.com/messaging/registrations/{0}/messages";

        private const string ClientIdName = "ClientId";
        private const string ClientSecretName = "ClientSecret";
        private const string AuthTokenUrlName = "AuthTokenUrl";
        private const string SendUrlTemplateName = "SendUrlTemplate";
        
        private const string MockAuthTokenUrl = @"http://localhost:8450/adm/token";
        private const string MockSendUrlTemplate = @"http://localhost:8450/adm/send/{0}/messages";
        private const string MockIntSendUrlTemplate = @"http://pushtestservice4.cloudapp.net/adm/send/{0}/messages";
        private const string MockIntAuthTokenUrl = @"http://pushtestservice4.cloudapp.net/adm/token";

        private const string RequiredPropertiesList = "ClientId, ClientSecret";

        /// <summary>
        /// Initializes a new instance of the <see cref="AdmCredential"/> class.
        /// </summary>
        public AdmCredential()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdmCredential"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientSecret">The client secret.</param>
        public AdmCredential(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// 
        /// <returns>
        /// The client identifier.
        /// </returns>
        public string ClientId
        {
            get { return base[ClientIdName]; }
            set { base[ClientIdName] = value; }
        }

        /// <summary>
        /// Gets or sets the credential secret access key.
        /// </summary>
        /// 
        /// <returns>
        /// The credential secret access key.
        /// </returns>
        public string ClientSecret
        {
            get { return base[ClientSecretName]; }
            set { base[ClientSecretName] = value; }
        }

        /// <summary>
        /// Gets or sets the URL of the authorization token.
        /// </summary>
        /// 
        /// <returns>
        /// The URL of the authorization token.
        /// </returns>
        public string AuthTokenUrl
        {
            get { return base[AuthTokenUrlName] ?? ProdAuthTokenUrl; }
            set { base[AuthTokenUrlName] = value; }
        }

        /// <summary>
        /// Gets or sets the sending URL template.
        /// </summary>
        /// 
        /// <returns>
        /// The sending URL template.
        /// </returns>
        public string SendUrlTemplate
        {
            get { return base[SendUrlTemplateName] ?? ProdSendUrlTemplate; }
            set { base[SendUrlTemplateName] = value; }
        }

        internal override string AppPlatform
        {
            get { return AppPlatformName; }
        }

        /// <summary>
        /// Specifies whether this credential is equal to the specified object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>
        /// true if this credential is equal with the specific object; otherwise, false.
        /// </returns>
        public override bool Equals(object other)
        {
            var otherCredential = other as AdmCredential;
            if (otherCredential == null)
            {
                return false;
            }

            return otherCredential.ClientId == ClientId && otherCredential.ClientSecret == ClientSecret;
        }

        /// <summary>
        /// Retrieves the hash code for the credential.
        /// </summary>
        /// 
        /// <returns>
        /// The hash code for the credential.
        /// </returns>
        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(ClientId) || string.IsNullOrWhiteSpace(ClientSecret))
            {
                return base.GetHashCode();
            }

            return unchecked(ClientId.GetHashCode() ^ ClientSecret.GetHashCode());
        }

        /// <summary>
        /// Validates the Amazon Device Messaging credentials.
        /// </summary>
        /// <param name="allowLocalMockPns">true to allow local mock PNS; otherwise, false.</param>
        /// <exception cref="System.Runtime.Serialization.InvalidDataContractException">
        /// </exception>
        protected override void OnValidate(bool allowLocalMockPns)
        {
            if (Properties == null ||
                (string.IsNullOrEmpty(Properties[ClientIdName]) && string.IsNullOrEmpty(Properties[ClientSecretName])))
            {
                throw new InvalidDataContractException(SRClient.RequiredPropertiesNotSpecified(RequiredPropertiesList));
            }

            if (string.IsNullOrEmpty(Properties[ClientIdName]))
            {
                throw new InvalidDataContractException(SRClient.RequiredPropertyNotSpecified(ClientIdName));
            }

            if (string.IsNullOrEmpty(Properties[ClientSecretName]))
            {
                throw new InvalidDataContractException(SRClient.RequiredPropertyNotSpecified(ClientSecretName));
            }

            if (Properties.Count > 2 &&
                Properties.Count > Properties.Keys.Intersect(
                    new[] { ClientIdName, ClientSecretName, SendUrlTemplateName, AuthTokenUrlName }).Count())
            {
                throw new InvalidDataContractException(SRClient.OnlyNPropertiesRequired(2, RequiredPropertiesList));
            }

            Uri uri;

            if (!Uri.TryCreate(AuthTokenUrl, UriKind.Absolute, out uri) || (
                !string.Equals(AuthTokenUrl, ProdAuthTokenUrl, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(AuthTokenUrl, MockIntAuthTokenUrl, StringComparison.OrdinalIgnoreCase) &&
                !(allowLocalMockPns && string.Equals(AuthTokenUrl, MockAuthTokenUrl, StringComparison.OrdinalIgnoreCase))))
            {
                throw new InvalidDataContractException(SRClient.InvalidAdmAuthTokenUrl);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(SendUrlTemplate) ||
                    !Uri.TryCreate(string.Format(CultureInfo.InvariantCulture, SendUrlTemplate, "AdmRegistrationId"), UriKind.Absolute, out uri) ||
                    (!string.Equals(SendUrlTemplate, ProdSendUrlTemplate, StringComparison.OrdinalIgnoreCase) &&
                     !string.Equals(SendUrlTemplate, MockIntSendUrlTemplate, StringComparison.OrdinalIgnoreCase) &&
                     !(allowLocalMockPns && string.Equals(SendUrlTemplate, MockSendUrlTemplate, StringComparison.OrdinalIgnoreCase))))
                {
                    throw new InvalidDataContractException(SRClient.InvalidAdmSendUrlTemplate);
                }
            }
            catch (FormatException)
            {
                throw new InvalidDataContractException(SRClient.InvalidAdmSendUrlTemplate);
            }
        }
    }
}
