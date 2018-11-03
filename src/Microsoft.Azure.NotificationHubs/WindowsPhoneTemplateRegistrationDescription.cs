//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Azure.NotificationHubs.Messaging;

    /// <summary>
    /// Represents the description of the MPNS template registration.
    /// </summary>
    [DataContract(Name = ManagementStrings.MpnsTemplateRegistrationDescription, Namespace = ManagementStrings.Namespace)]
    public class MpnsTemplateRegistrationDescription : MpnsRegistrationDescription
    {
        internal override string AppPlatForm
        {
            get { return MpnsCredential.AppPlatformName; }
        }

        internal override string RegistrationType
        {
            get { return RegistrationDescription.TemplateRegistrationType; }
        }

        internal override string PlatformType
        {
            get
            {
                return MpnsCredential.AppPlatformName + RegistrationDescription.TemplateRegistrationType;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="sourceRegistration">The source registration.</param>
        public MpnsTemplateRegistrationDescription(MpnsTemplateRegistrationDescription sourceRegistration)
            : base(sourceRegistration)
        {
            this.MpnsHeaders = new MpnsHeaderCollection();
            if (sourceRegistration.MpnsHeaders != null)
            {
                foreach (var header in sourceRegistration.MpnsHeaders)
                {
                    this.MpnsHeaders.Add(header.Key, header.Value);
                }
            }

            this.BodyTemplate = sourceRegistration.BodyTemplate;
            this.TemplateName = sourceRegistration.TemplateName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        public MpnsTemplateRegistrationDescription(Uri channelUri)
            : base(channelUri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="templatePayload">The template payload.</param>
        public MpnsTemplateRegistrationDescription(
            Uri channelUri,
            string templatePayload)
            : this(string.Empty, channelUri, templatePayload, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        public MpnsTemplateRegistrationDescription(
            Uri channelUri,
            XmlDocument xmlTemplate)
            : this(string.Empty, channelUri, xmlTemplate.InnerXml, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="templatePayload">The template payload.</param>
        /// <param name="mpnsHeaders">The MPNS headers.</param>
        public MpnsTemplateRegistrationDescription(
            Uri channelUri,
            string templatePayload,
            IDictionary<string, string> mpnsHeaders)
            : this(string.Empty, channelUri, templatePayload, mpnsHeaders, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="templatePayload">The template payload.</param>
        /// <param name="tags">The tags collection.</param>
        public MpnsTemplateRegistrationDescription(
           Uri channelUri,
           string templatePayload,
           IEnumerable<string> tags)
            : this(string.Empty, channelUri, templatePayload, null, tags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="templatePayload">The template payload.</param>
        /// <param name="mpnsHeaders">The MPNS headers.</param>
        /// <param name="tags">The tags collection.</param>
        public MpnsTemplateRegistrationDescription(
            Uri channelUri,
            string templatePayload,
            IDictionary<string, string> mpnsHeaders,
            IEnumerable<string> tags)
            : this(string.Empty, channelUri, templatePayload, mpnsHeaders, tags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        public MpnsTemplateRegistrationDescription(string channelUri)
            : base(new Uri(channelUri))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="templatePayload">The template payload.</param>
        public MpnsTemplateRegistrationDescription(
            string channelUri,
            string templatePayload)
            : this(string.Empty, new Uri(channelUri), templatePayload, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="xmlTemplate">The XML template.</param>
        public MpnsTemplateRegistrationDescription(
            string channelUri,
            XmlDocument xmlTemplate)
            : this(string.Empty, new Uri(channelUri), xmlTemplate.InnerXml, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="templatePayload">The template payload.</param>
        /// <param name="mpnsHeaders">The MPNS headers.</param>
        public MpnsTemplateRegistrationDescription(
            string channelUri,
            string templatePayload,
            IDictionary<string, string> mpnsHeaders)
            : this(string.Empty, new Uri(channelUri), templatePayload, mpnsHeaders, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="templatePayload">The template payload.</param>
        /// <param name="tags">The tags collection.</param>
        public MpnsTemplateRegistrationDescription(
           string channelUri,
           string templatePayload,
           IEnumerable<string> tags)
            : this(string.Empty, new Uri(channelUri), templatePayload, null, tags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MpnsTemplateRegistrationDescription"/> class.
        /// </summary>
        /// <param name="channelUri">The channel URI.</param>
        /// <param name="templatePayload">The template payload.</param>
        /// <param name="mpnsHeaders">The MPNS headers.</param>
        /// <param name="tags">The tags collection.</param>
        public MpnsTemplateRegistrationDescription(
            string channelUri,
            string templatePayload,
            IDictionary<string, string> mpnsHeaders,
            IEnumerable<string> tags)
            : this(string.Empty, new Uri(channelUri), templatePayload, mpnsHeaders, tags)
        {
        }

        internal MpnsTemplateRegistrationDescription(
            string notificationHubPath,
            string channelUri,
            string templatePayload,
            IDictionary<string, string> mpnsHeaders,
            IEnumerable<string> tags)
            : this(notificationHubPath, new Uri(channelUri), templatePayload, mpnsHeaders, tags)
        {
        }

        internal MpnsTemplateRegistrationDescription(
            string notificationHubPath,
            Uri channelUri,
            string templatePayload,
            IDictionary<string, string> mpnsHeaders,
            IEnumerable<string> tags)
            : base(notificationHubPath, channelUri, tags)
        {
            if (string.IsNullOrWhiteSpace(templatePayload))
            {
                throw new ArgumentNullException("templatePayload");
            }

            BodyTemplate = new CDataMember(templatePayload);

            this.MpnsHeaders = new MpnsHeaderCollection();
            if (mpnsHeaders != null)
            {
                foreach (var item in mpnsHeaders)
                {
                    this.MpnsHeaders.Add(item.Key, item.Value);
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
        /// Gets or sets the MPNS headers.
        /// </summary>
        /// <value>
        /// The MPNS headers.
        /// </value>
        [DataMember(Name = ManagementStrings.MpnsHeaders, IsRequired = true, Order = 3002)]
        public MpnsHeaderCollection MpnsHeaders { get; set; }

        internal List<int> ExpressionStartIndices { get; set; }

        internal List<int> ExpressionLengths { get; set; }

        internal List<string> Expressions { get; set; }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>
        /// The name of the template.
        /// </value>
        [DataMember(Name = ManagementStrings.TemplateName, IsRequired = false, Order = 3003)]
        public string TemplateName { get; set; }

        internal override void OnValidate(bool allowLocalMockPns, ApiVersion version)
        {
            base.OnValidate(allowLocalMockPns, version);
            this.ValidateMpnsHeaders(version);
            if (this.IsXmlPayLoad())
            {
                this.ValidateXmlPayLoad(version);
            }
            else if (this.IsJsonObjectPayLoad())
            {
                this.ValidateJsonPayLoad(version);
            }
            else
            {
                throw new InvalidDataContractException(SRClient.InvalidPayLoadFormat);
            }

            this.ValidateTemplateName();
        }

        internal override RegistrationDescription Clone()
        {
            return new MpnsTemplateRegistrationDescription(this);
        }

        internal bool IsXmlPayLoad()
        {
            string payload = this.BodyTemplate.Value.Trim();
            return payload.StartsWith("<", StringComparison.OrdinalIgnoreCase);
        }

        internal bool IsJsonObjectPayLoad()
        {
            string payload = this.BodyTemplate.Value.Trim();
            return payload.StartsWith("{", StringComparison.OrdinalIgnoreCase) &&
                   payload.EndsWith("}", StringComparison.OrdinalIgnoreCase);
        }

        void ValidateTemplateName()
        {
            if (this.TemplateName != null)
            {
                if (this.TemplateName.Length > RegistrationSDKHelper.TemplateMaxLength)
                {
                    throw new InvalidDataContractException(SRClient.TemplateNameLengthExceedsLimit(RegistrationSDKHelper.TemplateMaxLength));
                }
            }
        }

        void ValidateMpnsHeaders(ApiVersion version)
        {
            if (this.MpnsHeaders == null ||
                !this.MpnsHeaders.ContainsKey(MpnsTemplateRegistrationDescription.NotificationClass) ||
                string.IsNullOrWhiteSpace(this.MpnsHeaders[MpnsTemplateRegistrationDescription.NotificationClass]))
            {
                throw new InvalidDataContractException(
                    SRClient.MissingMpnsHeader(MpnsTemplateRegistrationDescription.NotificationClass));
            }

            // MPNS headers validation
            foreach (string header in this.MpnsHeaders.Keys)
            {
                if (string.IsNullOrWhiteSpace(this.MpnsHeaders[header]))
                {
                    throw new InvalidDataContractException(SRClient.MpnsHeaderIsNullOrEmpty(header));
                }

                ExpressionEvaluator.Validate(this.MpnsHeaders[header], version);
            }
        }

        void ValidateXmlPayLoad(ApiVersion version)
        {
            XDocument payloadDocument = XDocument.Parse(this.BodyTemplate);
            this.ExpressionStartIndices = new List<int>();
            this.ExpressionLengths = new List<int>();
            this.Expressions = new List<string>();
            IDictionary<string, int> expressionToIndexMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (XElement element in payloadDocument.Root.DescendantsAndSelf())
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    if (ExpressionEvaluator.Validate(attribute.Value, version) != ExpressionEvaluator.ExpressionType.Literal)
                    {
                        // Extracts escaped expression.
                        // Example: id="$(id&gt;)" --> $(id&gt;)
                        string rawAttribute = attribute.ToString();
                        string rawAttributeValue = rawAttribute.Substring(rawAttribute.IndexOf('=') + 1);
                        string escapedExpression = rawAttributeValue.Substring(1, rawAttributeValue.Length - 2);

                        this.AddExpression(attribute.Value, escapedExpression, expressionToIndexMap);
                    }
                }

                if (!element.HasElements && !string.IsNullOrEmpty(element.Value))
                {
                    if (ExpressionEvaluator.Validate(element.Value, version) != ExpressionEvaluator.ExpressionType.Literal)
                    {
                        // Extracts escaped expression.
                        // Example: <text id="1">$(na&gt;me)</text> --> $(na&gt;me)
                        using (XmlReader reader = element.CreateReader())
                        {
                            reader.MoveToContent();
                            string escapedExpression = reader.ReadInnerXml();
                            this.AddExpression(element.Value, escapedExpression, expressionToIndexMap);
                        }
                    }
                }
            }
        }

        void ValidateJsonPayLoad(ApiVersion version)
        {
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
        }

        void AddExpression(string expression, string escapedExpression, IDictionary<string, int> expressionToIndexMap)
        {
            int previousIndex;
            if (!expressionToIndexMap.TryGetValue(expression, out previousIndex))
            {
                previousIndex = -1;
            }

            int newIndex = this.BodyTemplate.Value.IndexOf(escapedExpression, previousIndex + 1, StringComparison.OrdinalIgnoreCase);
            if (newIndex == -1)
            {
                throw new InvalidDataContractException(string.Format(CultureInfo.InvariantCulture, "Unsupported expression: {0}", expression));
            }

            this.ExpressionStartIndices.Add(newIndex);
            this.ExpressionLengths.Add(escapedExpression.Length);
            this.Expressions.Add(expression);
            expressionToIndexMap[expression] = newIndex;
        }
    }
}