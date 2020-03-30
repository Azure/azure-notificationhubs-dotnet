﻿//------------------------------------------------------------
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

    internal class EntityDescriptionSerializer
    {
        const int MaxItemsInObjectGraph = 256;

        readonly Dictionary<string, DataContractSerializer> entirySerializers;

        public EntityDescriptionSerializer()
        {
            this.entirySerializers = new Dictionary<string, DataContractSerializer>();
            this.entirySerializers.Add(
                typeof(RegistrationDescription).Name,
                this.CreateSerializer<RegistrationDescription>());

            this.entirySerializers.Add(
                typeof(WindowsRegistrationDescription).Name,
                this.CreateSerializer<WindowsRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(WindowsTemplateRegistrationDescription).Name,
                this.CreateSerializer<WindowsTemplateRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(AppleRegistrationDescription).Name,
                this.CreateSerializer<AppleRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(AppleTemplateRegistrationDescription).Name,
                this.CreateSerializer<AppleTemplateRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(GcmRegistrationDescription).Name,
                this.CreateSerializer<GcmRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(FcmRegistrationDescription).Name,
                this.CreateSerializer<FcmRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(GcmTemplateRegistrationDescription).Name,
                this.CreateSerializer<GcmTemplateRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(FcmTemplateRegistrationDescription).Name,
                this.CreateSerializer<FcmTemplateRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(MpnsRegistrationDescription).Name,
                this.CreateSerializer<MpnsRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(MpnsTemplateRegistrationDescription).Name,
                this.CreateSerializer<MpnsTemplateRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(AdmRegistrationDescription).Name,
                this.CreateSerializer<AdmRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(AdmTemplateRegistrationDescription).Name,
                this.CreateSerializer<AdmTemplateRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(BaiduRegistrationDescription).Name,
                this.CreateSerializer<BaiduRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(BaiduTemplateRegistrationDescription).Name,
                this.CreateSerializer<BaiduTemplateRegistrationDescription>());

            this.entirySerializers.Add(
                typeof(NotificationHubJob).Name,
                this.CreateSerializer<NotificationHubJob>());
        }

        private DataContractSerializer CreateSerializer<T>()
        {
            return new DataContractSerializer(typeof(T), new DataContractSerializerSettings
            {
                MaxItemsInObjectGraph = MaxItemsInObjectGraph
            });
        }

        public EntityDescription Deserialize(XmlReader reader, string typeName)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            var serializer = GetSerializer(typeName);

            return (EntityDescription)serializer.ReadObject(reader);
        }

        public string Serialize(EntityDescription description)
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

        public void Serialize(EntityDescription description, XmlWriter writer)
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
                serializer = GetSerializer(typeof(RegistrationDescription).Name);
            }
            else
            {
                serializer = GetSerializer(description.GetType().Name);
            }
            
            serializer.WriteObject(writer, description);
        }

        private DataContractSerializer GetSerializer(string typeName)
        {
            if (this.entirySerializers.TryGetValue(typeName, out var serializer))
            {
                return serializer;
            }
            else
            {
                throw new InvalidOperationException($"Unknown entity type {typeName}");
            }
        }
    }
}
