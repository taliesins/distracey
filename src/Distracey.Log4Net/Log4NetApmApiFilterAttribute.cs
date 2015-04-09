using System;
using log4net;
using log4net.Core;

namespace Distracey.Log4Net
{
    public class Log4NetApmApiFilterAttribute : ApmWebApiFilterAttributeBase
    {
        private static Type DeclaringType = typeof(Log4NetApmApiFilterAttribute);

        public Log4NetApmApiFilterAttribute()
            : base(ApplicationName, AddResponseHeaders, Start, Finish)
        {
        }

        public static string ApplicationName { get; set; }
        public static bool AddResponseHeaders { get; set; }
        public static ILog Log { get; set; }

        public static void Start(IApmContext apmContext, ApmWebApiStartInformation apmWebApiStartInformation)
        {
            var message = string.Format("SR - Start - {0} - {1}", apmWebApiStartInformation.MethodIdentifier, apmWebApiStartInformation.TraceId);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);
        }

        public static void Finish(IApmContext apmContext, ApmWebApiFinishInformation apmWebApiFinishInformation)
        {
            if (apmWebApiFinishInformation.Exception == null)
            {
                var message = string.Format("SS - Finish success - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);
                var logger = Log.Logger;
                var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);
                foreach (var property in apmContext)
                {
                    logEvent.Properties[property.Key] = property.Value;
                }
                logger.Log(logEvent);
            }
            else
            {
                var message = string.Format("SS - Finish failure - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);

                var logger = Log.Logger;
                var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Error, message, apmWebApiFinishInformation.Exception);
                foreach (var property in apmContext)
                {
                    logEvent.Properties[property.Key] = property.Value;
                }
                logger.Log(logEvent);
            }
        }
    }
}