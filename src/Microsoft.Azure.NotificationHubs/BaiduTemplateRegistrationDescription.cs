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
    /// Represents a Baidu template registration description.
    /// </summary>
    [DataContract(Name = ManagementStrings.BaiduTemplateRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class BaiduTemplateRegistrationDescription : BaiduRegistrationDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public BaiduTemplateRegistrationDescription(BaiduTemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.BodyTemplate = sourceRegistration.BodyTemplate;
            this.TemplateName = sourceRegistration.TemplateName;
            this.MessageType = sourceRegistration.MessageType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="baiduUserId">The baidu user identifier.</param>
        /// <param name="baiduChannelId">The baidu channel identifier.</param>
        public BaiduTemplateRegistrationDescription(string baiduUserId, string baiduChannelId)
            : base(baiduUserId, baiduChannelId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="baiduUserId">The baidu user identifier.</param>
        /// <param name="baiduChannelId">The baidu channel identifier.</param>
        /// <param name="jsonPayload">The json payload.</param>
        public BaiduTemplateRegistrationDescription(string baiduUserId, string baiduChannelId, string jsonPayload)
            : this(string.Empty, baiduUserId, baiduChannelId, jsonPayload, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="baiduUserId">The baidu user identifier.</param>
        /// <param name="baiduChannelId">The baidu channel identifier.</param>
        /// <param name="jsonPayload">The json payload.</param>
        /// <param name="tags">The tags.</param>
        public BaiduTemplateRegistrationDescription(string baiduUserId, string baiduChannelId, string jsonPayload, IEnumerable<string> tags)
            : this(string.Empty, baiduUserId, baiduChannelId, jsonPayload, tags, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="baiduUserId">The baidu user identifier.</param>
        /// <param name="baiduChannelId">The baidu channel identifier.</param>
        /// <param name="jsonPayload">The json payload.</param>
        /// <param name="messageType">Type of the message.</param>
        public BaiduTemplateRegistrationDescription(string baiduUserId, string baiduChannelId, string jsonPayload, int messageType)
            : this(string.Empty, baiduUserId, baiduChannelId, jsonPayload, null, messageType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="baiduUserId">The baidu user identifier.</param>
        /// <param name="baiduChannelId">The baidu channel identifier.</param>
        /// <param name="jsonPayload">The json payload.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="messageType">Type of the message.</param>
        public BaiduTemplateRegistrationDescription(string baiduUserId, string baiduChannelId, string jsonPayload, IEnumerable<string> tags, int messageType)
            : this(string.Empty, baiduUserId, baiduChannelId, jsonPayload, tags, messageType)
        {
        }
        
        internal BaiduTemplateRegistrationDescription(string notificationHubPath, string baiduUserId, string baiduChannelId, string jsonPayload, IEnumerable<string> tags, int? messageType)
            : base(notificationHubPath, baiduUserId, baiduChannelId, tags)
        {
            if (string.IsNullOrWhiteSpace(jsonPayload))
            {
                throw new ArgumentNullException("jsonPayload");
            }

            this.MessageType = messageType;
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

        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        [DataMember(Name = ManagementStrings.MessageType, IsRequired = false, Order = 3003, EmitDefaultValue = false)]
        public int? MessageType { get; set; }

        internal override string AppPlatForm
        {
            get
            {
                return BaiduCredential.AppPlatformName;
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
                return BaiduCredential.AppPlatformName + RegistrationDescription.TemplateRegistrationType;
            }
        }

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

        internal override RegistrationDescription Clone()
        {
            return new BaiduTemplateRegistrationDescription(this);
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
