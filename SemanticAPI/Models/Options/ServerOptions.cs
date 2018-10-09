using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemanticAPI.Models.Options
{
    public class ServerOptions
    {
        public ServerOptions()
        {
            //Constructor
        }

        public ServerIdentities[] Servers { get; set; }
    }

    public class ServerIdentities 
    {

        public ServerIdentities() { }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}
