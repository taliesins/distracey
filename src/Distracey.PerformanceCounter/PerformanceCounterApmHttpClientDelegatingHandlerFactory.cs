using Distracey.Web.HttpClient;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmHttpClientDelegatingHandlerFactory : IApmHttpClientDelegatingHandlerFactory
    {
        public ApmHttpClientDelegatingHandlerBase Create(IApmContext apmContext)
        {
            return new PerformanceCounterApmHttpClientDelegatingHandler(apmContext);
        }
    }
}
