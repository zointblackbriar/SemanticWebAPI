using Opc.Ua;

namespace SemanticAPI.OPCUADataType
{
    public class BaseType
    {
        public string NodeId { get; set; }
        public string DisplayName { get; set; }

        public NodeClass NodeClass { get; set; }

        public NodeId ReferenceTypeId { get; set; }

        public BaseType(string NodeId, string DisplayNameParam, NodeClass NodeClassParam, NodeId ReferenceTypeIdParam)
        {
            this.NodeId = NodeId;
            this.DisplayName = DisplayNameParam;
            this.NodeClass = NodeClassParam;
            this.ReferenceTypeId = ReferenceTypeIdParam;
        }
    }
}
