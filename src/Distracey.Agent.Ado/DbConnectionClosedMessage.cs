using System.Transactions;
using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class DbConnectionClosedMessage : IMessage
    {
        public ShortGuid ConectionId { get; set; }
        public TransactionInformation TransactionInformation { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
        public TransactionStatus Aborted { get; set; }
    }
}
