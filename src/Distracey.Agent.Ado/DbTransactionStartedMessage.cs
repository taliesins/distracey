using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class DbTransactionStartedMessage
    {
        public ShortGuid TransactionId { get; set; }
    }
}
