using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class ExecuteNonQueryStartedMessage : IMessage
    {
        public ShortGuid CommandId { get; set; }
    }
}
