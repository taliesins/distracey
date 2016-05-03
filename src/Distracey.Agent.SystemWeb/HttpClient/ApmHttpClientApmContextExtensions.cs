using System.Net.Http;
using Distracey.Common;

namespace Distracey.Agent.SystemWeb.HttpClient
{
    public static class ApmHttpClientApmContextExtensions
    {
        public static ApmHttpClientDelegatingHandler GetDelegatingHandler(this IApmContext apmContext, HttpMessageHandler httpMessageHandler)
        {
            return new ApmHttpClientDelegatingHandler(apmContext, httpMessageHandler);
        }
    }
}
