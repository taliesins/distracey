using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Common;
using Distracey.Web;
using Distracey.Web.WebApi;

namespace Distracey.NoOperation
{
    public static class NoOperationApmConfigurationExtensions
    {
        public static void AddNoOperationApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            EventLoggerExtensions.ApmEventLoggers.Add(new NoOperationApmEventLogger());

            configuration.AddApmWebApiFilter(addResponseHeaders);
            configuration.Services.Add(typeof(IExceptionLogger), new NoOperationApmExceptionLogger());
        }
    }
}