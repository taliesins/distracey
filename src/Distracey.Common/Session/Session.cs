using System;

namespace Distracey.Common.Session
{
    [Serializable]
    public class Session : ISession
    {
        public Guid SessionId { get; private set; }
        public string TraceId { get; set; }
        public string Sampled { get; set; }
        public string Flags { get; set; }

        public Session()
        {
            SessionId = Guid.NewGuid();
        }
    }
}
