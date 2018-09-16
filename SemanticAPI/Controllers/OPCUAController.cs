using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SemanticAPI.Options;
using SemanticAPI.OPCUASemantic;
using System;
using System.Threading.Tasks;
using Opc.Ua;

namespace SemanticAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class OPCUAController : Controller
    {

        //Create GetNode
        //TODO:  read nodes, values, schemas, status, Deadband, minimumSamplingInterval

        //Read Async Node
        //TODO: Async reading node information 

        //Monitoring the nodes
        //TODO: 

        private readonly ServerIdentities[] _uaServers;

        private readonly OPCUAClient _uaClient;

        public OPCUAController(IOptions<ServerOptions> servers)
        {
            this._uaServers = servers.Value.Servers;
        }

        //Get method for all server option parameters
        [HttpGet("data-sets")]
        public IActionResult getDataSets()
        {
            return Ok(_uaServers);
        }

        //Rest request for node_id
        [HttpGet("data-sets/{ds_id:int}/nodes/{node_id:regex(^\\d+-(?:(\\d+)|(.+))$)?)}")]
        public async Task<IActionResult> GetNode(int serverDataSetId, string node_id = "0-85")
        {
            if (serverDataSetId < 0 || serverDataSetId > _uaServers.Length)
                return NotFound("No Dataset is specified");

            var serverUrl = _uaServers[serverDataSetId].Url;
            //Check if the server available

            //Url will be decoded and used
            var decodedNodeId = System.Net.WebUtility.UrlDecode(node_id);

            var result = new Newtonsoft.Json.Linq.JObject();

            try
            {
                var sourceNode = await _uaClient.ReadNodeAsync(serverUrl, decodedNodeId);
                result["node-id"] = decodedNodeId;
                result["name"] = sourceNode.DisplayName.Text;
                switch(sourceNode.NodeClass)
                {
                    //Implementation Variables in switch case statements
                    case NodeClass.Variable:
                        result["type"] = "variable";
                        var variableNode = (VariableNode)sourceNode;
                        var opcUAValue = await _uaClient.ReadUaValueAsync(serverUrl, variableNode);

                        break;
                    case NodeClass.Method:
                        result["type"] = "method";
                        break;
                    case NodeClass.Object:
                        //TODO Implement a function is it folder or async
                        result["type"] = 1 == 2 ? "folder" : "object";
                        break;

                }
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
