//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents an error response with error code and detail.
    /// </summary>
    [DataContract(Name = "Error", Namespace = "")]
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the error detail.
        /// </summary>
        [DataMember]
        public string Detail { get; set; }
    }
}
