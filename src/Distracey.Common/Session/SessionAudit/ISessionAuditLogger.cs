using System.Threading.Tasks;

namespace Distracey.Common.Session.SessionAudit
{
    public interface ISessionAuditLogger : IEventLogger
    {
        Task OnSessionAuditMessage(Task<SessionAuditMessage> task);
    }
}
