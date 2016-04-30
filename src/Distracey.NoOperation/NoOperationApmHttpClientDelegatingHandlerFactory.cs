using Distracey.Web.HttpClient;

namespace Distracey.NoOperation
{
    public class NoOperationApmHttpClientDelegatingHandlerFactory : IApmHttpClientDelegatingHandlerFactory
    {
        public ApmHttpClientDelegatingHandlerBase Create(IApmContext apmContext)
        {
            return new NoOperationApmHttpClientDelegatingHandler(apmContext);
        }
    }
}
