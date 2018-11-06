//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents the collection of Windows Push Notification Services (WNS) headers.
    /// </summary>
    [CollectionDataContract(Name = "WnsHeaders", Namespace = ManagementStrings.Namespace, ItemName = "WnsHeader", KeyName = "Header", ValueName = "Value")]
    public sealed class WnsHeaderCollection : SortedDictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.WnsHeaderCollection"/> class.
        /// </summary>
        public WnsHeaderCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
