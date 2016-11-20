using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;

namespace Distracey.NoOperation
{
    public static class NoOperationApmConfigurationExtensions
    {
        public static void AddNoOperationApm(this HttpConfiguration configuration, string applicationName)
        {
            EventLoggerExtensions.ApmEventLoggers.Add(new NoOperationApmEventLogger());

            configuration.AddApmWebApiFilter();
            configuration.Services.Add(typeof(IExceptionLogger), new NoOperationApmExceptionLogger());
        }
    }
}