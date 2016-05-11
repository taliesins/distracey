using System;
using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class DbTransactionFinishedMessage : IMessage
    {
        public ShortGuid TransactionId { get; set; }
        public bool Rollback { get; set; }
        public Exception Exception { get; set; }
    }
}
