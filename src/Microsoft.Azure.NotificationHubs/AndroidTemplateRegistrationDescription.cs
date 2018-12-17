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
    using System.Xml.Linq;
    using System.Xml;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents Notification Hub template registration description for Google Cloud Messaging
    /// </summary>
    [DataContract(Name = ManagementStrings.GcmTemplateRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    [Obsolete("GcmTemplateRegistrationDescription is deprecated, please use FcmTemplateRegistrationDescription instead.")]
    public class GcmTemplateRegistrationDescription : GcmRegistrationDescription
    {
        internal override string AppPlatForm
        {
            get
            {
                return GcmCredential.AppPlatformName;
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
                return GcmCredential.AppPlatformName + RegistrationDescription.TemplateRegistrationType;
            }
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.GcmTemplateRegistrationDescription"/> class by copying fields from the given instance
        /// </summary>
        /// <param name="sourceRegistration">Another <see cref="T:Microsoft.Azure.NotificationHubs.GcmTemplateRegistrationDescription"/> instance fields values are copyed from</param>
        public GcmTemplateRegistrationDescription(GcmTemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.BodyTemplate = sourceRegistration.BodyTemplate;
            this.TemplateName = sourceRegistration.TemplateName;
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.GcmTemplateRegistrationDescription"/> class using given Google Cloud Messaging registration id
        /// </summary>
        /// <param name="gcmRegistrationId">Registration id obtained from the Google Cloud Messaging service</param>
        public GcmTemplateRegistrationDescription(string gcmRegistrationId)
            : base(gcmRegistrationId)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.GcmTemplateRegistrationDescription"/> class using given Google Cloud Messaging registration id and template for payload
        /// </summary>
        /// <param name="gcmRegistrationId">Registration id obtained from the Google Cloud Messaging service</param>
        /// <param name="jsonPayload">Payload template</param>
        public GcmTemplateRegistrationDescription(string gcmRegistrationId, string jsonPayload)
            : this(string.Empty, gcmRegistrationId, jsonPayload, null)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.GcmTemplateRegistrationDescription"/> class using given Google Cloud Messaging registration id, template for payload and collection of tags
        /// </summary>
        /// <param name="gcmRegistrationId">Registration id obtained from the Google Cloud Messaging service</param>
        /// <param name="jsonPayload">Payload template</param>
        /// <param name="tags">Collection of tags. Tags can be used for audience targeting purposes.</param>
        public GcmTemplateRegistrationDescription(string gcmRegistrationId, string jsonPayload, IEnumerable<string> tags)
            : this(string.Empty, gcmRegistrationId, jsonPayload, tags)
        {
        }

        internal GcmTemplateRegistrationDescription(string notificationHubPath, string gcmRegistrationId, string jsonPayload, IEnumerable<string> tags)
            : base(notificationHubPath, gcmRegistrationId, tags)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.BodyTemplate = new CDataMember(jsonPayload);
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

        internal override void OnValidate(ApiVersion version)
        {
            base.OnValidate(version);

            try
            {
                using (XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(this.BodyTemplate), new XmlDictionaryReaderQuotas()))
                {
                    XDocument payloadDocument = XDocument.Load(reader);

                    foreach (XElement element in payloadDocument.Root.DescendantsAndSelf())
                    {
                        foreach (XAttribute attribute in element.Attributes())
                        {
                            ExpressionEvaluator.Validate(attribute.Value, version);
                        }

                        if (!element.HasElements && !string.IsNullOrEmpty(element.Value))
                        {
                            ExpressionEvaluator.Validate(element.Value, version);
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

            this.ValidateTemplateName();
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

        internal override RegistrationDescription Clone()
        {
            return new GcmTemplateRegistrationDescription(this);
        }
    }

     /// <summary>
    /// Represents Notification Hub template registration description for Firebase Cloud Messaging
    /// </summary>
    [DataContract(Name = ManagementStrings.FcmTemplateRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class FcmTemplateRegistrationDescription : FcmRegistrationDescription
    {
        internal override string AppPlatForm
        {
            get
            {
                return FcmCredential.AppPlatformName;
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
                return FcmCredential.AppPlatformName + RegistrationDescription.TemplateRegistrationType;
            }
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmTemplateRegistrationDescription"/> class by copying fields from the given instance
        /// </summary>
        /// <param name="sourceRegistration">Another <see cref="T:Microsoft.Azure.NotificationHubs.FcmTemplateRegistrationDescription"/> instance fields values are copyed from</param>
        public FcmTemplateRegistrationDescription(FcmTemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.BodyTemplate = sourceRegistration.BodyTemplate;
            this.TemplateName = sourceRegistration.TemplateName;
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmTemplateRegistrationDescription"/> class using given Firebase Cloud Messaging registration id
        /// </summary>
        /// <param name="fcmRegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        public FcmTemplateRegistrationDescription(string fcmRegistrationId)
            : base(fcmRegistrationId)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmTemplateRegistrationDescription"/> class using given Firebase Cloud Messaging registration id and template for payload
        /// </summary>
        /// <param name="fcmRegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        /// <param name="jsonPayload">Payload template</param>
        public FcmTemplateRegistrationDescription(string fcmRegistrationId, string jsonPayload)
            : this(string.Empty, fcmRegistrationId, jsonPayload, null)
        {
        }

        /// <summary>
        /// Creates instance of <see cref="T:Microsoft.Azure.NotificationHubs.FcmTemplateRegistrationDescription"/> class using given Firebase Cloud Messaging registration id, template for payload and collection of tags
        /// </summary>
        /// <param name="fcmRegistrationId">Registration id obtained from the Firebase Cloud Messaging service</param>
        /// <param name="jsonPayload">Payload template</param>
        /// <param name="tags">Collection of tags. Tags can be used for audience targeting purposes.</param>
        public FcmTemplateRegistrationDescription(string fcmRegistrationId, string jsonPayload, IEnumerable<string> tags)
            : this(string.Empty, fcmRegistrationId, jsonPayload, tags)
        {
        }

        internal FcmTemplateRegistrationDescription(string notificationHubPath, string fcmRegistrationId, string jsonPayload, IEnumerable<string> tags)
            : base(notificationHubPath, fcmRegistrationId, tags)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.BodyTemplate = new CDataMember(jsonPayload);
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

        internal override void OnValidate(ApiVersion version)
        {
            base.OnValidate(version);

            try
            {
                using (XmlReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(this.BodyTemplate), new XmlDictionaryReaderQuotas()))
                {
                    XDocument payloadDocument = XDocument.Load(reader);

                    foreach (XElement element in payloadDocument.Root.DescendantsAndSelf())
                    {
                        foreach (XAttribute attribute in element.Attributes())
                        {
                            ExpressionEvaluator.Validate(attribute.Value, version);
                        }

                        if (!element.HasElements && !string.IsNullOrEmpty(element.Value))
                        {
                            ExpressionEvaluator.Validate(element.Value, version);
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

            this.ValidateTemplateName();
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

        internal override RegistrationDescription Clone()
        {
            return new FcmTemplateRegistrationDescription(this);
        }
    }
}
