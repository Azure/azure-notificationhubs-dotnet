//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Collections.Generic;
    using Microsoft.Azure.NotificationHubs;

    internal static class EntityDescriptionSerializer
    {
        const int MaxItemsInObjectGraph = 256;

        private static DataContractSerializer CreateSerializer<T>()
        {
            return new DataContractSerializer(typeof(T), new DataContractSerializerSettings
            {
                MaxItemsInObjectGraph = MaxItemsInObjectGraph
            });
        }

        private static DataContractSerializer CreateSerializer(string typeName)
        {
            switch(typeName){
                case "RegistrationDescription":
                    return CreateSerializer<RegistrationDescription>();

                case "WindowsRegistrationDescription":
                    return CreateSerializer<WindowsRegistrationDescription>();

                case "WindowsTemplateRegistrationDescription":
                    return CreateSerializer<WindowsTemplateRegistrationDescription>();

                case "AppleRegistrationDescription":
                    return CreateSerializer<AppleRegistrationDescription>();

                case "AppleTemplateRegistrationDescription":
                    return CreateSerializer<AppleTemplateRegistrationDescription>();

                case "GcmRegistrationDescription":
                    return CreateSerializer<GcmRegistrationDescription>();

                case "FcmRegistrationDescription":
                    return CreateSerializer<FcmRegistrationDescription>();

                case "GcmTemplateRegistrationDescription":
                    return CreateSerializer<GcmTemplateRegistrationDescription>();

                case "FcmTemplateRegistrationDescription":
                    return CreateSerializer<FcmTemplateRegistrationDescription>();

                case "MpnsRegistrationDescription":
                    return CreateSerializer<MpnsRegistrationDescription>();

                case "MpnsTemplateRegistrationDescription":
                    return CreateSerializer<MpnsTemplateRegistrationDescription>();

                case "AdmRegistrationDescription":
                    return CreateSerializer<AdmRegistrationDescription>();

                case "AdmTemplateRegistrationDescription":
                    return CreateSerializer<AdmTemplateRegistrationDescription>();

                case "BaiduRegistrationDescription":
                    return CreateSerializer<BaiduRegistrationDescription>();

                case "BaiduTemplateRegistrationDescription":
                    return CreateSerializer<BaiduTemplateRegistrationDescription>();

                case "NotificationHubJob":
                    return CreateSerializer<NotificationHubJob>();

                default:
                    throw new InvalidOperationException($"Unknown entity type {typeName}");
            }
        }

        public static EntityDescription Deserialize<T>(XmlReader reader, string typeName) where T : EntityDescription
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var serializer = CreateSerializer(typeName);

            return (EntityDescription)serializer.ReadObject(reader);
        }

        public static string Serialize(EntityDescription description)
        {
            var stringBuilder = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                Serialize(description, xmlWriter);
            }

            return stringBuilder.ToString();
        }

        public static void Serialize(EntityDescription description, XmlWriter writer)
        {
            // Convert FCM descriptions into their GCM counterparts
            if (description.GetType().Name == "FcmRegistrationDescription")
            {
                description = new GcmRegistrationDescription((FcmRegistrationDescription) description);
            }

            if (description.GetType().Name == "FcmTemplateRegistrationDescription")
            {
                description = new GcmTemplateRegistrationDescription((FcmTemplateRegistrationDescription) description);
            }

            DataContractSerializer serializer;
            if (description is RegistrationDescription)
            {
                serializer = CreateSerializer("RegistrationDescription");
            }
            else
            {
                serializer = CreateSerializer(description.GetType().Name);
            }
            
            serializer.WriteObject(writer, description);
        }
    }
}
