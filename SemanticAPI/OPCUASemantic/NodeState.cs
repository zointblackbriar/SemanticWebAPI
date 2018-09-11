using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticAPI.OPCUASemantic
{
    public class NodeState
    {
        public JToken Value { get; set; }

        public bool isValid => Value != null;
    }
}
