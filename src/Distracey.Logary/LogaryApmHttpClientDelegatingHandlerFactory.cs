using Distracey.Web.HttpClient;

namespace Distracey.Logary
{
    public class LogaryApmHttpClientDelegatingHandlerFactory : IApmHttpClientDelegatingHandlerFactory
    {
        public ApmHttpClientDelegatingHandlerBase Create(IApmContext apmContext)
        {
            return new LogaryApmHttpClientDelegatingHandler(apmContext);
        }
    }
}
