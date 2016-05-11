using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class DbDataAdapterStartedMessage : IMessage
    {
        public ShortGuid CommandId { get; set; }
    }
}
