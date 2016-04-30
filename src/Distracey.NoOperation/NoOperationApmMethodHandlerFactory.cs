using Distracey.MethodHandler;

namespace Distracey.NoOperation
{
    public class NoOperationApmMethodHandlerFactory : IApmMethodHandlerFactory
    {
        public ApmMethodHandlerBase Create(IApmContext apmContext)
        {
            return new NoOperationApmMethodHandler(apmContext);
        }
    }
}
