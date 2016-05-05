using System;
using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class ExecuteScalarAsyncFinishedMessage
    {
        public ShortGuid CommandId { get; set; }
        public Exception Exception { get; set; }
    }
}
