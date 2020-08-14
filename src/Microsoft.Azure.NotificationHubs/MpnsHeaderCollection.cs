//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Provides a collection of MPNS header.
    /// </summary>
    [CollectionDataContract(Name = ManagementStrings.MpnsHeaders, Namespace = ManagementStrings.Namespace, ItemName = ManagementStrings.MpnsHeader, KeyName = "Header", ValueName = "Value")]
    public sealed class MpnsHeaderCollection : SortedDictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.MpnsHeaderCollection"/> class.
        /// </summary>
        public MpnsHeaderCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
