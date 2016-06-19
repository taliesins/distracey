﻿using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;

namespace Distracey.PerformanceCounter
{
    public static class PerformanceCounterApmConfigurationExtensions
    {
        public static void AddPerformanceCountersApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders)
        {
            EventLoggerExtensions.ApmEventLoggers.Add(new PerformanceCounterApmEventLogger(applicationName));

            configuration.AddApmWebApiFilter(addResponseHeaders);
            configuration.Services.Add(typeof(IExceptionLogger), new PerformanceCounterApmExceptionLogger(applicationName));
        }
    }
}