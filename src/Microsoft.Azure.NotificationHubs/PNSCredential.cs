//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents a PNS credential.
    /// </summary>
    [DataContract(Name = ManagementStrings.PnsCredential, Namespace = ManagementStrings.Namespace)]
    [KnownType(typeof(ApnsCredential))]
    [KnownType(typeof(GcmCredential))]
    [KnownType(typeof(FcmCredential))]
    [KnownType(typeof(MpnsCredential))]
    [KnownType(typeof(WnsCredential))]
    [KnownType(typeof(AdmCredential))]
    public abstract class PnsCredential : EntityDescription
    {
        internal PnsCredential()
        {
            this.Properties = new PnsCredentialProperties();
        }

        internal abstract string AppPlatform
        {
            get;
        }

        /// <summary>
        /// Gets or sets the properties of this credential.
        /// </summary>
        /// 
        /// <returns>
        /// The properties of this credential.
        /// </returns>
        [DataMember(IsRequired = true)]
        public PnsCredentialProperties Properties { get; set; }

        /// <summary>
        /// Gets or sets the time and date this credential is blocked on.
        /// </summary>
        /// 
        /// <returns>
        /// The time and date this credential is blocked on.
        /// </returns>
        [DataMember(Name = "BlockedOn", IsRequired = false, EmitDefaultValue = false)]
        public DateTime? BlockedOn { get; set; }

        /// <summary>
        /// Gets or sets the value associated with this credential.
        /// </summary>
        /// 
        /// <returns>
        /// The value associated with this credential.
        /// </returns>
        /// <param name="name">The name of the credential.</param>
        protected string this[string name]
        {
            get
            {
                if (this.Properties.ContainsKey(name))
                {
                    return this.Properties[name];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (this.Properties.ContainsKey(name))
                {
                    this.Properties[name] = value;
                }
                else
                {
                    this.Properties.Add(name, value);
                }
            }
        }

        /// <summary>
        /// Specifies whether the two credentials are equal.
        /// </summary>
        /// 
        /// <returns>
        /// true if the two credentials are equal; otherwise, false.
        /// </returns>
        /// <param name="cred1">The first credential to compare.</param><param name="cred2">The second credential to compare.</param>
        public static bool IsEqual(PnsCredential cred1, PnsCredential cred2)
        {
            if (cred1 == null || cred2 == null)
            {
                return cred1 == cred2;
            }

            return cred1.Equals(cred2);
        }
    }
}
