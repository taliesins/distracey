using System;

namespace Distracey.Common.Session
{
    [Serializable]
    public class Session : ISession
    {
        public Guid SessionId { get; private set; }
        public IApmContext ApmContext { get; set; }

        public Session()
        {
            SessionId = Guid.NewGuid();
        }
    }
}
