using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models.Events
{
    public class TaskCompleteEvent : EventArgs
    {
        public Guid TaskId { get; set; }
        [AllowNull]
        public object Result { get; set; }
    }

    public class TaskCompleteEvent<ResultType> : EventArgs
    {
        public Guid TaskGuid { get; set; }
        public ResultType ResultPayload { get; set; }
    }
}
