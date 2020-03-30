﻿//------------------------------------------------------------
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
    /// Represents Baidu credentials
    /// </summary>
    [DataContract(Name = ManagementStrings.BaiduCredential, Namespace = ManagementStrings.Namespace)]
    public class BaiduCredential : PnsCredential
    {
        internal const string AppPlatformName = "baidu";
        internal const string ProdAccessTokenServiceUrl = @"https://channel.api.duapp.com/rest/2.0/channel/channel";

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduCredential"/> class.
        /// </summary>
        public BaiduCredential() : base()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduCredential"/> class.
        /// </summary>
        /// <param name="baiduApiKey">The Baidu API key.</param>
        public BaiduCredential(string baiduApiKey)
        {
            BaiduApiKey = baiduApiKey;
        }

        /// <summary>
        /// Gets or sets the Baidu API key.
        /// </summary>
        /// <value>
        /// The Baidu API key.
        /// </value>
        public string BaiduApiKey
        {
            get { return base["BaiduApiKey"]; }
            set { base["BaiduApiKey"] = value; }
        }

        /// <summary>
        /// Gets or sets the Baidu end point.
        /// </summary>
        /// <value>
        /// The Baidu end point.
        /// </value>
        public string BaiduEndPoint
        {
            get { return base["BaiduEndPoint"] ?? BaiduCredential.ProdAccessTokenServiceUrl; }
            set { base["BaiduEndPoint"] = value; }
        }

        /// <summary>
        /// Gets or sets the Baidu secret key.
        /// </summary>
        /// <value>
        /// The Baidu secret key.
        /// </value>
        public string BaiduSecretKey
        {
            get { return base["BaiduSecretKey"]; }
            set { base["BaiduSecretKey"] = value; }
        }

        internal override string AppPlatform
        {
            get { return BaiduCredential.AppPlatformName; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var cre = obj as BaiduCredential;
            if (cre == null)
            {
                return false;
            }

            return cre.BaiduApiKey == this.BaiduApiKey;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(this.BaiduApiKey))
            {
                return base.GetHashCode();
            }

            return this.BaiduApiKey.GetHashCode();
        }

        /// <summary>Validates the credential.</summary>
        /// <param name="allowLocalMockPns">true to allow local mock PNS; otherwise, false.</param>
        /// <exception cref="T:System.Runtime.Serialization.InvalidDataContractException">
        /// </exception>
        protected override void OnValidate(bool allowLocalMockPns)
        {
            if (this.Properties == null || this.Properties.Count > 2)
                throw new InvalidDataContractException(SRClient.BaiduRequiredProperties);
            if (this.Properties.Count < 1 || string.IsNullOrWhiteSpace(this.BaiduApiKey))
                throw new InvalidDataContractException(SRClient.BaiduApiKeyNotSpecified);
            bool flag = !string.Equals(this.BaiduEndPoint, "https://channel.api.duapp.com/rest/2.0/channel/channel", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "https://nnapi.ovi.com/nnapi/2.0/send", StringComparison.OrdinalIgnoreCase) && (!string.Equals(this.BaiduEndPoint, "http://pushtestservice.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "http://pushtestservice4.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase)) && (!string.Equals(this.BaiduEndPoint, "http://pushperfnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "http://pushstressnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase) && !string.Equals(this.BaiduEndPoint, "http://pushnotificationserver.cloudapp.net/gcm/send", StringComparison.OrdinalIgnoreCase)) && (!allowLocalMockPns || !string.Equals(this.BaiduEndPoint, "http://localhost:8450/gcm/send", StringComparison.OrdinalIgnoreCase));
            Uri result;
            if (!Uri.TryCreate(this.BaiduEndPoint, UriKind.Absolute, out result) | flag)
                throw new InvalidDataContractException(SRClient.InvalidBaiduEndpoint);
        }
    }
}
