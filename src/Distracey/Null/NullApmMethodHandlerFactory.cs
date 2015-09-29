using Distracey.MethodHandler;

namespace Distracey.Null
{
    public class NullApmMethodHandlerFactory : IApmMethodHandlerFactory
    {
        public ApmMethodHandlerBase Create(IApmContext apmContext)
        {
            return new NullApmMethodHandler(apmContext);
        }
    }
}
