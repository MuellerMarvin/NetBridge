using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models
{
    public struct NetworkTask
    {
        public int Id { get; set; }
        public int TaskType { get; set; }
        public Object[] Payload { get; set; }
    }
}
