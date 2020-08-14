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
    /// Represents credential properties for a push notification service.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    [CollectionDataContract(Name = "Properties", Namespace = ManagementStrings.Namespace, ItemName = "Property", KeyName = "Name", ValueName = "Value")]
    public sealed class PnsCredentialProperties : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.Messaging.PnsCredentialProperties"/> class.
        /// </summary>
        public PnsCredentialProperties()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
