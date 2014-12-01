using System.Web.Http;

namespace Distracey.PerformanceCounter
{
    public static class PerformanceCounterApmConfigurationExtensions
    {
        public static void AddPerformanceCountersApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            PerformanceCounterApmApiFilterAttribute.ApplicationName = applicationName;
            PerformanceCounterApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;
            PerformanceCounterApmHttpClientDelegatingHandler.ApplicationName = applicationName;

            var performanceCounterApmApiFilterAttribute = new PerformanceCounterApmApiFilterAttribute();
            configuration.Filters.Add(performanceCounterApmApiFilterAttribute);
        }
    }
}