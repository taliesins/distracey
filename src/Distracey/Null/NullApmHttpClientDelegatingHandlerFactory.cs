using Distracey.Web.HttpClient;

namespace Distracey.Null
{
    public class NullApmHttpClientDelegatingHandlerFactory : IApmHttpClientDelegatingHandlerFactory
    {
        public ApmHttpClientDelegatingHandlerBase Create(IApmContext apmContext)
        {
            return new NullApmHttpClientDelegatingHandler(apmContext);
        }
    }
}
