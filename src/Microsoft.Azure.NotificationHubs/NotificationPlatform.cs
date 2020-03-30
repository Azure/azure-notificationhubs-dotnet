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
        /// FCM Installation Platform
        /// </summary>
        ///
        /// <remarks>
        /// The "gcm" value is intentional. As of version 3.0.0, all GCM related methods are hidden from
        /// the end-user in order to incentivize them to switch their code over to FCM methods. However,
        /// due to the backend having issues dealing with FCM models, the SDK converts them to GCM prior
        /// sending to the service, so the "Fcm" value will be sent as "Gcm". Both SDK and backend still
        /// transparently operate GCM models while allowing end-user to work with FCM classes.
        /// </remarks>
        [EnumMember(Value = "gcm")]
        Fcm=4,

        /// <summary>
        /// ADM Installation Platform
        /// </summary>
        [EnumMember(Value = "adm")]
        Adm=5,

        /// <summary>
        /// Baidu Installation Platform
        /// </summary>
        [EnumMember(Value = "baidu")]
        Baidu=6,
    }
}
