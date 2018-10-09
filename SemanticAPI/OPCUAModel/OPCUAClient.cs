using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using SemanticAPI.Controllers;
using SemanticAPI.Models.OPCUADataType;
using SemanticAPI.Models.DataSet;
//TODO Logger class is supposed to be added.

namespace SemanticAPI.OPCUAModel
{
    //public static ExitCode ExitCode { get => exitCode; }

    public interface IOPCUAClient
    {
        Task<Node> ReadNodeAsync(string serverUrl, string nodeIdStr);
        Task<Node> ReadNodeAsync(string serverUrl, NodeId nodeId);
        Task<IEnumerable<EdgeDescription>> BrowseAsync(string serverUrl, string nodeToBrowseIdStr);
        Task<UaValue> ReadUaValueAsync(string serverUrl, VariableNode varNode);
        Task<bool> WriteNodeValueAsync(string serverUrl, VariableNode variableNode, VariableState state);
        Task<bool> IsFolderTypeAsync(string serverUrlstring, string nodeIdStr);
        Task<bool> IsServerAvailable(string serverUrlstring);
    }

    public interface IOPCUAClientSingleton : IOPCUAClient { }

    public class OPCUAClient : IOPCUAClientSingleton
    {

        private readonly ApplicationInstance _application;
        private ApplicationConfiguration _appConfiguration;
        private bool _autoAccept;

        //A Dictionary containing al the active Sessions, indexed per server Id.
        private readonly Dictionary<string, Session> _sessions;

        public OPCUAClient()
        {
            _application = new ApplicationInstance
            {
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "SemanticAPI"
            };

            _sessions = new Dictionary<string, Session>();
        }

        public async Task<Node> ReadNodeAsync(string serverUrl, string nodeIdStr)
        {
            Session session = await GetSessionAsync(serverUrl);
            NodeId nodeToRead = ParserUtils.ParsePlatformNodeIdString(nodeIdStr);
            var node = session.ReadNode(nodeToRead);
            return node;
        }

        public async Task<Node> ReadNodeAsync(string serverUrl, NodeId nodeToRead)
        {
            Session session = await GetSessionAsync(serverUrl);
            Node node;
            node = session.ReadNode(nodeToRead);
            return node;
        }


        public async Task<bool> WriteNodeValueAsync(string serverUrl, VariableNode variableNode, VariableState state)
        {
            Session session = await GetSessionAsync(serverUrl);
            var typeManager = new DataTypeManager(session);
            WriteValueCollection writeValues = new WriteValueCollection();

            WriteValue writeValue = new WriteValue
            {
                NodeId = variableNode.NodeId,
                AttributeId = Attributes.Value,
                Value = typeManager.GetDataValueFromVariableState(state, variableNode)
            };

            writeValues.Add(writeValue);

            session.Write(null, writeValues, out var results, out _);
            if (!StatusCode.IsGood(results[0]))
            {
                if (results[0] == StatusCodes.BadTypeMismatch)
                    throw new Exception("Wrong Type Error: data sent are not of the type expected. Check your data and try again");
                throw new Exception(results[0].ToString());
            }
            return true;
        }


        public async Task<IEnumerable<EdgeDescription>> BrowseAsync(string serverUrl, string nodeToBrowseIdStr)
        {
            Session session = await GetSessionAsync(serverUrl);
            NodeId nodeToBrowseId = ParserUtils.ParsePlatformNodeIdString(nodeToBrowseIdStr);
            System.Diagnostics.Debug.WriteLine("NodeToBrowseID [TEST] " + nodeToBrowseId);
            var browser = new Browser(session)
            {
                NodeClassMask = (int)NodeClass.Method | (int)NodeClass.Object | (int)NodeClass.Variable,
                ResultMask = (uint)BrowseResultMask.DisplayName | (uint)BrowseResultMask.NodeClass | (uint)BrowseResultMask.ReferenceTypeInfo,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences
            };
            System.Diagnostics.Debug.WriteLine("browser to String[TEST] " + browser.Browse(nodeToBrowseId));
            
            var sender = browser.Browse(nodeToBrowseId)
                .Select(rd => new EdgeDescription(rd.NodeId.ToStringId(session.MessageContext.NamespaceUris),
                    rd.DisplayName.Text,
                    rd.NodeClass,
                    rd.ReferenceTypeId)
                    );
        System.Diagnostics.Debug.WriteLine("sender to String[TEST] " + sender);

            return sender;
        }

        //private object _lock = new object();

        public async Task<bool> IsFolderTypeAsync(string serverUrl, string nodeIdStr)
        {
                Session session = await GetSessionAsync(serverUrl);
                    NodeId nodeToBrowseId = ParserUtils.ParsePlatformNodeIdString(nodeIdStr);

                    //Set a Browser object to follow HasTypeDefinition Reference only
                    var browser = new Browser(session)
                    {
                        ResultMask = (uint)BrowseResultMask.DisplayName | (uint)BrowseResultMask.TargetInfo,
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition
                    };


                    ReferenceDescription refDescription = browser.Browse(nodeToBrowseId)[0];
                    NodeId targetId = ExpandedNodeId.ToNodeId(refDescription.NodeId, session.MessageContext.NamespaceUris);

                    //Once got the Object Type, set the browser to follow Type hierarchy in inverse order.
                    browser.ReferenceTypeId = ReferenceTypeIds.HasSubtype;
                    browser.BrowseDirection = BrowseDirection.Inverse;

                    while (targetId != ObjectTypeIds.FolderType && targetId != ObjectTypeIds.BaseObjectType)
                    {
                        refDescription = browser.Browse(targetId)[0];
                        targetId = ExpandedNodeId.ToNodeId(refDescription.NodeId, session.MessageContext.NamespaceUris);
                    }
                    return targetId == ObjectTypeIds.FolderType;
        }

        public async Task<UaValue> ReadUaValueAsync(string serverUrl, VariableNode variableNode)
        {
            Session session = await GetSessionAsync(serverUrl);
            var typeManager = new DataTypeManager(session);

            return typeManager.GetUaValue(variableNode);
        }

        public async Task<bool> IsServerAvailable(string serverUrlstring)
        {
            Session session;
            try
            {
                session = await GetSessionAsync(serverUrlstring);
            }
            catch (Exception exc)
            {
                return false;
            }
            if (session.IsServerStatusGood())
                return true;
            return await RestoreSessionAsync(serverUrlstring);
        }

        private static Subscription CreateSubscription(Session session, int publishingInterval, uint maxNotificationPerPublish)
        {
            var sub = new Subscription(session.DefaultSubscription)
            {
                PublishingInterval = publishingInterval,
                MaxNotificationsPerPublish = maxNotificationPerPublish
            };

            if (!session.AddSubscription(sub)) return null;
            sub.Create();
            return sub;

        }

        /// <summary>
        /// This method is called when a OPC UA Service call in a session object returns an error 
        /// </summary>
        /// <param name="serverUrlstring"></param>
        /// <returns></returns>
        private async Task<bool> RestoreSessionAsync(string serverUrlstring)
        {
            lock (_sessions)
            {
                if (_sessions.ContainsKey(serverUrlstring))
                    _sessions.Remove(serverUrlstring);
            }

            Session session;
            try
            {
                return (await GetSessionAsync(serverUrlstring)).IsServerStatusGood();
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<Session> GetSessionAsync(string serverUrl)
        {
            lock (_sessions)
            {
                if (_sessions.ContainsKey(serverUrl)) return _sessions[serverUrl];
            }

            await CheckAndLoadConfiguration();
            EndpointDescription endpointDescription;
            try
            {
                endpointDescription = CoreClientUtils.SelectEndpoint(serverUrl, true, 15000);
            }
            catch (Exception)
            {
                throw new Exception();
            }

            Console.WriteLine("    Selected endpoint uses: {0}",
                endpointDescription.SecurityPolicyUri.Substring(endpointDescription.SecurityPolicyUri.LastIndexOf('#') + 1));

            var endpointConfiguration = EndpointConfiguration.Create(_appConfiguration);

            var endpoint = new ConfiguredEndpoint(endpointDescription.Server, endpointConfiguration);
            endpoint.Update(endpointDescription);

            var s = await Session.Create(_appConfiguration,
                                             endpoint,
                                             true,
                                             false,
                                             _appConfiguration.ApplicationName + "_session",
                                             (uint)_appConfiguration.ClientConfiguration.DefaultSessionTimeout,
                                             null,
                                             null);

            lock (_sessions)
            {
                if (_sessions.ContainsKey(serverUrl))
                    s = _sessions[serverUrl];
                else
                    _sessions.Add(serverUrl, s);
            }

            return s;
        }

        private async Task CheckAndLoadConfiguration()
        {
            if (_appConfiguration == null)
            {
                _appConfiguration = await _application.LoadApplicationConfiguration(false);

                var haveAppCertificate = await _application.CheckApplicationInstanceCertificate(false, 0);
                if (!haveAppCertificate)
                {
                    throw new Exception("Application instance certificate invalid!");
                }

                _appConfiguration.ApplicationUri =
                    Utils.GetApplicationUriFromCertificate(_appConfiguration.SecurityConfiguration.ApplicationCertificate
                        .Certificate);
                if (_appConfiguration.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    _autoAccept = true;
                }

                _appConfiguration.CertificateValidator.CertificateValidation += CertificateValidator_CertificateValidation;
            }
        }

        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = _autoAccept;
                Console.WriteLine(_autoAccept ? "Accepted Certificate: {0}" : "Rejected Certificate: {0}",
                    e.Certificate.Subject);
            }
        }
    }
}
