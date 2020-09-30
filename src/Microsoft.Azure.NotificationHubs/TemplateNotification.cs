//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents a template notification.
    /// </summary>
    public sealed class TemplateNotification : Notification
    {
        static string contentType = $"application/json;charset={Encoding.UTF8.WebName}";
        IDictionary<string, string> templateProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.TemplateNotification" /> class.
        /// </summary>
        /// <param name="templateProperties">The template properties.</param>
        /// <exception cref="System.ArgumentNullException">properties</exception>
        public TemplateNotification(IDictionary<string, string> templateProperties)
            : base(null, null, contentType)
        {
            if (templateProperties == null)
            {
                throw new ArgumentNullException("properties");
            }

            this.templateProperties = templateProperties;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.TemplateNotification" /> class.
        /// </summary>
        /// <param name="templateProperties">The template properties.</param>
        /// <param name="tag">The notification tag.</param>
        /// <exception cref="System.ArgumentNullException">properties</exception>
        [Obsolete("This method is obsolete.")]
        public TemplateNotification(IDictionary<string, string> templateProperties, string tag)
            : base(null, tag, contentType)
        {
            if (templateProperties == null)
            {
                throw new ArgumentNullException("properties");
            }

            this.templateProperties = templateProperties;
        }

        /// <summary>
        /// Gets the type of the platform.
        /// </summary>
        /// <value>
        /// The type of the platform.
        /// </value>
        protected override string PlatformType
        {
            get { return RegistrationDescription.TemplateRegistrationType; }
        }

        /// <summary>
        /// Validate and populates the headers.
        /// </summary>
        protected override void OnValidateAndPopulateHeaders()
        {
            this.Body = JsonConvert.SerializeObject(this.templateProperties);
        }
    }
}
