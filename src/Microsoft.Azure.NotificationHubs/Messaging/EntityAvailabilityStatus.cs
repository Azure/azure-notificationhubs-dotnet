//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{    
    /// <summary>
    /// Enumerates the status of the entity availability.
    /// </summary>
    [DataContract(Name = ManagementStrings.EntityAvailabilityStatus, Namespace = ManagementStrings.Namespace)]
    public enum EntityAvailabilityStatus
    {
        /// <summary>
        /// Indicates the entity is in unknown state or faulted state
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Indicates the entity in ready available state
        /// </summary>
        [EnumMember]
        Available = 1,

        /// <summary>
        /// Indicates the entity is in limited state
        /// </summary>
        [EnumMember]
        Limited = 2,

        /// <summary>
        /// Indicates the entity is currently being restored
        /// </summary>
        [EnumMember]
        Restoring = 3
    }
}