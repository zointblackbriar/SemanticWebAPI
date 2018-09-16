using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Opc.Ua;

namespace SemanticAPI.OPCUADataType
{
    public class SchemaBaseType
    {
        public  JToken value;

        public  JSchema schema;

        public StatusCode? StatusCode { get; set; }

        public SchemaBaseType(JToken value, JSchema schema)
        {
            this.value = value;
            this.schema = schema;
        }

        public SchemaBaseType()
        {
            //Empty constructor
        }
    }
}
