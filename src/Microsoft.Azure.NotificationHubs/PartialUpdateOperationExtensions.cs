﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Azure.NotificationHubs
{
    internal static class PartialUpdateOperationExtensions
    {
        internal static string ToJson(this IList<PartialUpdateOperation> operations)
        {
            if (operations == null) throw new ArgumentNullException("operations");
            return JsonConvert.SerializeObject(operations);
        }
    }
}