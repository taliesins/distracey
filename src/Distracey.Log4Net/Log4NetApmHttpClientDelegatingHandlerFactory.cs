namespace Distracey.Log4Net
{
    public class Log4NetApmHttpClientDelegatingHandlerFactory : IApmHttpClientDelegatingHandlerFactory
    {
        public ApmHttpClientDelegatingHandlerBase Create(IApmContext apmContext)
        {
            return new Log4NetApmHttpClientDelegatingHandler(apmContext);
        }
    }
}
