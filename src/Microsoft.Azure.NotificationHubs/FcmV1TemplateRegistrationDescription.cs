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
using System.Xml.Linq;
using System.Xml;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace Microsoft.Azure.NotificationHubs
{
    /// <summary>
    /// Represents Notification Hub template registration description for Firebase Cloud Messaging V1
    /// </summary>
    [DataContract(Name = ManagementStrings.FcmV1TemplateRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class FcmV1TemplateRegistrationDescription : FcmV1RegistrationDescription
    {
        internal override string AppPlatForm
        {
            get
            {
                return FcmV1Credential.AppPlatformName;
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
                return FcmV1Credential.AppPlatformName + RegistrationDescription.TemplateRegistrationType;
            }
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1TemplateRegistrationDescription"/> class by copying fields from the given instance
        /// </summary>
        /// <param name="sourceRegistration">Another <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1TemplateRegistrationDescription"/> instance fields values are copyed from</param>
        public FcmV1TemplateRegistrationDescription(FcmV1TemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.BodyTemplate = sourceRegistration.BodyTemplate;
            this.TemplateName = sourceRegistration.TemplateName;
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1TemplateRegistrationDescription"/> class using given Firebase Cloud Messaging registration id
        /// </summary>
        /// <param name="fcmV1RegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        public FcmV1TemplateRegistrationDescription(string fcmV1RegistrationId)
            : base(fcmV1RegistrationId)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1TemplateRegistrationDescription"/> class using given Firebase Cloud Messaging registration id and template for payload
        /// </summary>
        /// <param name="fcmV1RegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        /// <param name="jsonPayload">Payload template</param>
        public FcmV1TemplateRegistrationDescription(string fcmV1RegistrationId, string jsonPayload)
            : this(string.Empty, fcmV1RegistrationId, jsonPayload, null)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmV1TemplateRegistrationDescription"/> class using given Firebase Cloud Messaging registration id, template for payload and collection of tags
        /// </summary>
        /// <param name="fcmV1RegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        /// <param name="jsonPayload">Payload template</param>
        /// <param name="tags">Collection of tags. Tags can be used for audience targeting purposes.</param>
        public FcmV1TemplateRegistrationDescription(string fcmV1RegistrationId, string jsonPayload, IEnumerable<string> tags)
            : this(string.Empty, fcmV1RegistrationId, jsonPayload, tags)
        {
        }

        internal FcmV1TemplateRegistrationDescription(string notificationHubPath, string fcmV1RegistrationId, string jsonPayload, IEnumerable<string> tags)
            : base(notificationHubPath, fcmV1RegistrationId, tags)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
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
            return new FcmV1TemplateRegistrationDescription(this);
        }
    }
}
