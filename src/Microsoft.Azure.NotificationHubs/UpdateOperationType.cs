//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    ///  Update Operation Types
    /// </summary>
    public enum UpdateOperationType
    {
        /// <summary>
        /// The add
        /// </summary>
        [EnumMember(Value = "add")]
        Add=1,

        /// <summary>
        /// The remove
        /// </summary>
        [EnumMember(Value = "remove")]
        Remove=2,

        /// <summary>
        /// The replace
        /// </summary>
        [EnumMember(Value = "replace")]
        Replace=3
    }
}