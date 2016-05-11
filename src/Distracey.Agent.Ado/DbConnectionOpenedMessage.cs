using System.Transactions;
using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class DbConnectionOpenedMessage : IMessage
    {
        public ShortGuid ConectionId { get; set; }
        public TransactionInformation TransactionInformation { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
    }
}
