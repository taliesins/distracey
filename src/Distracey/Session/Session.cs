using System;
using Distracey.Helpers;

namespace Distracey.Session
{
    [Serializable]
    public class Session : ISession
    {
        public ShortGuid SessionId { get; private set; }
        public IApmContext ApmContext { get; set; }

        public Session()
        {
            SessionId = ShortGuid.NewGuid();
        }
    }
}
