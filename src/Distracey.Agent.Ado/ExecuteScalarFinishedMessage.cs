using System;
using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class ExecuteScalarFinishedMessage
    {
        public ShortGuid CommandId { get; set; }
        public Exception Exception { get; set; }
    }
}
