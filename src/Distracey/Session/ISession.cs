using Distracey.Helpers;

namespace Distracey.Session
{ 
    public interface ISession 
    {
        ShortGuid SessionId { get; }
        IApmContext ApmContext { get; set; }
    }
}
