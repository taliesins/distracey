﻿using System.Web.Http;
using Distracey.MethodHandler;
using Distracey.Web;
using Distracey.Web.HttpClient;

namespace Distracey.PerformanceCounter
{
    public static class PerformanceCounterApmConfigurationExtensions
    {
        public static void AddPerformanceCountersApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            ApmContextHttpMessageExtractor.AddExtractor();

            ApmHttpClientApmContextExtensions.ApmHttpClientDelegatingHandlerFactories.Add(new PerformanceCounterApmHttpClientDelegatingHandlerFactory());
            ApmMethodHandlerApmContextExtensions.ApmMethodHttpFactories.Add(new PerformanceCounterApmMethodHandlerFactory());

            PerformanceCounterApmApiFilterAttribute.ApplicationName = applicationName;
            PerformanceCounterApmApiFilterAttribute.AddResponseHeaders = addResponseHeaders;

            PerformanceCounterApmHttpClientDelegatingHandler.ApplicationName = applicationName;

            PerformanceCounterApmMethodHandler.ApplicationName = applicationName;

            var performanceCounterApmApiFilterAttribute = new PerformanceCounterApmApiFilterAttribute();
            configuration.Filters.Add(performanceCounterApmApiFilterAttribute);
        }
    }
}