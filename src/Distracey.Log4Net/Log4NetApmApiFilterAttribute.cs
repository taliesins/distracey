using System;
using log4net;
using log4net.Core;

namespace Distracey.Log4Net
{
    public class Log4NetApmApiFilterAttribute : ApmWebApiFilterAttributeBase
    {
        private static readonly string EventType = typeof(Log4NetApmApiFilterAttribute).Name;
        private static Type DeclaringType = typeof(Log4NetApmApiFilterAttribute);

        public Log4NetApmApiFilterAttribute()
            : base(ApplicationName, AddResponseHeaders, Start, Finish)
        {
        }

        public static string ApplicationName { get; set; }
        public static bool AddResponseHeaders { get; set; }
        public static ILog Log { get; set; }

        public static void Start(ApmWebApiStartInformation apmWebApiStartInformation)
        {
            object apmContextObject;
            if (!apmWebApiStartInformation.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                apmContextObject = new ApmContext();
                apmWebApiStartInformation.Request.Properties.Add(Constants.ApmContextPropertyKey, apmContextObject);
            }

            var apmContext = (IApmContext) apmContextObject;

            if (!apmContext.ContainsKey(Constants.EventTypePropertyKey))
            {
                apmContext[Constants.EventTypePropertyKey] = EventType;
            }

            if (!apmContext.ContainsKey(Constants.EventNamePropertyKey))
            {
                apmContext[Constants.EventNamePropertyKey] = apmWebApiStartInformation.EventName;
            }

            if (!apmContext.ContainsKey(Constants.MethodIdentifierPropertyKey))
            {
                apmContext[Constants.MethodIdentifierPropertyKey] = apmWebApiStartInformation.EventName;
            }

            if (!apmContext.ContainsKey(Constants.RequestUriPropertyKey))
            {
                apmContext[Constants.RequestUriPropertyKey] = apmWebApiStartInformation.Request.RequestUri.ToString();
            }

            if (!apmContext.ContainsKey(Constants.RequestMethodPropertyKey))
            {
                apmContext[Constants.RequestMethodPropertyKey] = apmWebApiStartInformation.Request.Method.ToString();
            }

            var message = string.Format("Start - {0} - {1}", apmWebApiStartInformation.MethodIdentifier, apmWebApiStartInformation.TraceId);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);
        }

        public static void Finish(ApmWebApiFinishInformation apmWebApiFinishInformation)
        {
            object apmContextObject;
            if (!apmWebApiFinishInformation.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                throw new Exception("Add global filter for ApmWebApiFilterAttributeBase");
            }

            var apmContext = (IApmContext)apmContextObject;
            if (!apmContext.ContainsKey(Constants.TimeTakeMsPropertyKey))
            {
                apmContext[Constants.TimeTakeMsPropertyKey] = apmWebApiFinishInformation.ResponseTime.ToString();
            }

            if (apmWebApiFinishInformation.Exception == null)
            {
                var message = string.Format("Finish success - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);
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
                var message = string.Format("Finish failure - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);

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