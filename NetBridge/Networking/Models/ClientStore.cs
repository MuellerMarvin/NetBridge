using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models
{
    public class ClientStore
    {
        public ClientIdentity Identity { get; set; }
        public TcpClient TcpClient { get; set; }
        public bool IsBusy { get; set; }

        public ClientStore(ClientIdentity identity, TcpClient tcpClient, bool isBusy)
        {
            Identity = identity;
            TcpClient = tcpClient;
            this.IsBusy = isBusy;
        }
    }
}
