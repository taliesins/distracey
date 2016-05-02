using System.Web.Http;
using Distracey.Web;

namespace Distracey.PerformanceCounter
{
    public static class PerformanceCounterApmConfigurationExtensions
    {
        public static void AddPerformanceCountersApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            PerformanceCounterEventLogger.ApplicationName = applicationName;
            EventLoggerExtensions.ApmMethodHttpFactories.Add(new PerformanceCounterEventLogger());

            PerformanceCounterApmApiFilterAttribute.ApplicationName = applicationName;
            PerformanceCounterApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            var performanceCounterApmApiFilterAttribute = new PerformanceCounterApmApiFilterAttribute();
            configuration.Filters.Add(performanceCounterApmApiFilterAttribute);
        }
    }
}