using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Opc.Ua;
using SemanticAPI.OPCUADataType;

namespace SemanticAPI.OPCUAModel
{
    //This class will be used to parse Opc.Ua.Types.bsd
    //https://github.com/OPCFoundation/UA-.NET-Legacy/blob/master/Stack/Core/Schema/Opc.Ua.Types.bsd
    public class XMLParser
    {

        private  XPathNavigator _navigate;
        private  XmlNamespaceManager _namespaceMan;
        private  BinaryDecoder _binaryDecoder;
        public XMLParser(string sourceXML)
        {
            addNamespaceFromXml(sourceXML);
        }

        private void addNamespaceFromXml(string param_sourceXML)
        {
            using (TextReader textReader = new StringReader(param_sourceXML))
            {
                var path = new XPathDocument(textReader);
                _navigate = path.CreateNavigator();

                _navigate.MoveToFollowing(XPathNodeType.Element);

                IDictionary<string, string> namespaces = _navigate.GetNamespacesInScope(System.Xml.XmlNamespaceScope.All);
                _namespaceMan = new XmlNamespaceManager(_navigate.NameTable);

                foreach (KeyValuePair<string, string> item in namespaces)
                    _namespaceMan.AddNamespace(item.Key, item.Value);
            }
        }

        public SchemaBaseType Parse(string descriptionId, ExtensionObject extensionObject, ServiceMessageContext context, bool generateSchema)
        {
            //ExtensionObject may contain any scalar data type.Even those that are unknown to the
            //receiver
            //@Reference : https://open62541.org/doc/0.2/types.html#extensionobject
            _binaryDecoder = new BinaryDecoder((byte[])extensionObject.Body, context);

            return BuildJsonForObject(descriptionId, generateSchema);
        }

        private SchemaBaseType BuildJsonForObject(string param_descriptionId, bool param_generateSchema )
        {
            var complexObject = new JObject();
            var complexSchema = new JSchema { Type = JSchemaType.Object };
            int length = 0;
            if (complexSchema == null)
                return null;
            XPathNodeIterator iterator = _navigate.Select($"/opc:TypeDictionary/opc:StructuredType[@Name='{param_descriptionId}']", _namespaceMan);
            while(iterator.MoveNext())
            {
                var structuredBaseType = iterator.Current.GetAttribute("BaseType", "");

                if (structuredBaseType == "ua:Union") throw new Exception("Decoding process is not correct");

                XPathNodeIterator newIterator = iterator.Current.SelectDescendants(XPathNodeType.Element, false);

                while(newIterator.MoveNext())
                {
                    if(newIterator.Current.Name.Equals("opc:Field"))
                    {
                        string fieldName = newIterator.Current.GetAttribute("Name", "");
                        string type = newIterator.Current.GetAttribute("TypeName", "");
                        string lengthSource = newIterator.Current.GetAttribute("LengthField", "");

                        if (string.IsNullOrEmpty(lengthSource)) length = 1;
                        int.Parse((string)complexObject[lengthSource]);

                        if(!type.Contains("opc:") || type.Contains("ua:"))
                        {
                            var uaValue = BuildInnerComplex(type.Split(':')[1], length, param_generateSchema);
                            complexObject[fieldName] = uaValue.value;
                        }


                    }
                }
            }


            return new SchemaBaseType(complexObject, complexSchema);
        }

        private SchemaBaseType BuildInnerComplex(string description, int length, bool generateSchema)
        {
            if (length == 1) BuildJsonForObject(description, generateSchema);

            var jsonArray = new JArray();
            SchemaBaseType schemaVal = new SchemaBaseType();

            for(int counter = 0; counter < length; counter++)
            {
                schemaVal = BuildJsonForObject(description, generateSchema);
                jsonArray.Insert(counter, schemaVal.value);
            }

            var jsonSchema = (generateSchema) ? DataTypeSchemaGenerator.GenerateSchemaForArray(new[] { length }, schemaVal.schema) : null;
            return new SchemaBaseType(jsonArray, jsonSchema);

        }

        private Object ReadBuiltinValue (BuiltInType builtInType)
        {
            var methodToCall = "Read" + builtInType;

            MethodInfo mInfo = typeof(BinaryDecoder).GetMethod(methodToCall, new[] { typeof(string) });
            if(builtInType == BuiltInType.ByteString)
            {
                byte[] byteString = mInfo.Invoke(_binaryDecoder, new object[] { "" }) as byte[];
                var base64ByteString = Convert.ToBase64String(byteString);
                return base64ByteString;
            }
            if(builtInType == BuiltInType.Guid)
            {
                string guid = mInfo.Invoke(_binaryDecoder, new object[] { " " }).ToString();
                return guid;
            }
            if(builtInType == BuiltInType.ExtensionObject)
            {
                ExtensionObject extObject = mInfo.Invoke(_binaryDecoder, new object[] { "" }) as ExtensionObject;
                return extObject.Body;
            }

            return mInfo.Invoke(_binaryDecoder, new object[] { "" });
        }
    }
}
