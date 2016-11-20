using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;
using Logary;

namespace Distracey.Logary
{
    public static class LogaryApmConfigurationExtensions
    {
        public static void AddLogaryApm(this HttpConfiguration configuration, string applicationName, Logger log)
        {
            EventLoggerExtensions.ApmEventLoggers.Add(new LogaryApmEventLogger(applicationName, log));

            configuration.AddApmWebApiFilter();
            configuration.Services.Add(typeof(IExceptionLogger), new LogaryApmExceptionLogger(log));
        }
    }
}