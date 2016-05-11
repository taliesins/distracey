using System;
using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class ExecuteNonQueryAsyncFinishedMessage : IMessage
    {
        public ShortGuid CommandId { get; set; }
        public int RecordsEffected { get; set; }
        public Exception Exception { get; set; }
    }
}
