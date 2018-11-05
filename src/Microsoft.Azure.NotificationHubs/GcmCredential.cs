//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using Microsoft.Azure.NotificationHubs.Messaging;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the Google Cloud Messaging credential.
    /// </summary>
    [DataContract(Name = ManagementStrings.GcmCredential, Namespace = ManagementStrings.Namespace)]
    public class GcmCredential : PnsCredential
    {
        internal const string AppPlatformName = "gcm";
        internal const string ProdAccessTokenServiceUrl = @"https://android.googleapis.com/gcm/send";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.GcmCredential"/> class.
        /// </summary>
        public GcmCredential()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.GcmCredential"/> class.
        /// </summary>
        /// <param name="googleApiKey">The Google API key.</param>
        public GcmCredential(string googleApiKey)
        {
            this.GoogleApiKey = googleApiKey;
        }

        /// <summary>
        /// Gets or sets the GCM endpoint.
        /// </summary>
        /// 
        /// <returns>
        /// The GCM endpoint.
        /// </returns>
        public string GcmEndpoint
        {
            get { return base["GcmEndpoint"] ?? GcmCredential.ProdAccessTokenServiceUrl; }
            set { base["GcmEndpoint"] = value; }
        }

        /// <summary>
        /// Gets or sets the Google API key.
        /// </summary>
        /// 
        /// <returns>
        /// The Google API key.
        /// </returns>
        public string GoogleApiKey
        {
            get { return base["GoogleApiKey"]; }
            set { base["GoogleApiKey"] = value; }
        }

        internal override string AppPlatform
        {
            get { return GcmCredential.AppPlatformName; }
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
            GcmCredential otherCredential = other as GcmCredential;
            if (otherCredential == null)
            {
                return false;
            }

            return (otherCredential.GoogleApiKey == this.GoogleApiKey);
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
            if (string.IsNullOrWhiteSpace(this.GoogleApiKey))
            {
                return base.GetHashCode();
            }

            return this.GoogleApiKey.GetHashCode();
        }
    }
}