using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using SemanticAPI.OPCUASemantic;
using System.Threading;

namespace SemanticAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello OPC UA Semantic WEB API");
            Console.WriteLine((Utils.IsRunningOnMono() ? "Mono" : ".Net Core") + "OPC UA Client");

            //Principally, parameters could get via args 
            //But we will use web API with AngularJS GUI
            //Sample OPC UA Endpoint
            string endpointURL = "opc.tcp://desktop-674d0i3:62541/DynamicServer";
            bool autoAccept = false;
            int stopTimeout = Timeout.Infinite;
            try
            {
                Console.WriteLine("Client is running");
                OPCUAClient client = new OPCUAClient(endpointURL, autoAccept, stopTimeout);
                client.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:4000")
                .UseStartup<Startup>();
    }
}
