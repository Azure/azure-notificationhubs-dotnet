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
    /// <summary>
    /// Represents the description of the Xiaomi template registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.XiaomiTemplateRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class XiaomiTemplateRegistrationDescription : XiaomiRegistrationDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XiaomiTemplateRegistrationDescription"/> class with specified source registration.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public XiaomiTemplateRegistrationDescription(XiaomiTemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.BodyTemplate = sourceRegistration.BodyTemplate;
            this.TemplateName = sourceRegistration.TemplateName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XiaomiTemplateRegistrationDescription"/> class with specified Xiaomi registration identifier.
        /// </summary>
        /// <param name="xiaomiRegistrationId">The Xiaomi registration identifier.</param>
        public XiaomiTemplateRegistrationDescription(string xiaomiRegistrationId)
            : base(xiaomiRegistrationId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XiaomiTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="xiaomiRegistrationId">The Xiaomi registration identifier.</param><param name="jsonPayload">The JSON payload.</param>
        public XiaomiTemplateRegistrationDescription(string xiaomiRegistrationId, string jsonPayload)
            : this(string.Empty, xiaomiRegistrationId, jsonPayload, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XiaomiTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="xiaomiRegistrationId">The Xiaomi registration identifier.</param><param name="jsonPayload">The JSON payload.</param><param name="tags">The description tags.</param>
        public XiaomiTemplateRegistrationDescription(string xiaomiRegistrationId, string jsonPayload, IEnumerable<string> tags)
            : this(string.Empty, xiaomiRegistrationId, jsonPayload, tags)
        {
        }

        internal XiaomiTemplateRegistrationDescription(string notificationHubPath, string xiaomiRegistrationId, string jsonPayload, IEnumerable<string> tags)
            : base(notificationHubPath, xiaomiRegistrationId, tags)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.BodyTemplate = new CDataMember(jsonPayload);
        }

        /// <summary>
        /// Gets or sets the body template.
        /// </summary>
        /// <value>
        /// The body template.
        /// </value>
        [DataMember(Name = ManagementStrings.BodyTemplate, IsRequired = true, Order = 3001)]
        public CDataMember BodyTemplate { get; set; }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>
        /// The name of the template.
        /// </value>
        [DataMember(Name = ManagementStrings.TemplateName, IsRequired = false, Order = 3002)]
        public string TemplateName { get; set; }

        internal override string AppPlatForm
        {
            get
            {
                // This needs to be changed to XiaomiCredential.AppPlatformName in a following PR when we add XiaomiCredential class
                return "xiaomi"; 
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
                // This needs to be changed to XiaomiCredential.AppPlatformName + RegistrationDescription.TemplateRegistrationType in a following PR when we add XiaomiCredential class
                return "xiaomi" + RegistrationDescription.TemplateRegistrationType; 
            }
        }

        internal override void OnValidate()
        {
            base.OnValidate();

            try
            {
                using (XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(this.BodyTemplate), new XmlDictionaryReaderQuotas()))
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

        internal override RegistrationDescription Clone()
        {
            return new XiaomiTemplateRegistrationDescription(this);
        }

        private void ValidateTemplateName()
        {
            if (this.TemplateName != null)
            {
                if (this.TemplateName.Length > RegistrationSDKHelper.TemplateMaxLength)
                {
                    throw new InvalidDataContractException(string.Format(SRClient.TemplateNameLengthExceedsLimit, RegistrationSDKHelper.TemplateMaxLength));
                }
            }
        }
    }
}
