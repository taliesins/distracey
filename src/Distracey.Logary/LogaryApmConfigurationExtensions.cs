using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Web;
using Logary;

namespace Distracey.Logary
{
    public static class LogaryApmConfigurationExtensions
    {
        public static void AddLogaryApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders, Logger log)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            EventLoggerExtensions.ApmMethodHttpFactories.Add(new LogaryEventLogger(applicationName, log));

            LogaryApmApiFilterAttribute.ApplicationName = applicationName;
            LogaryApmApiFilterAttribute.Log = log;
            LogaryApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            var logaryApmApiFilterAttribute = new LogaryApmApiFilterAttribute();
            configuration.Filters.Add(logaryApmApiFilterAttribute);

            configuration.Services.Add(typeof(IExceptionLogger), new LogaryApmExceptionLogger(log));
        }
    }
}