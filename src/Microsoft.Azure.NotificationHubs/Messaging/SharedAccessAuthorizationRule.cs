//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Web;
using Microsoft.Azure.NotificationHubs.Common;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary>
    /// Defines the authorization rule for shared access operation.
    /// </summary>
    [DataContract(Name = ManagementStrings.SharedAccessAuthorizationRule, Namespace = ManagementStrings.Namespace)]
    public class SharedAccessAuthorizationRule : AuthorizationRule
    {
        const int SupportedSASKeyLength = 44;
        const string FixedClaimType = "SharedAccessKey";
        const string FixedClaimValue = "None";

        /// <summary>
        /// This is done to help json and other deserializers
        /// </summary>
        SharedAccessAuthorizationRule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.SharedAccessAuthorizationRule" /> class.
        /// </summary>
        /// <param name="keyName">The authorization rule key name.</param>
        /// <param name="rights">The list of rights.</param>
        public SharedAccessAuthorizationRule(string keyName, IEnumerable<AccessRights> rights)
            : this(keyName, SharedAccessAuthorizationRule.GenerateRandomKey(), SharedAccessAuthorizationRule.GenerateRandomKey(), rights)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.SharedAccessAuthorizationRule" /> class.
        /// </summary>
        /// <param name="keyName">The authorization rule key name.</param>
        /// <param name="primaryKey">The primary key for the authorization rule.</param>
        /// <param name="rights">The list of rights.</param>
        public SharedAccessAuthorizationRule(string keyName, string primaryKey, IEnumerable<AccessRights> rights)
            : this(keyName, primaryKey, SharedAccessAuthorizationRule.GenerateRandomKey(), rights)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.SharedAccessAuthorizationRule" /> class.
        /// </summary>
        /// <param name="keyName">The authorization rule key name.</param>
        /// <param name="primaryKey">The primary key for the authorization rule.</param>
        /// <param name="secondaryKey">The secondary key for the authorization rule.</param>
        /// <param name="rights">The list of rights.</param>
        public SharedAccessAuthorizationRule(string keyName, string primaryKey, string secondaryKey, IEnumerable<AccessRights> rights)
        {
            this.ClaimType = SharedAccessAuthorizationRule.FixedClaimType;
            this.ClaimValue = SharedAccessAuthorizationRule.FixedClaimValue;
            this.PrimaryKey = primaryKey;
            this.SecondaryKey = secondaryKey;
            this.Rights = rights;
            this.KeyName = keyName;
        }

        /// <summary>
        /// Checks the validity of the authorization rule.
        /// </summary>
        protected override void OnValidate()
        {
            if (string.IsNullOrEmpty(this.InternalKeyName) || !string.Equals(this.InternalKeyName, HttpUtility.UrlEncode(this.InternalKeyName)))
            {    
                throw new InvalidDataContractException(SRCore.SharedAccessAuthorizationRuleKeyContainsInvalidCharacters);
            }  

            if (this.InternalKeyName.Length > 256)
            {    
                throw new InvalidDataContractException(SRCore.SharedAccessAuthorizationRuleKeyNameTooBig((object) 256));
            }           
            if (string.IsNullOrEmpty(this.InternalPrimaryKey))
            {    
                throw new InvalidDataContractException(SRCore.SharedAccessAuthorizationRuleRequiresPrimaryKey);
            }    

            if (Encoding.ASCII.GetByteCount(this.InternalPrimaryKey) != 44)
            {
                throw new InvalidDataContractException(SRCore.SharedAccessRuleAllowsFixedLengthKeys((object) 44));
            }
            
            if (!SharedAccessAuthorizationRule.CheckBase64(this.InternalPrimaryKey))
            {
                throw new InvalidDataContractException(SRCore.SharedAccessKeyShouldbeBase64);
            }

            if (!string.IsNullOrEmpty(this.InternalSecondaryKey))
            {
                if (Encoding.ASCII.GetByteCount(this.InternalSecondaryKey) != 44)
                throw new InvalidDataContractException(SRCore.SharedAccessRuleAllowsFixedLengthKeys((object) 44));
                if (!SharedAccessAuthorizationRule.CheckBase64(this.InternalSecondaryKey))
                throw new InvalidDataContractException(SRCore.SharedAccessKeyShouldbeBase64);
            }

            if (!SharedAccessAuthorizationRule.IsValidCombinationOfRights(this.Rights))
            {
                throw new InvalidDataContractException(SRClient.InvalidCombinationOfManageRight);
            }
        }

        static bool CheckBase64(string base64EncodedString)
        {
            try
            {
                Convert.FromBase64String(base64EncodedString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the name of the key.
        /// </summary>
        /// <value>
        /// The name of the key.
        /// </value>
        /// <exception cref="System.ArgumentNullException">Thrown in case the key is null or a whitespace.</exception>
        /// <exception cref="System.ArgumentException">Thrown in case the key is not properly URL encoded</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown in case the key name lenght is greater than maximum allowed</exception>
        public override sealed string KeyName
        {
            get { return this.InternalKeyName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {    
                    throw new ArgumentNullException(nameof (KeyName));
                }

                if (!string.Equals(this.InternalKeyName, HttpUtility.UrlEncode(this.InternalKeyName)))
                {    
                    throw new ArgumentException(SRCore.SharedAccessAuthorizationRuleKeyContainsInvalidCharacters);
                }

                if (value.Length > 256)
                {    
                    throw new ArgumentOutOfRangeException(nameof (KeyName), SRCore.ArgumentStringTooBig((object) nameof (KeyName), (object) 256));
                
                }
                this.InternalKeyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the primary key.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        /// <exception cref="System.ArgumentNullException">Thrown in case the key is null or a whitespace.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown in case the key lenght is greater than maximum allowed</exception>
        public string PrimaryKey
        {
            get { return this.InternalPrimaryKey; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof (PrimaryKey));
                }

                if (value.Length > 256)
                {   
                    throw new ArgumentOutOfRangeException(nameof (PrimaryKey), SRCore.ArgumentStringTooBig((object) nameof (PrimaryKey), (object) 256));
                
                }

                this.InternalPrimaryKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the secondary key.
        /// </summary>
        /// <value>
        /// The secondary key.
        /// </value>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown in case the key lenght is greater than maximum allowed</exception>
        public string SecondaryKey
        {
            get { return this.InternalSecondaryKey; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {    
                    this.InternalSecondaryKey = value;
                }    
                else if (value.Length > 256)
                {    
                    throw new ArgumentOutOfRangeException(nameof (SecondaryKey), SRCore.ArgumentStringTooBig((object) nameof (SecondaryKey), (object) 256));
                }

                this.InternalSecondaryKey = value;
            }
        }

        /// <summary>
        /// Checks the validity of the specified access rights.
        /// </summary>
        /// <param name="value">
        /// The access rights to check.
        /// </param>
        protected override void ValidateRights(IEnumerable<AccessRights> value)
        {
            base.ValidateRights(value);

            if (!SharedAccessAuthorizationRule.IsValidCombinationOfRights(value))
            {    
                throw new ArgumentException(SRClient.InvalidCombinationOfManageRight);
            }    
        }

        static bool IsValidCombinationOfRights(IEnumerable<AccessRights> rights)
        {
            return !rights.Contains(AccessRights.Manage) || rights.Count() == 3;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        /// The hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            int result = base.GetHashCode();

            foreach (string value in new string[] { this.KeyName, this.PrimaryKey, this.SecondaryKey })
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result += value.GetHashCode();
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object.</param>
        public override bool Equals(Object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            SharedAccessAuthorizationRule comparand = (SharedAccessAuthorizationRule)obj;
            return string.Equals(this.KeyName, comparand.KeyName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(this.PrimaryKey, comparand.PrimaryKey, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(this.SecondaryKey, comparand.SecondaryKey, StringComparison.OrdinalIgnoreCase);
        }

        [DataMember(Name = ManagementStrings.KeyName, IsRequired = true, Order = 1001, EmitDefaultValue = false)]
        internal string InternalKeyName { get; set; }

        [DataMember(Name = ManagementStrings.PrimaryKey, IsRequired = true, Order = 1002, EmitDefaultValue = false)]
        internal string InternalPrimaryKey { get; set; }

        [DataMember(Name = ManagementStrings.SecondaryKey, IsRequired = false, Order = 1003, EmitDefaultValue = false)]
        internal string InternalSecondaryKey { get; set; }

        /// <summary>
        /// Generates a Random base 64 encoded Key with crypto apis
        /// </summary>
        /// <returns>Random Key</returns>
        public static string GenerateRandomKey()
        {
            byte[] key256 = new byte[32];
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rngCryptoServiceProvider.GetBytes(key256);
            }

            return Convert.ToBase64String(key256);
        }
    }
}
