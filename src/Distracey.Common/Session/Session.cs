using System;
using System.Collections;
using System.Collections.Generic;

namespace Distracey.Common.Session
{
    [Serializable]
    public class Session : ISession
    {
        public Guid SessionId { get; private set; }
        public IDictionary Items { get; private set; }
        public Dictionary<Guid, IActivity> Activities { get; set; }

        public Session()
        {
            SessionId = Guid.NewGuid();
            Items = new Hashtable();
            Activities = new Dictionary<Guid, IActivity>();
        }
    }
}
