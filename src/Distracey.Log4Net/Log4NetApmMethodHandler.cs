using System;
using log4net;
using log4net.Core;

namespace Distracey.Log4Net
{
    public class Log4NetApmMethodHandler : ApmMethodHandlerBase
    {
        private static readonly Type DeclaringType = typeof (Log4NetApmMethodHandler);

        public static string ApplicationName { get; set; }
        public static ILog Log { get; set; }

        public Log4NetApmMethodHandler()
            : base(ApplicationName, Start, Finish)
        {    
        }

        public static void Start(IApmContext apmContext, ApmMethodHandlerStartInformation apmMethodHandlerStartInformation)
        {
            var message = string.Format("CS - Start - {0} - {1}", apmMethodHandlerStartInformation.EventName, apmMethodHandlerStartInformation.TraceId);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);
        }

        public static void Finish(IApmContext apmContext, ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation)
        {
            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmMethodHandlerFinishInformation.EventName, apmMethodHandlerFinishInformation.TraceId, apmMethodHandlerFinishInformation.ResponseTime);
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