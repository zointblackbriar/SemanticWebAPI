﻿//OPC UA UniCT
//@Source : https://github.com/OPCUAUniCT

using System;
using Opc.Ua;
using Opc.Ua.Client;

namespace SemanticAPI.OPCUAModel
{
    public class DataTypeAnalyzer
    {
        private Session _session;

        public DataTypeAnalyzer(Session session)
        {
            this._session = session;
        }

        public static BuiltInType GetBuiltinTypeFromTypeName(string nameSpace, string type)
        {
            switch (nameSpace)
            {
                case "opc":
                    return GetBuiltinTypeFromBinaryTypeName(type);
                case "ua":
                    return GetBuiltinTypeFromUaTypeName(type);
                default:
                    throw new Exception("The structured value contains a field of an unknown TypeName.");
            }
        }

        private static BuiltInType GetBuiltinTypeFromUaTypeName(string type)
        {
            Type mType = Type.GetType("Opc.Ua." + type + ", Opc.Ua.Core");
            BuiltInType builtInType = TypeInfo.GetBuiltInType(TypeInfo.GetDataTypeId(mType));
            return builtInType;
        }

        private static BuiltInType GetBuiltinTypeFromBinaryTypeName(string type)
        {
            switch (type)
            {
                case "Bit":
                case "Boolean":
                    return BuiltInType.Boolean;
                case "SByte":
                    return BuiltInType.SByte;
                case "Byte":
                    return BuiltInType.Byte;
                case "Int16":
                    return BuiltInType.Int16;
                case "UInt16":
                    return BuiltInType.UInt16;
                case "Int32":
                    return BuiltInType.Int32;
                case "UInt32":
                    return BuiltInType.UInt32;
                case "Int64":
                    return BuiltInType.Int64;
                case "UInt64":
                    return BuiltInType.UInt64;
                case "Float":
                    return BuiltInType.Float;
                case "Double":
                    return BuiltInType.Double;
                case "Char":
                case "WideChar":
                case "String":
                case "CharArray":
                case "WideString":
                case "WideCharArray":
                    return BuiltInType.String;
                case "DateTime":
                    return BuiltInType.DateTime;
                case "ByteString":
                    return BuiltInType.ByteString;
                case "Guid":
                    return BuiltInType.Guid;
                default:
                    return BuiltInType.Null;
            }
        }

        //Node Findings with References
        internal NodeId GetDataTypeEncodingNodeId(NodeId dataTypeNodeId)
        {
            _session.Browse(
                null,
                null,
                dataTypeNodeId,
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HasEncoding,
                true,
                (uint)NodeClass.Object,
                out var continuationPoint,
                out var refDescriptionCollection);

            //Choose always first encoding
            return (NodeId)refDescriptionCollection[0].NodeId;
        }

        internal NodeId GetDataTypeDescriptionNodeId(NodeId dataTypeEncodingNodeId)
        {
            _session.Browse(
                null,
                null,
                dataTypeEncodingNodeId, //starting node is always an EncodingNode
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HasDescription,  //HasDescription reference
                true,
                (uint)NodeClass.Variable,
                out var continuationPoint,
                out var refDescriptionCollection);

            return (NodeId)refDescriptionCollection[0].NodeId;
        }

        internal string GetDictionary(NodeId dataTypeDescriptionNodeId)
        {
            _session.Browse(
                null,
                null,
                dataTypeDescriptionNodeId, //the starting node is a DataTypeDescription
                0u,
                BrowseDirection.Inverse, //It is an inverse Reference 
                ReferenceTypeIds.HasComponent, 
                true,
                (uint)NodeClass.Variable,
                out var continuationPoint,
                out var refDescriptionCollection);

            var dataTypeDictionaryNodeId = (NodeId)refDescriptionCollection[0].NodeId;

            var dataValueCollection = Read(dataTypeDictionaryNodeId, Attributes.Value);

            return System.Text.Encoding.UTF8.GetString((byte[])dataValueCollection[0].Value);
        }

        private DataValueCollection Read(NodeId nodeId, uint attributeId)
        {
            ReadValueIdCollection nodeToRead = new ReadValueIdCollection(1);

            ReadValueId vId = new ReadValueId()
            {
                NodeId = nodeId,
                AttributeId = attributeId
            };

            nodeToRead.Add(vId);

            var responseRead = _session.Read(null,
                0,
                TimestampsToReturn.Both,
                nodeToRead,
                out var dataValueCollection,
                out var diagnCollection
            );

            return dataValueCollection;
        }
    }
}
