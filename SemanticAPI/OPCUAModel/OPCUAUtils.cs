using Newtonsoft.Json.Linq;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SemanticAPI.OPCUAModel
{
    public static class OPCUAUtils
    {
        public static string ToStringId(this ExpandedNodeId expandedNodeId, NamespaceTable namespaceTable)
        {
            var nodeId = ExpandedNodeId.ToNodeId(expandedNodeId, namespaceTable);
            return $"{nodeId.NamespaceIndex}--{nodeId.Identifier}";
        }
    }

    public static class CollectionMethods
    {
        public static void Add(this IList<JToken> list, IList<JToken> toAdd)
        {
            foreach(var a in toAdd)
            {
                list.Add(a);
            }
        }
    }

    public static class JTokenClass
    {
        public static int[] jsonArrayDim(this JToken jToken)
        {
            if (!isNullOrEmptyJToken(jToken))
                throw new Exception("jToken type contains null value");

            if(jToken.Type != JTokenType.Array)
            {
                Console.WriteLine("JToken Type is not suitable"); 
            }

            while(jToken.HasValues)
            {
                var children = jToken.Children();
                var count = children.First().Count();

                foreach(var child in children)
                {
                    if(child.Count() != count)
                    {
                        throw new Exception("The array need to be same number of elements in each dimensions");
                    }
                }
                jToken = jToken.Last;

            }

            const string pattern = @"\[(\d+)\]";
            var regex = new Regex(pattern);
            var matchAll = regex.Matches(jToken.Path);
            var dimensions = new int[matchAll.Count];
            for(var i = 0; i < matchAll.Count; i++)
            {
                dimensions[i] = int.Parse(matchAll[i].Groups[1].Value) + 1;
            }
            return dimensions;
            
        }

        public static JArray convertOneDimensionJArray(this JToken token)
        {
            var dimensions = token.jsonArrayDim();
            return token.combinedDimensionAndArray(dimensions);
        }

        public static JArray combinedDimensionAndArray(this JToken token, int[] dimensions)
        {
            var valuesToWrite = token.Children().ToArray();
            for (var i = 0; i < dimensions.Length - 1; i++)
                valuesToWrite = valuesToWrite.SelectMany(a => a).ToArray();

            return new JArray(valuesToWrite);
        }

        public static int calculateDimension(JToken token)
        {
            if (token.Type != JTokenType.Array || !isNullOrEmptyJToken(token))
                return -1;

            var jArray = token.ToArray();
            int numDimension = 1;

            while(jArray.GetElementsType() == JTokenType.Array)
            {
                jArray = jArray.Children().ToArray();
                numDimension++;
            }

            return numDimension;
        }

        public static JTokenType GetElementsType(this JToken[] tokens)
        {
            if (!tokens.ElementTypeCheck())
                throw new Exception("Element type is not suitable");
            return tokens.First().Type;
        }

        private static bool ElementTypeCheck(this JToken[] tokens)
        {
            var checkType = tokens[0].Type ==
                JTokenType.Integer ? JTokenType.Float : tokens[0].Type;
            return tokens
                .Select(x => (x.Type == JTokenType.Integer) ? JTokenType.Float : x.Type)
                .All(t => t == checkType);
        }

        private static bool isNullOrEmptyJToken(this JToken token)
        {
            return (token == null) ||
                (token.Type == JTokenType.Array && !token.HasValues) ||
                (token.Type == JTokenType.Object && !token.HasValues) ||
                (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                (token.Type == JTokenType.Null);
        }
    }

    public static class BuiltInDataTypeClass
    {
        public static Func<Variant> GetDecodeDelegate(this BuiltInType builtIn, OPCUAJsonDecoder decoder)
        {
            switch(builtIn)
            {
                case BuiltInType.Boolean:
                    return () => new Variant(decoder.ReadBoolean("Value"));
                case BuiltInType.SByte:
                    return () => new Variant(decoder.ReadBoolean("Value"));
                case BuiltInType.Byte:
                    return () => new Variant(decoder.ReadSByte("Value"));
                case BuiltInType.Int16:
                    return () => new Variant(decoder.ReadUInt16("Value"));
                case BuiltInType.UInt16:
                    return () => new Variant(decoder.ReadInt32("Value"));
                case BuiltInType.Int32:
                    return () => new Variant(decoder.ReadInt32("Value"));
                case BuiltInType.UInt32:
                    return () => new Variant(decoder.ReadUInt32("Value"));
                case BuiltInType.Int64:
                    return () => new Variant(decoder.ReadInt64("Value"));
                case BuiltInType.UInt64:
                    return () => new Variant(decoder.ReadFloat("Value"));
            }
        }
    }
}
