namespace Distracey.Logary
{
    public class LogaryApmMethodHandlerFactory : IApmMethodHandlerFactory
    {
        public ApmMethodHandlerBase Create(IApmContext apmContext)
        {
            return new LogaryApmMethodHandler(apmContext);
        }
    }
}
