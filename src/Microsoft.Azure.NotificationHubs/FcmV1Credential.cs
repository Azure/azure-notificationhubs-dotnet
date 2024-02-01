//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Runtime.Serialization;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents the Firebase Cloud Messaging V1 credential.
    /// </summary>
    [DataContract(Name = ManagementStrings.FcmV1Credential, Namespace = ManagementStrings.Namespace)]
    public class FcmV1Credential : PnsCredential
    {
        internal const string AppPlatformName = "fcmv1";
        internal const string ProdAccessTokenServiceUrl = @"https://fcm.googleapis.com/v1/projects/ProjectId/messages:send";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1Credential"/> class.
        /// </summary>
        public FcmV1Credential()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1Credential"/> class.
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <param name="projectId">Project ID</param>
        /// <param name="clientEmail">Client Email</param>
        public FcmV1Credential(string privateKey, string projectId, string clientEmail)
        {
            PrivateKey = privateKey;
            ProjectId = projectId;
            ClientEmail = clientEmail;
        }

        /// <summary>
        /// Gets the FCM endpoint.
        /// </summary>
        /// 
        /// <returns>
        /// The FCM endpoint.
        /// </returns>
        public string FcmV1Endpoint
        {
            get { return ProdAccessTokenServiceUrl.Replace(nameof(ProjectId), base[nameof(ProjectId)]); }
        }

        /// <summary>
        /// Gets or sets the Private Key.
        /// </summary>
        /// 
        /// <returns>
        /// The Private Key
        /// </returns>
        public string PrivateKey
        {
            get { return base[nameof(PrivateKey)]; }
            set { base[nameof(PrivateKey)] = value; }
        }

        /// <summary>
        /// Gets or sets the Project ID.
        /// </summary>
        /// 
        /// <returns>
        /// The Project ID
        /// </returns>
        public string ProjectId
        {
            get { return base[nameof(ProjectId)]; }
            set { base[nameof(ProjectId)] = value; }
        }

        /// <summary>
        /// Gets or sets the Client Email.
        /// </summary>
        /// 
        /// <returns>
        /// The Client Email
        /// </returns>
        public string ClientEmail
        {
            get { return base[nameof(ClientEmail)]; }
            set { base[nameof(ClientEmail)] = value; }
        }

        internal override string AppPlatform
        {
            get { return FcmV1Credential.AppPlatformName; }
        }

        /// <summary>
        /// Specifies whether the credential is equal with the specific object.
        /// </summary>
        /// 
        /// <returns>
        /// true if the credential is equal with the specific object; otherwise, false.
        /// </returns>
        /// <param name="other">The other object to compare.</param>
        public override bool Equals(object other)
        {
            FcmV1Credential otherCredential = other as FcmV1Credential;
            if (otherCredential == null)
            {
                return false;
            }

            return (otherCredential.PrivateKey == PrivateKey && otherCredential.ProjectId == ProjectId && otherCredential.ClientEmail == ClientEmail);
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
            if (string.IsNullOrWhiteSpace(PrivateKey))
            {
                return base.GetHashCode();
            }

            return unchecked(PrivateKey.GetHashCode() ^ ProjectId.GetHashCode() ^ ClientEmail.GetHashCode());
        }

        /// <summary>Validates the FCM V1 credential.</summary>
        /// <param name="allowLocalMockPns">true to allow local mock PNS; otherwise, false.</param>
        protected override void OnValidate(bool allowLocalMockPns)
        {
            if (Properties == null || Properties.Count > 3)
            {
                throw new InvalidDataContractException(SRClient.FcmV1RequiredProperties);
            }

            if (string.IsNullOrWhiteSpace(PrivateKey))
            {
                throw new InvalidDataContractException(SRClient.FcmV1PrivateKeyNotSpecified);
            }

            if (string.IsNullOrWhiteSpace(ProjectId))
            {
                throw new InvalidDataContractException(SRClient.FcmV1ProjectIdNotSpecified);
            }

            if (string.IsNullOrWhiteSpace(ClientEmail))
            {
                throw new InvalidDataContractException(SRClient.FcmV1ClientEmailNotSpecified);
            }
        }
    }
}
