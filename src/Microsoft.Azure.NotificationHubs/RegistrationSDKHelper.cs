//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using Microsoft.Azure.NotificationHubs;

    internal static class RegistrationSDKHelper
    {
        internal const int TemplateMaxLength = 200;

        internal static void ValidateRegistration(RegistrationDescription registration)
        {
            var windowsTemplateRegistration = registration as WindowsTemplateRegistrationDescription;
            if (windowsTemplateRegistration != null)
            {
                windowsTemplateRegistration.SetWnsType();
            }
            else
            {
                var mpnsTemplateRegistration = registration as MpnsTemplateRegistrationDescription;
                if (mpnsTemplateRegistration != null)
                {
                    mpnsTemplateRegistration.SetMpnsType();
                }
            }

            // validate
            registration.Validate();
        }

        /// <summary>
        /// Find type from xml string, and it should set to WnsHeaders["X-WNS-Type"];
        /// If the header already there, this function won't overwrite.
        /// </summary>
        private static void SetMpnsType(this MpnsTemplateRegistrationDescription registration)
        {
            if (registration == null || registration.IsJsonObjectPayLoad())
            {
                return;
            }

            if (registration.MpnsHeaders != null && registration.MpnsHeaders.ContainsKey(MpnsRegistrationDescription.NotificationClass))
            {
                int notificationClass = Int32.Parse(registration.MpnsHeaders[MpnsRegistrationDescription.NotificationClass], CultureInfo.InvariantCulture);
                if ((notificationClass >= 3 && notificationClass <= 10) ||
                    (notificationClass >= 13 && notificationClass <= 20) ||
                    (notificationClass >= 23 && notificationClass <= 31))

                    // raw type
                    return;
            }

            if(registration.IsXmlPayLoad())
            {
                if (registration.MpnsHeaders == null)
                {
                    registration.MpnsHeaders = new MpnsHeaderCollection();
                }

                switch (DetectMpnsTemplateRegistationType(registration.BodyTemplate, SRClient.NotSupportedXMLFormatAsBodyTemplateForMpns))
                {
                    case MpnsTemplateBodyType.Tile:
                        AddOrUpdateHeader(registration.MpnsHeaders, MpnsRegistrationDescription.Type, MpnsRegistrationDescription.Tile);
                        AddOrUpdateHeader(registration.MpnsHeaders, MpnsRegistrationDescription.NotificationClass, MpnsRegistrationDescription.TileClass);
                        break;
                    case MpnsTemplateBodyType.Toast:
                        AddOrUpdateHeader(registration.MpnsHeaders, MpnsRegistrationDescription.Type, MpnsRegistrationDescription.Toast);
                        AddOrUpdateHeader(registration.MpnsHeaders, MpnsRegistrationDescription.NotificationClass, MpnsRegistrationDescription.ToastClass);
                        break;
                }
            }
        }

        /// <summary>
        /// Find type from xml string, and it should set to WnsHeaders["X-WNS-Type"];
        /// If the header already there, this function won't overwrite.
        /// </summary>
        private static void SetWnsType(this WindowsTemplateRegistrationDescription registration)
        {
            if (registration == null || registration.IsJsonObjectPayLoad())
            {
                return;
            }
            
            if (registration.IsXmlPayLoad())
            {
                if (registration.WnsHeaders == null)
                {
                    registration.WnsHeaders = new WnsHeaderCollection();
                }

                if (registration.WnsHeaders.ContainsKey(WindowsRegistrationDescription.Type) &&
                registration.WnsHeaders[WindowsRegistrationDescription.Type].Equals(WindowsRegistrationDescription.Raw, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        XmlDocument xmlPayload = new XmlDocument();
                        using (var reader = XmlTextReader.Create(new StringReader(registration.BodyTemplate)))
                        {
                            xmlPayload.Load(reader);
                        }
                    }
                    catch (XmlException)
                    {
                        throw new ArgumentException(SRClient.NotSupportedXMLFormatAsBodyTemplate);
                    }
                }
                else
                {
                    switch (DetectWindowsTemplateRegistationType(registration.BodyTemplate, SRClient.NotSupportedXMLFormatAsBodyTemplate))
                    {
                        case WindowsTemplateBodyType.Toast:
                            AddOrUpdateHeader(registration.WnsHeaders, WindowsRegistrationDescription.Type, WindowsRegistrationDescription.Toast);
                            break;
                        case WindowsTemplateBodyType.Tile:
                            AddOrUpdateHeader(registration.WnsHeaders, WindowsRegistrationDescription.Type, WindowsRegistrationDescription.Tile);
                            break;
                        case WindowsTemplateBodyType.Badge:
                            AddOrUpdateHeader(registration.WnsHeaders, WindowsRegistrationDescription.Type, WindowsRegistrationDescription.Badge);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void AddOrUpdateHeader(SortedDictionary<string, string> headers, string key, string value)
        {
            if (!headers.ContainsKey(key))
            {
                headers.Add(key, value);
            }
            else
            {
                headers[key] = value;
            }
        }

        public static WindowsTemplateBodyType DetectWindowsTemplateRegistationType(string body, string errorMsg)
        {
            XmlDocument xmlPayload = new XmlDocument();

            using (var reader = XmlTextReader.Create(new StringReader(body)))
            {
                try
                {
                    xmlPayload.Load(reader);
                }
                catch (XmlException)
                {
                    throw new ArgumentException(errorMsg);
                }

                XmlNode node = xmlPayload.FirstChild;
                while (node != null && node.NodeType != XmlNodeType.Element)
                {
                    node = node.NextSibling;
                }

                if (node == null)
                {
                    throw new ArgumentException(errorMsg);
                }

                WindowsTemplateBodyType registrationType;
                if (node == null || !Enum.TryParse(node.Name, true, out registrationType))
                {
                    throw new ArgumentException(errorMsg);
                }

                return registrationType;
            }
        }

        public static MpnsTemplateBodyType DetectMpnsTemplateRegistationType(string body, string errorMsg)
        {
            XmlDocument xmlPayload = new XmlDocument();
            using (var reader = XmlTextReader.Create(new StringReader(body)))
            {
                try
                {
                    xmlPayload.Load(reader);
                }
                catch (XmlException)
                {
                    throw new ArgumentException(errorMsg);
                }

                XmlNode node = xmlPayload.FirstChild;
                while (node != null && node.NodeType != XmlNodeType.Element)
                {
                    node = node.NextSibling;
                }

                if (node == null)
                {
                    throw new ArgumentException(errorMsg);
                }

                // notification node
                if (!node.NamespaceURI.Equals(MpnsRegistrationDescription.NamespaceName, StringComparison.OrdinalIgnoreCase) ||
                    !node.LocalName.Equals(MpnsRegistrationDescription.NotificationElementName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(errorMsg);
                }

                // type node
                XmlNode typeNode = node.FirstChild;
                MpnsTemplateBodyType registrationType;
                if (typeNode == null || !Enum.TryParse(typeNode.LocalName, true, out registrationType))
                {
                    throw new ArgumentException(errorMsg);
                }

                return registrationType;
            }
        }

        public static string AddDeclarationToXml(string content)
        {
            XmlDocument xmlPayload = new XmlDocument();

            using (var reader = XmlTextReader.Create(new StringReader(content)))
            {
                xmlPayload.Load(reader);
                if (xmlPayload.FirstChild.NodeType != XmlNodeType.XmlDeclaration)
                {
                    XmlNode declarationNode = xmlPayload.CreateXmlDeclaration("1.0", "utf-16", null);
                    XmlNode root = xmlPayload.DocumentElement;
                    xmlPayload.InsertBefore(declarationNode, root);
                }

                return xmlPayload.InnerXml;
            }
        }
    }

    internal enum WindowsTemplateBodyType
    {
        Toast,
        Tile,
        Badge,
        Raw
    }

    internal enum MpnsTemplateBodyType
    {
        Toast,
        Tile,   
        Raw
    }
}
