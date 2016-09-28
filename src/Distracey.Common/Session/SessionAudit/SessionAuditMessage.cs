using Distracey.Common.Message;

namespace Distracey.Common.Session.SessionAudit
{
    public class SessionAuditMessage : IMessage
    {
        public ISession Session { get; set; }
    }
}
