using System;

namespace Distracey.Common.Session
{ 
    public interface ISession 
    {
        Guid SessionId { get; }
        string TraceId { get; set; }
        string Sampled { get; set; }
        string Flags { get; set; }
    }
}