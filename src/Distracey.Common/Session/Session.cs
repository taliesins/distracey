using System;
using Distracey.Common.Helpers;

namespace Distracey.Common.Session
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
