using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models
{
    [Serializable]
    public class NetworkTask
    {
        public Guid Guid { get; set; }

        public T Payload { get; set; }

        public NetworkTask() { }

        [JsonConstructor]
        protected NetworkTask(Guid guid, payload)
        {
            Guid = guid;
            Payload 
        }
    }
}
