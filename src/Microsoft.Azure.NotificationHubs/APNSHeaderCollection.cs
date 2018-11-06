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
    /// Provides a collection of APNS header.
    /// </summary>
    [CollectionDataContract(Name = ManagementStrings.ApnsHeaders, Namespace = ManagementStrings.Namespace, ItemName = ManagementStrings.ApnsHeader, KeyName = "Header", ValueName = "Value")]
    public sealed class ApnsHeaderCollection : SortedDictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.ApnsHeaderCollection"/> class.
        /// </summary>
        public ApnsHeaderCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
