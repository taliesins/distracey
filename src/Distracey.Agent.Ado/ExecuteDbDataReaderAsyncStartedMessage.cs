using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class ExecuteDbDataReaderAsyncStartedMessage : IMessage
    {
        public ShortGuid CommandId { get; set; }
    }
}
