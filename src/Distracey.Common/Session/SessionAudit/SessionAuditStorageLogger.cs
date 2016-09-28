using Distracey.Common.EventAggregator;

namespace Distracey.Common.Session.SessionAudit
{
    public class SessionAuditStorageLogger : SessionAuditStorageBase
    {
        protected override void Save(ISession session)
        {
            var sessionAuditMessage = new SessionAuditMessage
            {
                Session = session
            };

            this.Publish(sessionAuditMessage).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
