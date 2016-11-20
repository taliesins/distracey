using Distracey.Common.Session;

namespace Distracey.Common.Message
{
    public interface ISessionMessage : IMessage
    {
        ISession Session { get; }
    }
}
