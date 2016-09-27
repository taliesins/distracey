using System;
using System.Collections;
using System.Collections.Generic;

namespace Distracey.Common.Session
{ 
    public interface ISession 
    {
        Guid SessionId { get; }
        IDictionary Items { get; }
        Dictionary<Guid, IActivity> Activities { get; set; }
    }
}