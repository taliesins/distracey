using System;
using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class ExecuteNonQueryAsyncFinishedMessage
    {
        public ShortGuid CommandId { get; set; }
        public int RecordsEffected { get; set; }
        public Exception Exception { get; set; }
    }
}
