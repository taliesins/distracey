using System;
using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class ExecuteDbDataReaderFinishedMessage
    {
        public ShortGuid CommandId { get; set; }
        public int RecordsEffected { get; set; }
        public Exception Exception { get; set; }
    }
}
