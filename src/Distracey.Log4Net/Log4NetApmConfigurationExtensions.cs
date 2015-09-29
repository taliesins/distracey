using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Web;
using Distracey.Web.HttpClient;
using log4net;

namespace Distracey.Log4Net
{
    public static class Log4NetApmConfigurationExtensions
    {
        public static void AddLog4NetApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders, ILog log)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            ApmHttpClientApmContextExtensions.ApmHttpClientDelegatingHandlerFactories.Add(new Log4NetApmHttpClientDelegatingHandlerFactory());

            Log4NetApmApiFilterAttribute.ApplicationName = applicationName;
            Log4NetApmApiFilterAttribute.Log = log;
            Log4NetApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            Log4NetApmHttpClientDelegatingHandler.ApplicationName = applicationName;
            Log4NetApmHttpClientDelegatingHandler.Log = log;

            var log4NetApmApiFilterAttribute = new Log4NetApmApiFilterAttribute();
            configuration.Filters.Add(log4NetApmApiFilterAttribute);

            configuration.Services.Add(typeof(IExceptionLogger), new Log4NetApmExceptionLogger(log));
        }
    }
}