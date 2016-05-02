using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Common;
using Distracey.Web;
using Distracey.Web.WebApi;
using Logary;

namespace Distracey.Logary
{
    public static class LogaryApmConfigurationExtensions
    {
        public static void AddLogaryApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders, Logger log)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            EventLoggerExtensions.ApmEventLoggers.Add(new LogaryApmEventLogger(applicationName, log));

            configuration.AddApmWebApiFilter(addResponseHeaders);
            configuration.Services.Add(typeof(IExceptionLogger), new LogaryApmExceptionLogger(log));
        }
    }
}