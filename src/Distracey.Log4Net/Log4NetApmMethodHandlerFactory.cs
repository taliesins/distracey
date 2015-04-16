namespace Distracey.Log4Net
{
    public class Log4NetApmMethodHandlerFactory : IApmMethodHandlerFactory
    {
        public ApmMethodHandlerBase Create(IApmContext apmContext)
        {
            return new Log4NetApmMethodHandler(apmContext);
        }
    }
}
