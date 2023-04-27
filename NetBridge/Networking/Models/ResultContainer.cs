using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBridge.Networking.Models
{
    internal class ResultContainer
    {
        [AllowNull]
        public Object Result = null;

        public ResultContainer(object result)
        {
            this.Result = result;
        }
    }
}
