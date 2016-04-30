using System;
using Distracey.Web.HttpClient;
using log4net;
using log4net.Core;

namespace Distracey.Log4Net
{
    public class Log4NetApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        private static readonly Type DeclaringType = typeof (Log4NetApmHttpClientDelegatingHandler);

        public static string ApplicationName { get; set; }
        public static ILog Log { get; set; }

        public Log4NetApmHttpClientDelegatingHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {    
        }
        
        public static void Start(IApmContext apmContext, ApmHttpClientStartInformation apmWebApiStartInformation)
        {
            var message = string.Format("CS - Start - {0} - {1}", apmWebApiStartInformation.EventName, apmWebApiStartInformation.TraceId);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);
        }

        public static void Finish(IApmContext apmContext, ApmHttpClientFinishInformation apmWebApiFinishInformation)
        {
            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmWebApiFinishInformation.EventName, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);
         }
    }
}