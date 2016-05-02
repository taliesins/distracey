using Distracey.Common.Helpers;

namespace Distracey.Common.Session
{ 
    public interface ISession 
    {
        ShortGuid SessionId { get; }
        IApmContext ApmContext { get; set; }
    }
}
