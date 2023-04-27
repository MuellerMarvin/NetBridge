using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models
{
    [Serializable]
    public class NetworkTask<PayloadType>
    {
        public Guid Guid { get; set; }
        public PayloadType Payload { get; set; }

        public NetworkTask() { }

        [JsonConstructor]
        public NetworkTask(Guid guid, PayloadType payload)
        {
            Guid = guid;
            this.Payload = payload;
        }
    }
}
