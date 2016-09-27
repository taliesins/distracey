using System;
using System.Collections;

namespace Distracey.Common.Session
{
    [Serializable]
    public class Activity : IActivity
    {
        public Guid ActivityId { get; private set; }

        public IDictionary Items { get; private set; }

        public Activity(Guid activityId)
        {
            ActivityId = activityId;
            Items = new Hashtable();
        }
    }
}
