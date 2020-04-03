//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents the description of the Amazon Device Messaging (ADM) template registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.AdmTemplateRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class AdmTemplateRegistrationDescription : AdmRegistrationDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AdmTemplateRegistrationDescription"/> class with specified source registration.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public AdmTemplateRegistrationDescription(AdmTemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.BodyTemplate = sourceRegistration.BodyTemplate;
            this.TemplateName = sourceRegistration.TemplateName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AdmTemplateRegistrationDescription"/> class with specified Amazon Device Messaging registration identifier.
        /// </summary>
        /// <param name="admRegistrationId">The Amazon Device Messaging registration identifier.</param>
        public AdmTemplateRegistrationDescription(string admRegistrationId)
            : base(admRegistrationId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AdmTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="admRegistrationId">The Amazon Device Messaging registration identifier.</param><param name="jsonPayload">The JSON payload.</param>
        public AdmTemplateRegistrationDescription(string admRegistrationId, string jsonPayload)
            : this(string.Empty, admRegistrationId, jsonPayload, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AdmTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="admRegistrationId">The Amazon Device Messaging registration identifier.</param><param name="jsonPayload">The JSON payload.</param><param name="tags">The description tags.</param>
        public AdmTemplateRegistrationDescription(string admRegistrationId, string jsonPayload, IEnumerable<string> tags)
            : this(string.Empty, admRegistrationId, jsonPayload, tags)
        {
        }

        internal AdmTemplateRegistrationDescription(string notificationHubPath, string admRegistrationId, string jsonPayload, IEnumerable<string> tags)
            : base(notificationHubPath, admRegistrationId, tags)
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
                return AdmCredential.AppPlatformName;
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
                return AdmCredential.AppPlatformName + RegistrationDescription.TemplateRegistrationType;
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
            return new AdmTemplateRegistrationDescription(this);
        }

        private void ValidateTemplateName()
        {
            if (this.TemplateName != null)
            {
                if (this.TemplateName.Length > RegistrationSDKHelper.TemplateMaxLength)
                {
                    throw new InvalidDataContractException(SRClient.TemplateNameLengthExceedsLimit(RegistrationSDKHelper.TemplateMaxLength));
                }
            }
        }
    }
}
