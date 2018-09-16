using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticAPI.OPCUASemantic
{
    public class TypeControl
    {
        private readonly Session session;

        public TypeControl(Session session)
        {
            this.session = session;
        }

        public DataValue DataFetcherFromNodeState(VariableNode variableState, NodeState nodeState)
        {
            //This function will be implemented
            return null;
        }

    }
}
