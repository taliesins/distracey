using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class ExecuteNonQueryAsyncStartedMessage : IMessage
    {
        public ShortGuid CommandId { get; set; }
        public string CommandText { get; set; }
    }
}
