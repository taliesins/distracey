using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.MethodHandler;
using Distracey.Web;
using Distracey.Web.HttpClient;
using Logary;

namespace Distracey.Logary
{
    public static class LogaryApmConfigurationExtensions
    {
        public static void AddLogaryApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders, Logger log)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            ApmHttpClientApmContextExtensions.ApmHttpClientDelegatingHandlerFactories.Add(new LogaryApmHttpClientDelegatingHandlerFactory());
            ApmMethodHandlerApmContextExtensions.ApmMethodHttpFactories.Add(new LogaryApmMethodHandlerFactory());

            LogaryApmApiFilterAttribute.ApplicationName = applicationName;
            LogaryApmApiFilterAttribute.Log = log;
            LogaryApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            LogaryApmHttpClientDelegatingHandler.ApplicationName = applicationName;
            LogaryApmHttpClientDelegatingHandler.Log = log;

            LogaryApmMethodHandler.ApplicationName = applicationName;
            LogaryApmMethodHandler.Log = log;

            var logaryApmApiFilterAttribute = new LogaryApmApiFilterAttribute();
            configuration.Filters.Add(logaryApmApiFilterAttribute);

            configuration.Services.Add(typeof(IExceptionLogger), new LogaryApmExceptionLogger(log));
        }
    }
}