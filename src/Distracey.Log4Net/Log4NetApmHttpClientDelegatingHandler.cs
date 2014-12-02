using System;
using log4net;
using log4net.Core;

namespace Distracey.Log4Net
{
    public class Log4NetApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        private const string EventType = "Log4NetApmHttpClient";
        private static Type DeclaringType = typeof (Log4NetApmHttpClientDelegatingHandler);

        public static string ApplicationName { get; set; }
        public static ILog Log { get; set; }

        public Log4NetApmHttpClientDelegatingHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {    
        }
        
        public static void Start(ApmHttpClientStartInformation apmWebApiStartInformation)
        {
            object apmContextObject;
            if (!apmWebApiStartInformation.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                apmContextObject = new ApmContext();
                apmWebApiStartInformation.Request.Properties.Add(Constants.ApmContextPropertyKey, apmContextObject);
            }

            var apmContext = (IApmContext)apmContextObject;
            var eventName = apmContext[Constants.EventNamePropertyKey];

            if (!apmContext.ContainsKey(Constants.EventTypePropertyKey))
            {
                apmContext[Constants.EventTypePropertyKey] = EventType;
            }

            if (!apmContext.ContainsKey(Constants.RequestUriPropertyKey))
            {
                apmContext[Constants.RequestUriPropertyKey] = apmWebApiStartInformation.Request.RequestUri.ToString();
            }

            if (!apmContext.ContainsKey(Constants.RequestMethodPropertyKey))
            {
                apmContext[Constants.RequestMethodPropertyKey] = apmWebApiStartInformation.Request.Method.ToString();
            }

            var message = string.Format("Start - {0} - {1}", eventName, apmWebApiStartInformation.TraceId);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);
        }

        public static void Finish(ApmHttpClientFinishInformation apmWebApiFinishInformation)
        {
            object apmContextObject;
            if (!apmWebApiFinishInformation.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                throw new Exception("Add delegating handler filter for Log4NetApmHttpClientDelegatingHandler");
            }

            var apmContext = (IApmContext)apmContextObject;

            var eventName = apmContext[Constants.EventNamePropertyKey];

            if (!apmContext.ContainsKey(Constants.TimeTakeMsPropertyKey))
            {
                apmContext[Constants.TimeTakeMsPropertyKey] = apmWebApiFinishInformation.ResponseTime.ToString();
            }

            if (!apmContext.ContainsKey(Constants.ResponseStatusCodePropertyKey))
            {
                apmContext[Constants.ResponseStatusCodePropertyKey] = apmWebApiFinishInformation.Response.StatusCode.ToString();
            }

            var message = string.Format("Finish - {0} - {1} in {2} ms", eventName, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);
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