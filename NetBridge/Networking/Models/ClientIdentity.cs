using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking
{
    [Serializable]
    public struct ClientIdentity
    {
        public Guid GUID { get; set; }

        public ClientIdentity(Guid guid)
        {
            this.GUID = guid;
        }
    }
}
