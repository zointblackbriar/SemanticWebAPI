using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Opc.Ua;

namespace SemanticAPI.OPCUADataType
{
    public class SchemaType
    {
        public readonly JToken value;

        public readonly JsonSchema schema;

        public StatusCode? StatusCode { get; set; }

        public SchemaType(JToken value, JsonSchema schema)
        {
            this.value = value;
            this.schema = schema;
        }
    }
}
