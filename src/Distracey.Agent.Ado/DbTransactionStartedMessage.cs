using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class DbTransactionStartedMessage : IMessage
    {
        public ShortGuid TransactionId { get; set; }
    }
}
