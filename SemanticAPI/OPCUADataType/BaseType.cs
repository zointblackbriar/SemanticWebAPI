using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opc.Ua;

namespace SemanticAPI.OPCUADataType
{
    public class BaseType
    {
        public string PlatformNodeId { get; set; }
        public string DisplayName { get; set; }

        public NodeClass NodeClass { get; set; }

        public NodeId ReferenceTypeId { get; set; }

        public BaseType(string platformNodeIdParam, string DisplayNameParam, NodeClass NodeClassParam, NodeId ReferenceTypeIdParam)
        {
            this.PlatformNodeId = platformNodeIdParam;
            this.DisplayName = DisplayNameParam;
            this.NodeClass = NodeClassParam;
            this.ReferenceTypeId = ReferenceTypeIdParam;
        }
    }
}
