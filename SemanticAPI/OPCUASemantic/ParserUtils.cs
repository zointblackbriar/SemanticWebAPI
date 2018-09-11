using System;
using System.Text.RegularExpressions;
using Opc.Ua;

namespace SemanticAPI.OPCUASemantic
{
    public class ParserUtils
    {
        public static NodeId ParserUtilsNodeIdString(string str_param)
        {
            NodeId nodeId = null;
            const string patternString = @"^(\d+)-(?:(\d+)|(\S+))$";
            var match = Regex.Match(str_param, patternString);
            var isString = match.Groups[3].Length != 0;
            var isNumeric = match.Groups[2].Length != 0;

            var idString = (isString) ? $"s = {match.Groups[3]}" : $"i={match.Groups[2]}";
            var builtString = $"ns={match.Groups[1]};" + idString;

            //Create a NodeId and test it
            try
            {
                nodeId = new NodeId(builtString);
            }catch(Exception e)
            {
                Console.WriteLine("NodeId creation error: " + e.Message);
            }
            return nodeId;
        }
    }
}
