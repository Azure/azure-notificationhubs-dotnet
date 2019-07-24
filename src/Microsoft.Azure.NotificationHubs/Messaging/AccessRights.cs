//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//-----------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    /// <summary>
    /// Specifies the possible access rights for a user.
    /// </summary>
    [DataContract(Name = ManagementStrings.AccessRights, Namespace = ManagementStrings.Namespace)]
    public enum AccessRights
    {
        /// <summary>
        /// Allows management operations such as udpating and deleting on the notification hub.  Manage also include Send and Listen rights.
        /// </summary>
        [EnumMember]
        Manage = 0,

        /// <summary>
        /// Right to send messages only.
        /// </summary>
        [EnumMember]
        Send = 1,

        /// <summary>
        /// Right to receive messages only.
        /// </summary>
        [EnumMember]
        Listen = 2
    }
}
