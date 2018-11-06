//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Supported Intallation Platforms
    /// </summary>
    public enum NotificationPlatform
    {
        /// <summary>
        /// WNS Installation Platform
        /// </summary>
        [EnumMember(Value = "wns")]
        Wns=1,

        /// <summary>
        /// APNS Installation Platform
        /// </summary>
        [EnumMember(Value = "apns")]
        Apns=2,

        /// <summary>
        /// MPNS Installation Platform
        /// </summary>
        [EnumMember(Value = "mpns")]
        Mpns=3,

        /// <summary>
        /// GCM Installation Platform
        /// </summary>
        [EnumMember(Value = "gcm")]
        Gcm=4,

        /// <summary>
        /// ADM Installation Platform
        /// </summary>
        [EnumMember(Value = "adm")]
        Adm=5,
    }
}