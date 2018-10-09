//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using SemanticAPI.Options;
//using SemanticAPI.OPCUASemantic;
//using System;
//using System.Threading.Tasks;
//using Opc.Ua;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opc.Ua;
using System;
using Newtonsoft.Json.Linq;
using SemanticAPI.Models.DataSet;
using SemanticAPI.Models.OPCUADataType;
using SemanticAPI.Models.AuthCredentials;
using SemanticAPI.Models.Options;

using System.Threading.Tasks;
using System.Net;
using SemanticAPI.OPCUAModel;
using Microsoft.Extensions.Options;

namespace SemanticAPI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private readonly ServerIdentities[] _uaServers;

        private readonly IOPCUAClientSingleton _uaClient;

        public ApiController(IOptions<ServerOptions> servers, IOPCUAClientSingleton OPCUAClient)
        {
            this._uaServers = servers.Value.Servers;
            for (int i = 0; i < _uaServers.Length; i++) _uaServers[i].Id = i;

            this._uaClient = OPCUAClient;
        }

        //Get method for all server option parameters
        [HttpGet("data-sets")]
        public IActionResult getDataSets()
        {
            return Ok(_uaServers);
        }


        //Rest request for node_id
        [HttpGet("data-sets/{DataSetID:int}/nodes/{node_id:regex(^\\d+-(?:(\\d+)|(.+))$)?}")]
        public async Task<IActionResult> GetNode(int DataSetID, string node_id = "0-85")
        {

            if (DataSetID < 0 || DataSetID >= _uaServers.Length) return NotFound($"There is no Data Set for id {DataSetID}");

            var serverUrl = _uaServers[DataSetID].Url;
            if (!(await _uaClient.IsServerAvailable(serverUrl)))
                return StatusCode(500, "Data Set " + DataSetID + " NotAvailable");

            var decodedNodeId = WebUtility.UrlDecode(node_id);
            System.Diagnostics.Debug.WriteLine("decoded: " + decodedNodeId);

            var result = new JObject();

            try
            {
                var sourceNode = await _uaClient.ReadNodeAsync(serverUrl, decodedNodeId);
                result["node-id"] = decodedNodeId;
                result["name"] = sourceNode.DisplayName.Text;
                System.Diagnostics.Debug.WriteLine("sourceNode DecodedIT [TEST]" + result["node-id"]);

                switch (sourceNode.NodeClass)
                {
                    case NodeClass.Method:
                        result["type"] = "method";
                        break;
                    case NodeClass.Variable:
                        result["type"] = "variable";
                        var varNode = (VariableNode)sourceNode;
                        var uaValue = await _uaClient.ReadUaValueAsync(serverUrl, varNode);
                        result["value"] = uaValue.Value;
                        result["value-schema"] = JObject.Parse(uaValue.Schema.ToString());
                        result["status"] = uaValue.StatusCode?.ToString() ?? "";
                        break;
                    case NodeClass.Object:
                        result["type"] = await _uaClient.IsFolderTypeAsync(serverUrl, decodedNodeId) ? "folder" : "object";
                        System.Diagnostics.Debug.WriteLine("result type: " + result);
                        break;
                }

                var linkedNodes = new JArray();

                var refDescriptions = await _uaClient.BrowseAsync(serverUrl, decodedNodeId);
                System.Diagnostics.Debug.WriteLine("refDescriptions  [TEST]" + refDescriptions);

                foreach (var rd in refDescriptions)
                {
                    System.Diagnostics.Debug.WriteLine("refDescriptions Platform Node ID [TEST]" + rd.PlatformNodeId);
                    rd.PlatformNodeId = rd.PlatformNodeId.Remove(1, 1);
                    System.Diagnostics.Debug.WriteLine("refDescriptions Platform Node ID Changed [TEST]" + rd.PlatformNodeId);

                    var refTypeNode = await _uaClient.ReadNodeAsync(serverUrl, rd.ReferenceTypeId);
                    var targetNode = new JObject
                    {
                        ["node-id"] = rd.PlatformNodeId,
                        ["name"] = rd.DisplayName
                    };
                    System.Diagnostics.Debug.WriteLine("refDescriptions PlatformNodeId: " + refDescriptions);
                    System.Diagnostics.Debug.WriteLine("NodeClass.Object First [Test]: " + targetNode);


                    switch (rd.NodeClass)
                    {
                        case NodeClass.Variable:
                            targetNode["Type"] = "variable";
                            break;
                        case NodeClass.Method:
                            targetNode["Type"] = "method";
                            break;
                        case NodeClass.Object:
                            System.Diagnostics.Debug.WriteLine("NodeClass.Object Second [Test]: " + targetNode);

                            targetNode["Type"] = await _uaClient.IsFolderTypeAsync(serverUrl, rd.PlatformNodeId) ? "folder" : "object";

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    targetNode["relationship"] = refTypeNode.DisplayName.Text;

                    linkedNodes.Add(targetNode);
                }

                result["references"] = linkedNodes;
            }
            catch (ServiceResultException exc)
            {
                switch (exc.StatusCode)
                {
                    case StatusCodes.BadNodeIdUnknown:
                        return NotFound(new
                        {
                            error = "Wrong ID: There is no Resource with ID " + decodedNodeId
                        });
                    case StatusCodes.BadNodeIdInvalid:
                        return BadRequest(new
                        {
                            error = "Provided ID is invalid"
                        });
                    case StatusCodes.BadSessionIdInvalid:
                    case StatusCodes.BadSessionClosed:
                    case StatusCodes.BadSessionNotActivated:
                    case StatusCodes.BadTooManySessions:
                        return StatusCode(500, new
                        {
                            error = "Connection Lost"
                        });
                    default:
                        return StatusCode(500, new
                        {
                            error = exc.Message
                        });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Data Set " + DataSetID + " NotAvailable");
            }

            return Ok(result);
        }

        [HttpPost("data-sets/{DataSetID:int}/nodes/{node_id:regex(^\\d+-(?:(\\d+)|(.+))$)?}")]
        public async Task<IActionResult> InsertNodeAsync(int DataSetID, string node_id, [FromBody] VariableState state)
        {
            if (state == null || !state.IsValid)
                return BadRequest(new
                {
                    error = "Insert a valid state for a Variable Node."
                });

            if (DataSetID < 0 || DataSetID >= _uaServers.Length) return NotFound($"There is no Data Set for id {DataSetID}");

            var serverUrl = _uaServers[DataSetID].Url;
            if (!(await _uaClient.IsServerAvailable(serverUrl)))
                return StatusCode(500, new
                {
                    error = "Data Set " + DataSetID + " NotAvailable"
                });

            var decodedNodeId = WebUtility.UrlDecode(node_id);

            Node sourceNode;
            try
            {
                sourceNode = await _uaClient.ReadNodeAsync(serverUrl, decodedNodeId);
            }
            catch (ServiceResultException exc)
            {
                switch (exc.StatusCode)
                {
                    case StatusCodes.BadNodeIdUnknown:
                        return NotFound(new
                        {
                            error = "Wrong ID: There is no Resource with ID " + decodedNodeId
                        });
                    case StatusCodes.BadNodeIdInvalid:
                        return BadRequest(new
                        {
                            error = "Provided ID is invalid"
                        });
                    case StatusCodes.BadSessionIdInvalid:
                    case StatusCodes.BadSessionClosed:
                    case StatusCodes.BadSessionNotActivated:
                    case StatusCodes.BadTooManySessions:
                        return StatusCode(500, new
                        {
                            error = "Connection Lost"
                        });
                    default:
                        return StatusCode(500, new
                        {
                            error = exc.Message
                        });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    error = "Data Set " + DataSetID + " NotAvailable"
                });
            }

            if (sourceNode.NodeClass != NodeClass.Variable)
                return BadRequest(new
                {
                    error = "There is no Value for the Node specified by the NodeId " + node_id
                });

            VariableNode variableNode = (VariableNode)sourceNode;

            try
            {
                await _uaClient.WriteNodeValueAsync(serverUrl, variableNode, state);
            }
            catch (ServiceResultException exc)
            {
                switch (exc.StatusCode)
                {
                    case (StatusCodes.BadTypeMismatch):
                        return BadRequest(new
                        {
                            error = "Wrong Type - Check data and try again"
                        });
                    case StatusCodes.BadSessionIdInvalid:
                    case StatusCodes.BadSessionClosed:
                    case StatusCodes.BadSessionNotActivated:
                    case StatusCodes.BadTooManySessions:
                        return StatusCode(500, new
                        {
                            error = "Connection Lost"
                        });
                    default:
                        return BadRequest(new
                        {
                            error = exc.Message
                        });
                }
            }
            catch(Exception exc)
            {
                return BadRequest(new
                {
                    error = exc.Message
                });
            }
            return Ok("Write on Node {node_id} in the Data Set {DataSetID} executed.");
        }
    }
}

