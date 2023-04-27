using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models.Configuration
{
    public struct ServerConfig
    {
        /// <summary>
        /// The IP address the server should listen on.
        /// IPAddress.Parse("127.0.0.1") -> Only listen to connections from the local machine.
        /// IPAddress.Any -> Listen on all available IP addresses.
        /// </summary>
        public IPAddress ServerIP { get; set; } 
        public int ServerPort { get; set; }
        public int MaxConnections { get; set; }
        public int ConnectionTimeout { get; set; }
        public int BufferSize { get; set; }
        public bool WriteLogsToConsole { get; set; }
    }
}
