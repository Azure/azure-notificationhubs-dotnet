//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    public class BrowserTemplateRegistrationDescription : BrowserRegistrationDescription
    {
        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.BrowserTemplateRegistrationDescription"/> class by copying fields from the given instance
        /// </summary>
        /// <param name="sourceRegistration">Another <see cref="T:Microsoft.Azure.NotificationHubs.BrowserTemplateRegistrationDescription"/> instance whose fields values are copied from</param>
        public BrowserTemplateRegistrationDescription(BrowserTemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            BodyTemplate = sourceRegistration.BodyTemplate;
            TemplateName = sourceRegistration.TemplateName;
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.BrowserTemplateRegistrationDescription"/> class
        /// </summary>
        /// <param name="browserPushSubscription">The browser push subscription.</param>
        public BrowserTemplateRegistrationDescription(BrowserPushSubscription browserPushSubscription)
            : base(browserPushSubscription)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.BrowserTemplateRegistrationDescription"/> class
        /// </summary>
        /// <param name="browserPushSubscription">The browser push subscription.</param>
        /// <param name="jsonPayload">Payload template.</param>
        public BrowserTemplateRegistrationDescription(BrowserPushSubscription browserPushSubscription, string jsonPayload)
            : this(string.Empty, browserPushSubscription, jsonPayload, null)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1TemplateRegistrationDescription"/> class
        /// </summary>
        /// <param name="browserPushSubscription">The browser push subscription.</param>
        /// <param name="jsonPayload">Payload template.</param>
        /// <param name="tags">Collection of tags. Tags can be used for audience targeting purposes.</param>
        public BrowserTemplateRegistrationDescription(BrowserPushSubscription browserPushSubscription, string jsonPayload, IEnumerable<string> tags)
            : this(string.Empty, browserPushSubscription, jsonPayload, tags)
        {
        }

        internal BrowserTemplateRegistrationDescription(string notificationHubPath, BrowserPushSubscription browserPushSubscription, string jsonPayload, IEnumerable<string> tags)
            : base(notificationHubPath, browserPushSubscription, tags)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException(nameof(jsonPayload));
            }

            BodyTemplate = new CDataMember(jsonPayload);
        }

        /// <summary>
        /// Gets or sets a template body for notification payload which may contain placeholders to be filled in with actual data during the send operation
        /// </summary>
        [DataMember(Name = ManagementStrings.BodyTemplate, IsRequired = true, Order = 3001)]
        public CDataMember BodyTemplate { get; set; }

        /// <summary>
        /// Gets or sets a name of the template
        /// </summary>
        [DataMember(Name = ManagementStrings.TemplateName, IsRequired = false, Order = 3002)]
        public string TemplateName { get; set; }

        internal override string AppPlatForm
        {
            get
            {
                return BrowserCredential.AppPlatformName;
            }
        }

        internal override string RegistrationType
        {
            get
            {
                return RegistrationDescription.TemplateRegistrationType;
            }
        }

        internal override string PlatformType
        {
            get
            {
                return BrowserCredential.AppPlatformName + RegistrationDescription.TemplateRegistrationType;
            }
        }

        internal override void OnValidate()
        {
            base.OnValidate();

            try
            {
                using (XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(BodyTemplate), new XmlDictionaryReaderQuotas()))
                {
                    XDocument payloadDocument = XDocument.Load(reader);

                    foreach (XElement element in payloadDocument.Root.DescendantsAndSelf())
                    {
                        foreach (XAttribute attribute in element.Attributes())
                        {
                            ExpressionEvaluator.Validate(attribute.Value);
                        }

                        if (!element.HasElements && !string.IsNullOrEmpty(element.Value))
                        {
                            ExpressionEvaluator.Validate(element.Value);
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // We get an ugly and misleading error message when this exception happens -> The XmlReader state should be Interactive.
                // Hence we are using a more friendlier error message
                throw new XmlException(SRClient.FailedToDeserializeBodyTemplate);
            }

            ValidateTemplateName();
        }

        private void ValidateTemplateName()
        {
            if (TemplateName != null)
            {
                if (TemplateName.Length > RegistrationSDKHelper.TemplateMaxLength)
                {
                    throw new InvalidDataContractException(string.Format(SRClient.TemplateNameLengthExceedsLimit, RegistrationSDKHelper.TemplateMaxLength));
                }
            }
        }

        internal override RegistrationDescription Clone()
        {
            return new BrowserTemplateRegistrationDescription(this);
        }
    }
}
