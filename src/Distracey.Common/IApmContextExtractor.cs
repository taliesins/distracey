using System.Reflection;

namespace Distracey.Common
{
    public interface IApmContextExtractor
    {
        void GetContext(IApmContext apmContext, MethodBase method);
    }
}
