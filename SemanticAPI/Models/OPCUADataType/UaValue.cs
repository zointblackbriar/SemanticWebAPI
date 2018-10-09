using Opc.Ua;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace SemanticAPI.Models.OPCUADataType
{
    public class UaValue
    {
        public readonly JToken Value;
        public readonly JSchema Schema;

        public StatusCode? StatusCode { get; set; }

        public UaValue()
        {
        }

        public UaValue(JToken value, JSchema schema)
        {
            Value = value;
            Schema = schema;
        }
    }

}
