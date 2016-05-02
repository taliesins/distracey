using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Web;
using Distracey.Web.WebApi;

namespace Distracey.NoOperation
{
    public static class NoOperationApmConfigurationExtensions
    {
        public static void AddNoOperationApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            EventLoggerExtensions.ApmEventLoggers.Add(new NoOperationEventLogger());

            if (configuration.Filters.All(x => x.GetType() != typeof(ApmWebApiFilterAttribute)))
            {
                var apmWebApiFilterAttribute = new ApmWebApiFilterAttribute(addResponseHeaders);
                configuration.Filters.Add(apmWebApiFilterAttribute);
            }

            configuration.Services.Add(typeof(IExceptionLogger), new NoOperationApmExceptionLogger());
        }
    }
}