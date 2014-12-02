using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Logary;

namespace Distracey.Logary
{
    public static class LogaryApmConfigurationExtensions
    {
        public static void AddLogaryApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders, Logger log)
        {
            ApmContext.ApmHttpClientDelegatingHandlerFactories.Add(new LogaryApmHttpClientDelegatingHandlerFactory());

            LogaryApmApiFilterAttribute.ApplicationName = applicationName;
            LogaryApmApiFilterAttribute.Log = log;
            LogaryApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            LogaryApmHttpClientDelegatingHandler.ApplicationName = applicationName;
            LogaryApmHttpClientDelegatingHandler.Log = log;

            var logaryApmApiFilterAttribute = new LogaryApmApiFilterAttribute();
            configuration.Filters.Add(logaryApmApiFilterAttribute);

            configuration.Services.Add(typeof(IExceptionLogger), new LogaryApmExceptionLogger(log));
        }
    }
}