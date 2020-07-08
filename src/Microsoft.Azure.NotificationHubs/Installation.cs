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
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents device in Azure Notification Hub
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptOut, ItemRequired = Required.Default)]
    public class Installation
    {
        /// <summary>
        /// Get or sets unique identifier for the installation
        /// </summary>
        [JsonProperty(PropertyName = "installationId")]
        public string InstallationId { get; set; }

        /// <summary>
        /// Gets or set registration id, token or URI obtained from platform-specific notification service
        /// </summary>
        [JsonProperty(PropertyName = "pushChannel")]
        public string PushChannel { get; set; }

        /// <summary>
        /// Gets if installation is expired or not
        /// </summary>
        [JsonProperty(PropertyName = "pushChannelExpired")]
        public bool? PushChannelExpired { get; set; }

        /// <summary>
        /// Gets or sets notification platform for the installation
        /// </summary>
        [JsonConverter(typeof (StringEnumConverter))]
        [JsonProperty(PropertyName = "platform")]
        public NotificationPlatform Platform { get; set; }

        /// <summary>
        /// Gets or sets expiration for the installation
        /// </summary>
        [JsonProperty(PropertyName = "expirationTime")]
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// Gets or sets collection of tags
        /// </summary>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets collection of push variables
        /// </summary>
        [JsonProperty(PropertyName = "pushVariables", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> PushVariables { get; set; }

        /// <summary>
        /// Gets or sets collection of templates
        /// </summary>
        [JsonProperty(PropertyName = "templates", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, InstallationTemplate> Templates { get; set; }

        /// <summary>
        /// Gets or sets collection of secondary tiles for WNS
        /// </summary>
        [JsonProperty(PropertyName = "secondaryTiles", NullValueHandling = NullValueHandling.Ignore)]
        [Obsolete]
        public IDictionary<string, WnsSecondaryTile> SecondaryTiles { get; set; }

        internal string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        internal static Installation FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Installation>(json);
        }
    }
}
