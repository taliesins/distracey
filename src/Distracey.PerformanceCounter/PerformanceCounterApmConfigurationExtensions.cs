﻿using System.Linq;
using System.Web.Http;
using Distracey.Web;
using Distracey.Web.WebApi;

namespace Distracey.PerformanceCounter
{
    public static class PerformanceCounterApmConfigurationExtensions
    {
        public static void AddPerformanceCountersApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            PerformanceCounterEventLogger.ApplicationName = applicationName;
            EventLoggerExtensions.ApmEventLoggers.Add(new PerformanceCounterEventLogger());

            if (configuration.Filters.All(x => x.GetType() != typeof(ApmWebApiFilterAttribute)))
            {
                var apmWebApiFilterAttribute = new ApmWebApiFilterAttribute(addResponseHeaders);
                configuration.Filters.Add(apmWebApiFilterAttribute);
            }
        }
    }
}