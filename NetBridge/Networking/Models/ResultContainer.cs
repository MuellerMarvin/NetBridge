using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models
{
    internal class ResultContainer<ResultType>
    {
        public Guid TaskGuid { get; set; }
        public ResultType ResultPayload { get; set; }

        public ResultContainer(Guid taskGuid, ResultType resultPayload)
        {
            this.TaskGuid = taskGuid;
            this.ResultPayload = resultPayload;
        }
    }
}
