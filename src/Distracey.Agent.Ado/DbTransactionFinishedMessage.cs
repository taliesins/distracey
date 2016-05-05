using System;
using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class DbTransactionFinishedMessage
    {
        public ShortGuid TransactionId { get; set; }
        public bool Rollback { get; set; }
        public Exception Exception { get; set; }
    }
}
