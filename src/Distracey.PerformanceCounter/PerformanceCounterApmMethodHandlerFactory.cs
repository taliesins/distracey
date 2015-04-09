namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmMethodHandlerFactory : IApmMethodHandlerFactory
    {
        public ApmMethodHandlerBase Create(IApmContext apmContext)
        {
            return new PerformanceCounterApmMethodHandler();
        }
    }
}
