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

    internal class EntityDescriptionSerializer
    {
        const int MaxItemsInObjectGraph = 256;

        readonly Dictionary<string, DataContractSerializer> entirySerializers;

        public EntityDescriptionSerializer()
        {
            this.entirySerializers = new Dictionary<string, DataContractSerializer>();
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times",
            Justification = "Safe here. Any future behavior change is easy to detect")]
        public TMessagingDescription DeserializeFromAtomFeed<TMessagingDescription>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            XmlReaderSettings settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.None
            };

            MemoryStream streamCopy = new MemoryStream();
            stream.CopyTo(streamCopy);

            foreach (KeyValuePair<string, DataContractSerializer> item in this.entirySerializers)
            {
                streamCopy.Seek(0, SeekOrigin.Begin);
                using (XmlReader xmlReader = XmlReader.Create(streamCopy, settings))
                {
                    if (xmlReader.ReadToDescendant(item.Key))
                    {
                        return (TMessagingDescription)item.Value.ReadObject(xmlReader);
                    }
                }
            }

            throw new SerializationException();
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

            var serializer = GetSerializer(description.GetType().Name);
            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                serializer.WriteObject(xmlWriter, description);
            }

            return stringBuilder.ToString();
        }

        public void Serialize(EntityDescription description, XmlWriter writer)
        {
            var serializer = GetSerializer(description.GetType().Name);
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
