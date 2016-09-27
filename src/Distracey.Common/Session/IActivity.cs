using System;
using System.Collections;

namespace Distracey.Common.Session
{ 
    public interface IActivity
    {
        Guid ActivityId { get; }
        IDictionary Items { get; }
    }
}