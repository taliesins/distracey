using System;

namespace Distracey.Common.Session
{ 
    public interface ISession 
    {
        Guid SessionId { get; }
        IApmContext ApmContext { get; set; }
    }
}