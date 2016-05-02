using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
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

            EventLoggerExtensions.ApmEventLoggers.Add(new LogaryEventLogger(applicationName, log));

            if (configuration.Filters.All(x => x.GetType() != typeof(ApmWebApiFilterAttribute)))
            {
                var apmWebApiFilterAttribute = new ApmWebApiFilterAttribute(addResponseHeaders);
                configuration.Filters.Add(apmWebApiFilterAttribute);
            }

            configuration.Services.Add(typeof(IExceptionLogger), new LogaryApmExceptionLogger(log));
        }
    }
}