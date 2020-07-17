//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary>
    /// Defines the Azure Notification Hubs authorization rule that is used to determine whether an operation is permissible or not.
    /// </summary>
    [DataContract(Namespace = ManagementStrings.Namespace)]
    [KnownType(typeof(SharedAccessAuthorizationRule))]
    public abstract class AuthorizationRule
    {
        /// <summary>
        /// The name identifier claim rule.
        /// </summary>
        public const string NameIdentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        /// <summary>
        /// The short name identifier claim rule.
        /// </summary>
        public const string ShortNameIdentifierClaimType = "nameidentifier";
        /// <summary>
        /// The UPN claim rule.
        /// </summary>
        public const string UpnClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
        /// <summary>
        /// The short UPN claim rule.
        /// </summary>
        public const string ShortUpnClaimType = "upn";
        /// <summary>
        /// The role claim rule.
        /// </summary>
        public const string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        /// <summary>
        /// The role role claim rule.
        /// </summary>
        public const string RoleRoleClaimType = "role";
        /// <summary>
        /// The shared access key claim rule.
        /// </summary>
        public const string SharedAccessKeyClaimType = "sharedaccesskey";

        internal AuthorizationRule()
        {
            CreatedTime = DateTime.UtcNow;
            ModifiedTime = DateTime.UtcNow;
            Revision = 0L;
        }

        /// <summary>
        /// Gets or sets the name identifier of the issuer.
        /// </summary>
        /// 
        /// <returns>
        /// The name identifier of the issuer.
        /// </returns>
        public string IssuerName 
        {
            get { return this.InternalIssuerName; }
            set
            {
                this.InternalIssuerName = value;
            }
        }

        /// <summary>
        /// Gets or sets the claim type.
        /// </summary>
        /// 
        /// <returns>
        /// The claim type.
        /// </returns>
        public string ClaimType
        {
            get { return this.InternalClaimType; }
            set
            {
                this.InternalClaimType = value;
            }
        }

        /// <summary>
        /// Gets or sets the claim value which is either ‘Send’, ‘Listen’, or ‘Manage’.
        /// </summary>
        /// 
        /// <returns>
        /// The claim value which is either ‘Send’, ‘Listen’, or ‘Manage’.
        /// </returns>
        public string ClaimValue
        {
            get { return this.InternalClaimValue; }
            set
            {
                this.InternalClaimValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of rights.
        /// </summary>
        /// 
        /// <returns>
        /// The list of rights.
        /// </returns>
        public IEnumerable<AccessRights> Rights
        {
            get { return this.InternalRights; }
            set
            {
                this.InternalRights = value;
            }
        }

        /// <summary>
        /// Gets or sets the authorization rule key name.
        /// </summary>
        /// 
        /// <returns>
        /// The authorization rule key name.
        /// </returns>
        public abstract String KeyName { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the authorization rule was created.
        /// </summary>
        /// 
        /// <returns>
        /// The date and time when the authorization rule was created.
        /// </returns>
        [DataMember(IsRequired = false, Order = 1006, EmitDefaultValue = false)]
        public DateTime CreatedTime { get; private set; }

        /// <summary>
        /// Gets or sets the date and time when the authorization rule was modified.
        /// </summary>
        /// 
        /// <returns>
        /// The date and time when the authorization rule was modified.
        /// </returns>
        [DataMember(IsRequired = false, Order = 1007, EmitDefaultValue = false)]
        public DateTime ModifiedTime { get; private set; }

        /// <summary>
        /// Gets or sets the modification revision number.
        /// </summary>
        /// 
        /// <returns>
        /// The modification revision number.
        /// </returns>
        [DataMember(IsRequired = false, Order = 1008, EmitDefaultValue = false)]
        public long Revision { get; set; }

        /// <summary>
        /// Called when [validate].
        /// </summary>
        protected virtual void OnValidate()
        {
        }

        /// <summary>
        /// Checks the validity of the specified access rights.
        /// </summary>
        /// <param name="value">The access rights to check.</param>
        protected virtual void ValidateRights(IEnumerable<AccessRights> value)
        {
            if (value == null || !value.Any<AccessRights>() || value.Count<AccessRights>() > 3)
            {
                throw new ArgumentException(string.Format(SRClient.NullEmptyRights, 3));
            }
                
            if (!AuthorizationRule.AreAccessRightsUnique(value))
            {
                throw new ArgumentException(SRClient.CannotHaveDuplicateAccessRights);
            } 
        }

        internal void MarkModified()
        {
            this.ModifiedTime = DateTime.UtcNow;
            ++this.Revision;
        }

        internal void Validate()
        {
            if (this.Rights == null || !this.Rights.Any<AccessRights>() || this.Rights.Count<AccessRights>() > 3)
            {
                throw new InvalidDataContractException(string.Format(SRClient.NullEmptyRights, 3));
            }
                
            if (!AuthorizationRule.AreAccessRightsUnique(this.Rights))
            {
                throw new InvalidDataContractException(SRClient.CannotHaveDuplicateAccessRights);
            }
                
            this.OnValidate();
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
            int result = 0;

            foreach (string value in new string[] { this.IssuerName, this.ClaimValue, this.ClaimType })
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result += value.GetHashCode();
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a copy of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A created copy of <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.AuthorizationRule"/>.
        /// </returns>
        public virtual AuthorizationRule Clone()
        {
            return (AuthorizationRule) this.MemberwiseClone();
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
            if (!(this.GetType() == obj.GetType()))
            {
                return false;
            }

            AuthorizationRule comparand = (AuthorizationRule) obj;
            if (!string.Equals(this.IssuerName, comparand.IssuerName, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(this.ClaimType, comparand.ClaimType, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(this.ClaimValue, comparand.ClaimValue, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if ((this.Rights != null && comparand.Rights == null) ||
                (this.Rights == null && comparand.Rights != null))
            {
                return false;
            }

            if (this.Rights != null && comparand.Rights != null)
            {
                HashSet<AccessRights> thisRights = new HashSet<AccessRights>(this.Rights);
                HashSet<AccessRights> comparandRights = new HashSet<AccessRights>(comparand.Rights);

                if (comparandRights.Count != thisRights.Count)
                {
                    return false;
                }

                return thisRights.All(comparandRights.Contains);
            }

            return true;
        }

        [DataMember(Name = ManagementStrings.IssuerName, IsRequired = false, Order = 1002, EmitDefaultValue = false)]
        internal string InternalIssuerName { get; set; }

        [DataMember(Name = ManagementStrings.ClaimType, IsRequired = false, Order = 1003, EmitDefaultValue = false)]
        internal string InternalClaimType { get; set; }

        [DataMember(Name = ManagementStrings.ClaimValue, IsRequired = true, Order = 1004, EmitDefaultValue = false)]
        internal string InternalClaimValue { get; set; }

        [DataMember(Name = ManagementStrings.Rights, IsRequired = false, Order = 1005, EmitDefaultValue = false)]
        internal IEnumerable<AccessRights> InternalRights { get; set; }

        static bool AreAccessRightsUnique(IEnumerable<AccessRights> rights )
        {
            HashSet<AccessRights> dedupedAccessRights = new HashSet<AccessRights>(rights);
            return rights.Count() == dedupedAccessRights.Count;
        }
    }
}
