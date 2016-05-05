using System.Transactions;
using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class DbConnectionClosedMessage
    {
        public ShortGuid ConectionId { get; set; }
        public TransactionInformation TransactionInformation { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
        public TransactionStatus Aborted { get; set; }
    }
}
