using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Web;
using log4net;

namespace Distracey.Log4Net
{
    public static class Log4NetApmConfigurationExtensions
    {
        public static void AddLog4NetApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders, ILog log)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            EventLoggerExtensions.ApmMethodHttpFactories.Add(new Log4NetEventLogger(applicationName, log));

            Log4NetApmApiFilterAttribute.ApplicationName = applicationName;
            Log4NetApmApiFilterAttribute.Log = log;
            Log4NetApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            var log4NetApmApiFilterAttribute = new Log4NetApmApiFilterAttribute();
            configuration.Filters.Add(log4NetApmApiFilterAttribute);

            configuration.Services.Add(typeof(IExceptionLogger), new Log4NetApmExceptionLogger(log));
        }
    }
}