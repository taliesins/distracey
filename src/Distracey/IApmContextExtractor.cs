using System.Reflection;

namespace Distracey
{
    public interface IApmContextExtractor
    {
        void GetContext(IApmContext apmContext, MethodBase method);
    }
}
