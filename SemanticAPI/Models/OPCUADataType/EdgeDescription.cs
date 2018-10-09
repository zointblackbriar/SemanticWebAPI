using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticAPI.Models.OPCUADataType
{
    public class EdgeDescription
    {
        public string PlatformNodeId { get; set; }
        public string DisplayName { get; set; }
        public NodeClass NodeClass { get; set; }
        public NodeId ReferenceTypeId { get; set; }

        public EdgeDescription(string platformNodeId, string displayName, NodeClass nodeClass, NodeId referenceTypeId)
        {
            PlatformNodeId = platformNodeId;
            DisplayName = displayName;
            NodeClass = nodeClass;
            ReferenceTypeId = referenceTypeId;
        }
    }
}
