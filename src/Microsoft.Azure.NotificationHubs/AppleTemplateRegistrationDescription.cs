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
    /// Represents the description of the Apple template registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.AppleTemplateRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class AppleTemplateRegistrationDescription : AppleRegistrationDescription
    {
        internal override string AppPlatForm
        {
            get
            {
                return ApnsCredential.AppPlatformName;
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
                return ApnsCredential.AppPlatformName + RegistrationDescription.TemplateRegistrationType;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source of registration.</param>
        public AppleTemplateRegistrationDescription(AppleTemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.BodyTemplate = sourceRegistration.BodyTemplate;
            this.Expiry = sourceRegistration.Expiry;
            this.TemplateName = sourceRegistration.TemplateName;
            this.Priority = sourceRegistration.Priority;

            this.ApnsHeaders = new ApnsHeaderCollection();
            if (sourceRegistration.ApnsHeaders != null)
            {
                foreach (var header in sourceRegistration.ApnsHeaders)
                {
                    this.ApnsHeaders.Add(header.Key, header.Value);
                }
            }
        }

        /// <summary>
        /// Initializes new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="deviceToken">The device token.</param>
        public AppleTemplateRegistrationDescription(string deviceToken)
            : base(deviceToken)
        {
            this.ApnsHeaders = new ApnsHeaderCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="deviceToken">The device token.</param><param name="jsonPayload">The JSON payload.</param>
        public AppleTemplateRegistrationDescription(string deviceToken, string jsonPayload)
            : this(string.Empty, deviceToken, jsonPayload, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="deviceToken">The device token.</param><param name="jsonPayload">The JSON payload.</param><param name="apnsHeaders">The APNS headers.</param>
        public AppleTemplateRegistrationDescription(string deviceToken, string jsonPayload, IDictionary<string, string> apnsHeaders)
            : this(string.Empty, deviceToken, jsonPayload, null, apnsHeaders)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.AppleTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="deviceToken">The device token.</param><param name="jsonPayload">The JSON payload.</param><param name="tags">The description tags.</param>
        public AppleTemplateRegistrationDescription(string deviceToken, string jsonPayload, IEnumerable<string> tags)
            : this(string.Empty, deviceToken, jsonPayload, tags, null)
        {
        }

        internal AppleTemplateRegistrationDescription(string notificationHubPath, string deviceToken, string jsonPayload, IEnumerable<string> tags, IDictionary<string, string> apnsHeaders)
            : base(notificationHubPath, deviceToken, tags)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.BodyTemplate = new CDataMember(jsonPayload);

            this.ApnsHeaders = new ApnsHeaderCollection();
            if (apnsHeaders != null)
            {
                foreach (var item in apnsHeaders)
                {
                    this.ApnsHeaders.Add(item.Key, item.Value);
                }
            }
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
        /// Gets or sets the expiry date.
        /// </summary>
        /// <value>
        /// The expiry date.
        /// </value>
        [DataMember(Name = ManagementStrings.Expiry, IsRequired = false, Order = 3002)]
        public string Expiry { get; set; }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>
        /// The name of the template.
        /// </value>
        [DataMember(Name = ManagementStrings.TemplateName, IsRequired = false, Order = 3003)]
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the notification priority.
        /// </summary>
        /// <value>
        /// The notification priority.
        /// </value>
        [DataMember(Name = ManagementStrings.Priority, IsRequired = false, Order = 3004, EmitDefaultValue = false)]
        public string Priority { get; set; }

        /// <summary>
        /// Gets or sets the APNS headers.
        /// </summary>
        /// <value>
        /// The APNS headers.
        /// </value>
        [DataMember(Name = ManagementStrings.ApnsHeaders, IsRequired = false, Order = 3005, EmitDefaultValue = false)]
        public ApnsHeaderCollection ApnsHeaders { get; set; }

        internal override void OnValidate(ApiVersion version)
        {
            base.OnValidate(version);

            if (this.Expiry != null)
            {
                if (this.Expiry == string.Empty)
                {
                    throw new InvalidDataContractException(SRClient.EmptyExpiryValue);
                }

                if (ExpressionEvaluator.Validate(this.Expiry, version) == ExpressionEvaluator.ExpressionType.Literal)
                {
                    DateTime returnVal;
                    if (!DateTime.TryParse(this.Expiry, out returnVal) && !string.Equals("0", this.Expiry, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidDataContractException(SRClient.ExpiryDeserializationError);
                    }
                }
            }

            if (this.Priority != null)
            {
                if (this.Priority == string.Empty)
                {
                    throw new InvalidDataContractException(SRClient.EmptyPriorityValue);
                }

                if (ExpressionEvaluator.Validate(this.Priority, version) == ExpressionEvaluator.ExpressionType.Literal)
                {
                    byte returnVal;
                    if (!byte.TryParse(this.Priority, out returnVal))
                    {
                        throw new InvalidDataContractException(SRClient.PriorityDeserializationError);
                    }
                }
            }

            if (this.ApnsHeaders != null)
            {
                if (this.ApnsHeaders.ContainsKey(AppleRegistrationDescription.ApnsPriorityHeader))
                {
                    if (ExpressionEvaluator.Validate(this.ApnsHeaders[AppleRegistrationDescription.ApnsPriorityHeader], version) == ExpressionEvaluator.ExpressionType.Literal)
                    {
                        byte returnVal;
                        if (!byte.TryParse(this.ApnsHeaders[AppleRegistrationDescription.ApnsPriorityHeader], out returnVal))
                        {
                            throw new InvalidDataContractException(SRClient.PriorityDeserializationError);
                        }
                    }
                }

                if (this.ApnsHeaders.ContainsKey(AppleRegistrationDescription.ApnsExpiryHeader))
                {
                    if (ExpressionEvaluator.Validate(this.ApnsHeaders[AppleRegistrationDescription.ApnsExpiryHeader], version) == ExpressionEvaluator.ExpressionType.Literal)
                    {
                        int returnVal;
                        if (!int.TryParse(this.ApnsHeaders[AppleRegistrationDescription.ApnsExpiryHeader], out returnVal))
                        {
                            throw new InvalidDataContractException(SRClient.ApnsExpiryHeaderDeserializationError);
                        }
                    }
                }

                foreach (var header in this.ApnsHeaders)
                {
                    if (!header.Key.StartsWith(AppleRegistrationDescription.ApnsHeaderPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidDataContractException(SRClient.ApnsHeaderDeserializationError(header.Key));
                    }

                    ExpressionEvaluator.Validate(header.Value, version);
                }
            }

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
            return new AppleTemplateRegistrationDescription(this);
        }
    }
}