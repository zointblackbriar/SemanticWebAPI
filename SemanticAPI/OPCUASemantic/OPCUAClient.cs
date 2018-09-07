using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using SemanticAPI.Helpers;

//TODO Logger class is supposed to be added.

namespace SemanticAPI.OPCUASemantic
{
    //Error Set
    public enum ExitCode : int
    {
        Ok = 0,
        ErrorCreateApplication = 0x11,
        ErrorDiscoverEndpoints = 0x12,
        ErrorCreateSession = 0x13,
        ErrorBrowseNamespace = 0x14,
        ErrorCreateSubscription = 0x15,
        ErrorMonitoredItem = 0x16,
        ErrorAddSubscription = 0x17,
        ErrorRunning = 0x18,
        ErrorNoKeepAlive = 0x30,
        ErrorInvalidCommandLine = 0x100
    };

    //public static ExitCode ExitCode { get => exitCode; }

    public class OPCUAClient
    {
        const int ReconnectPeriod = 10;
        Session session = null;
        SessionReconnectHandler reconnectHandler = null;
        string endpointURL = null;
        int clientRunTime = Timeout.Infinite; // System.Threading Library
        static bool autoAccept = false;
        static ExitCode exitCode;

        public OPCUAClient(string _endpointURL, bool _autoAccept, int _stopTimeout)
        {
            //Constructor assignment
            endpointURL = _endpointURL;
            autoAccept = _autoAccept;
            clientRunTime = _stopTimeout <= 0 ? Timeout.Infinite : _stopTimeout * 1000;
        }

        public void Run()
        {
            try
            {
                SampleClient().Wait();
            } catch (Exception e)
            {
                Utils.Trace("ServíceResultException:" + e.Message);
                Console.WriteLine("Error code is" + e.Message);
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task SampleClient()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            //XmlDocument log4netConfig = new XmlDocument();
            //log4netConfig.Load(File.OpenRead("log4net.config"));
            Console.WriteLine("It started");
            try
            {
                Run();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception definiton: ", e.Message);
            }
            Console.WriteLine("Configuration of Application");
            //Take the exit code
            //You cannot reach via this. Because this member is static
            exitCode = ExitCode.ErrorCreateApplication;

            ApplicationInstance application = new ApplicationInstance
            {
                ApplicationName = "UA Core Client",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = Utils.IsRunningOnMono() ? "Opc.Ua.MonoSampleClient" : "Opc.Ua.SampleClient"
            };

            //get all config data as asynchronous
            ApplicationConfiguration configuration = await application.LoadApplicationConfiguration(false);


            //TODO: Check certificate if required
            //asynchronous check
            bool haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, 0);
            if(haveAppCertificate == false)
            {
                throw new Exception("Certificate cannot be found");
            }

            Console.WriteLine("Discover endpoints of {0}", endpointURL);
            //obtain exit code
            exitCode = ExitCode.ErrorDiscoverEndpoints;

            //public static EndpointDescription SelectEndpoint(string discoveryUrl, 
                                                            // bool useSecurity,
                                                            // int operationTimeout = -1 (default)
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointURL, haveAppCertificate, 15000);
            //print out selectedEndpoint
            Console.WriteLine("selected endpoint: " + selectedEndpoint);


            //Create a session with OPCUA Server
            exitCode = ExitCode.ErrorDiscoverEndpoints;
            var endpointConf = EndpointConfiguration.Create(configuration);
            var endpointSet = new ConfiguredEndpoint(null, selectedEndpoint, endpointConf);
            //create a session
            //TODO:we should handle session in a different class
            session = await Session.Create(configuration, endpointSet, false, "OPC UA Console Client",
                                            60000, new UserIdentity(new AnonymousIdentityToken()), null);


            //If there is no data to send after the next PublishingInterval, the server
            // will skip it.
            session.KeepAlive += KeepAlive;
            Console.WriteLine("session parameters: " + session);

            Console.WriteLine("Browse the OPC UA server namespace");
            exitCode = ExitCode.ErrorBrowseNamespace;
            ReferenceDescriptionCollection references;
            Byte[] continuationPoint;

            references = session.FetchReferences(ObjectIds.ObjectsFolder);

            //Session Browsing
            session.Browse(
                null,
                null,
                ObjectIds.ObjectsFolder,
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HierarchicalReferences,
                true,
                (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                out continuationPoint,
                out references);

            Console.WriteLine("Write all the references DisplayName, BrowserName, NodeClass");

        }

        //KeepAlive handler
        
        private void KeepAlive(Session sender, KeepAliveEventArgs e)
        {
            if(e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                Console.WriteLine("Keep Alive Handler");

                if(reconnectHandler == null)
                {
                    reconnectHandler = new SessionReconnectHandler();
                    reconnectHandler.BeginReconnect(sender, ReconnectPeriod * 1000, ReconnectComplete);
                }
            }
        }

        //Handler of reconnection
        private void ReconnectComplete(object sender, EventArgs e)
        {
            if(!Object.ReferenceEquals(sender, reconnectHandler))
            {
                //exit from function
                return;
            }

            session = reconnectHandler.Session;
            reconnectHandler.Dispose();
            reconnectHandler = null;
        }



    }


}
