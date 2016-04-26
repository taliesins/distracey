using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Web;
using Distracey.Web.HttpClient;

namespace Distracey.NoOperation
{
    public static class NoOperationApmConfigurationExtensions
    {
        public static void AddNoOperationApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            ApmHttpClientApmContextExtensions.ApmHttpClientDelegatingHandlerFactories.Add(new NoOperationApmHttpClientDelegatingHandlerFactory());

            NoOperationApmApiFilterAttribute.ApplicationName = applicationName;
            NoOperationApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            NoOperationApmHttpClientDelegatingHandler.ApplicationName = applicationName;

            var noOperationApmApiFilterAttribute = new NoOperationApmApiFilterAttribute();
            configuration.Filters.Add(noOperationApmApiFilterAttribute);

            configuration.Services.Add(typeof(IExceptionLogger), new NoOperationApmExceptionLogger());
        }
    }
}