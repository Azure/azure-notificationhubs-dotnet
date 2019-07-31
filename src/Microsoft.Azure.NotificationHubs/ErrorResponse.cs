//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
    [DataContract(Name = "Error", Namespace = "")]
    public class ErrorResponse
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Detail { get; set; }
    }
}
