using Distracey.Common;

namespace Distracey.Agent.Core.MethodHandler
{
    public static class ApmMethodHandlerApmContextExtensions
    {
        public static ApmMethodHandler GetMethodHander(this IApmContext apmContext)
        {
            return new ApmMethodHandler(apmContext);
        }
    }
}
