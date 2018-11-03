
//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the body part of Template registration descriptions. The body tag within a template XML description can be any string value
    /// and is treated as a CData section.
    /// </summary>
    [XmlSchemaProvider("GenerateSchema")]
    public sealed class CDataMember : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.CDataMember"/> class.
        /// </summary>
        public CDataMember()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Azure.NotificationHubs.CDataMember"/> class with the specified value.
        /// </summary>
        /// <param name="value">The specified value.</param>
        public CDataMember(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the member value.
        /// </summary>
        /// <value>
        /// The member value.
        /// </value>
        public string Value
        {
            get;
            set;
        }

        // implicit to/from string   

        /// <summary>
        /// Converts the specified value to a <see cref="T:Microsoft.Azure.NotificationHubs.CDataMember"/> object.
        /// </summary>
        /// 
        /// <returns>
        /// The value to converts the <see cref="T:Microsoft.Azure.NotificationHubs.CDataMember"/> object.
        /// </returns>
        /// <param name="value">The value to be converted.</param>
        public static implicit operator string(CDataMember value)
        {
            return value == null ? null : value.Value;
        }

        /// <summary>
        /// Converts the <see cref="T:Microsoft.Azure.NotificationHubs.CDataMember"/> object to a specified value.
        /// </summary>
        /// 
        /// <returns>
        /// The converted <see cref="T:Microsoft.Azure.NotificationHubs.CDataMember"/> object to a specified value.
        /// </returns>
        /// <param name="value">The value to be converted.</param>
        public static implicit operator CDataMember(string value)
        {
            return value == null ? null : new CDataMember(value);
        }

        /// <summary>
        /// Returns schema representation for the XML schemas.
        /// </summary>
        /// 
        /// <returns>
        /// The schema representation for the XML schemas.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates the schema representation for the XML schema conversion.
        /// </summary>
        /// 
        /// <returns>
        /// The schema representation for the XML schema conversion.
        /// </returns>
        /// <param name="xs">The XML schema.</param>
        // return "xs:string" as the type in scheme generation   
        public static XmlQualifiedName GenerateSchema(XmlSchemaSet xs)
        {
            return XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String).QualifiedName;
        }

        /// <summary>
        /// Writes XML data into its XML representation.
        /// </summary>
        /// <param name="writer">The XML writer to write.</param>
        // "" => <Node/>   // "Foo" => <Node><![CDATA[Foo]]></Node>   
        public void WriteXml(XmlWriter writer)
        {
      if (string.IsNullOrEmpty(this.Value))
        return;
      writer.WriteCData(this.Value);
        }

        /// <summary>
        /// Reads XML schema from its XML representation.
        /// </summary>
        /// <param name="reader">The XML reader to read the data.</param> 
        // <Node/> => ""   // <Node></Node> => ""   // <Node>Foo</Node> => "Foo"   // <Node><![CDATA[Foo]]></Node> => "Foo"   
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                Value = string.Empty;
            }
            else
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        Value = string.Empty;
                        break;
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        Value = reader.ReadContentAsString();
                        break;
                    default:
                        throw new SerializationException("Expected text/cdata");
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// 
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Value;
        }
    }
}