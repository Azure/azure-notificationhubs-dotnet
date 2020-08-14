//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents the WNS secondary tile
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptOut, ItemRequired = Required.Default)]
    public class WnsSecondaryTile
    {
        /// <summary>
        /// Gets or sets the push channel.
        /// </summary>
        /// <value>
        /// The push channel.
        /// </value>
        [JsonProperty(Required = Required.Always, PropertyName = "pushChannel")]
        public string PushChannel { get; set; }

        /// <summary>
        /// Gets or sets the push channel expiration property.
        /// </summary>
        /// <value>
        /// The push channel expiration property.
        /// </value>
        [JsonProperty(PropertyName = "pushChannelExpired")]
        public bool? PushChannelExpired { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of Templates.
        /// </summary>
        /// <value>
        /// The Dictionary of templates.
        /// </value>
        [JsonProperty(PropertyName = "templates", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, InstallationTemplate> Templates { get; set; }
        
        internal string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}