//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary>
    /// This represent the base abstract class for all the entity Description classes. 
    /// Mainly contain the read-only mechanics. This class is not meant to be used 
    /// or inherited by external customers.
    /// </summary>
    [DataContract(Namespace = ManagementStrings.Namespace)]    
    public abstract class EntityDescription : IExtensibleDataObject
    {
        // remarks: constructor is marked internal to prevent
        // external assemblies inheriting from it.
        internal EntityDescription()
        {
        }

        /// <summary>
        /// Indicate if the instance is read-only mode.
        /// </summary>
        /// <value>
        /// if true, setting any properties of this instance will return in a InvalidOperationException
        /// </value>
        public bool IsReadOnly { get; internal set; }

        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <value>
        /// Information describing the extension.
        /// </value>
        public ExtensionDataObject ExtensionData { get; set; }

        // This should return true if the description contains any secrets like SharedAccessKey.
        internal virtual bool RequiresEncryption
        {
            get { return false; }
        }

        /// <summary>
        /// Throw if the read only bool is set.
        /// </summary>
        protected void ThrowIfReadOnly()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(SRClient.ObjectIsReadOnly);
            }
        }
    }
}
