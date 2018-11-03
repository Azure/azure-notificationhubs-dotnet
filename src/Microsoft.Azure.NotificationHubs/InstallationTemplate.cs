//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------


namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents template which may belong to an instance of <see cref="T:Microsoft.Azure.NotificationHubs.Installation"/> class
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptOut, ItemRequired = Required.Default)]
    public class InstallationTemplate
    {
        /// <summary>
        /// Gets or sets a template body for notification payload which may contain placeholders to be filled in with actual data during the send operation
        /// </summary>
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }

        /// <summary>
        /// Gets or set collection of headers applicable for MPNS-targeted notifications
        /// </summary>
        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets expiry applicable for APNS-targeted notifications
        /// </summary>
        [JsonProperty(PropertyName = "expiry", NullValueHandling = NullValueHandling.Ignore)]
        public string Expiry { get; set; }

        /// <summary>
        /// Gets or sets collection of tags for particular template. Ususaly only one tag (template name) should be here.
        /// </summary>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        [Obsolete]
        public IList<string> Tags { get; set; }
        
        internal string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}