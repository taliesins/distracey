﻿using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;
using log4net;

namespace Distracey.Log4Net
{
    public static class Log4NetApmConfigurationExtensions
    {
        public static void AddLog4NetApm(this HttpConfiguration configuration, string applicationName, bool addResponseHeaders, ILog log)
        {
            EventLoggerExtensions.ApmEventLoggers.Add(new Log4NetApmEventLogger(applicationName, log));

            configuration.AddApmWebApiFilter(addResponseHeaders);
            configuration.Services.Add(typeof(IExceptionLogger), new Log4NetApmExceptionLogger(log));
        }
    }
}