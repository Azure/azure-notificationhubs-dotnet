//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary>
    /// Enumerates the possible values for the status of a messaging entity.
    /// </summary>
    [DataContract(Name = ManagementStrings.EntityStatus, Namespace = ManagementStrings.Namespace)]
    public enum EntityStatus
    {
        /// <summary>
        /// Entity active
        /// </summary>
        [EnumMember]
        Active = 0,

        /// <summary>
        /// Entity disabled
        /// </summary>
        [EnumMember]
        Disabled = 1,

        /// <summary>
        /// Entity restoring
        /// </summary>
        [EnumMember]
        Restoring = 2,

        /// <summary>
        /// Entity send disabled
        /// </summary>
        [EnumMember]
        SendDisabled = 3,

        /// <summary>
        /// Entity receive disabled
        /// </summary>
        [EnumMember]
        ReceiveDisabled = 4,

        /// <summary>
        /// Entity creating
        /// </summary>
        [EnumMember]
        Creating = 5,

        /// <summary>
        /// Entity deleting
        /// </summary>
        [EnumMember]
        Deleting = 6,
    }
}
