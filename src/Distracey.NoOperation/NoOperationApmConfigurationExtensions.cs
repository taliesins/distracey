using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Web;

namespace Distracey.NoOperation
{
    public static class NoOperationApmConfigurationExtensions
    {
        public static void AddNoOperationApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            EventLoggerExtensions.ApmMethodHttpFactories.Add(new NoOperationEventLogger());
            NoOperationApmApiFilterAttribute.ApplicationName = applicationName;
            NoOperationApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            var noOperationApmApiFilterAttribute = new NoOperationApmApiFilterAttribute();
            configuration.Filters.Add(noOperationApmApiFilterAttribute);

            configuration.Services.Add(typeof(IExceptionLogger), new NoOperationApmExceptionLogger());
        }
    }
}