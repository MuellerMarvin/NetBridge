using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models.Configuration
{
    [Serializable]
    public struct ClientConfig
    {
        public IPAddress ServerIP { get; set; }
        public int ServerPort { get; set; }
        public ClientIdentity Identity { get; set; }
        public bool WriteLogsToConsole { get; set; }

        /// <summary>
        /// Default values for the client configuration.
        /// </summary>
        public ClientConfig() {
            this.ServerIP = IPAddress.Parse("127.0.0.1");
            this.ServerPort = 1300;
            this.Identity = new ClientIdentity();
            this.WriteLogsToConsole = true;
        }
    }
}
