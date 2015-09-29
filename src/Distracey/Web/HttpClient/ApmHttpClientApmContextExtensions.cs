using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Distracey.Null;

namespace Distracey.Web.HttpClient
{
    public static class ApmHttpClientApmContextExtensions
    {
        public static readonly List<IApmHttpClientDelegatingHandlerFactory> ApmHttpClientDelegatingHandlerFactories = new List<IApmHttpClientDelegatingHandlerFactory>();
        
        public static ApmHttpClientDelegatingHandlerBase GetDelegatingHandler(this IApmContext apmContext)
        {
            if (!ApmHttpClientDelegatingHandlerFactories.Any())
            {
                return new NullApmHttpClientDelegatingHandlerFactory().Create(apmContext);
            }

            ApmHttpClientDelegatingHandlerBase apmHttpClientDelegatingHandler = null;

            foreach (var apmHttpClientDelegatingHandlerFactory in ApmHttpClientDelegatingHandlerFactories)
            {
                var currentApmHttpClientDelegatingHandler = apmHttpClientDelegatingHandlerFactory.Create(apmContext);
                if (apmHttpClientDelegatingHandler != null)
                {
                    currentApmHttpClientDelegatingHandler.InnerHandler = apmHttpClientDelegatingHandler;
                }
                else
                {
                    currentApmHttpClientDelegatingHandler.InnerHandler = new HttpClientHandler();
                }
                apmHttpClientDelegatingHandler = currentApmHttpClientDelegatingHandler;
            }

            return apmHttpClientDelegatingHandler;
        }
    }
}
