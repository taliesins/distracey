using System;
using Logary;

namespace Distracey.Logary
{
    public class LogaryApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        private static readonly string EventType = typeof(LogaryApmHttpClientDelegatingHandler).Name;

        public static string ApplicationName { get; set; }
        public static Logger Log { get; set; }

        public LogaryApmHttpClientDelegatingHandler(IApmContext apmContext)
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
            Log.Log(message, LogLevel.Info, apmContext);
        }

        public static void Finish(ApmHttpClientFinishInformation apmWebApiFinishInformation)
        {
            object apmContextObject;
            if (
                !apmWebApiFinishInformation.Response.RequestMessage.Properties.TryGetValue(
                    Constants.ApmContextPropertyKey, out apmContextObject))
            {
                throw new Exception("Add delegating handler filter for Log4NetApmHttpClientDelegatingHandler");
            }

            var apmContext = (IApmContext) apmContextObject;

            var eventName = apmContext[Constants.EventNamePropertyKey];

            if (!apmContext.ContainsKey(Constants.TimeTakeMsPropertyKey))
            {
                apmContext[Constants.TimeTakeMsPropertyKey] = apmWebApiFinishInformation.ResponseTime.ToString();
            }

            if (!apmContext.ContainsKey(Constants.ResponseStatusCodePropertyKey))
            {
                apmContext[Constants.ResponseStatusCodePropertyKey] =
                    apmWebApiFinishInformation.Response.StatusCode.ToString();
            }

            var message = string.Format("Finish - {0} - {1} in {2} ms", eventName,
                apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);

            Log.Log(message, LogLevel.Info, apmContext);
        }
    }
}