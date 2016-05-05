using System.Transactions;
using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class DbConnectionOpenedMessage
    {
        public ShortGuid ConectionId { get; set; }
        public TransactionInformation TransactionInformation { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
    }
}
