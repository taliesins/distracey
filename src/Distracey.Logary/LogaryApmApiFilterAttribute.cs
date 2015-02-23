using System;
using Logary;

namespace Distracey.Logary
{
    public class LogaryApmApiFilterAttribute : ApmWebApiFilterAttributeBase
    {
        private static readonly string EventType = typeof(LogaryApmApiFilterAttribute).Name;

        public static string ApplicationName { get; set; }
        public static bool AddResponseHeaders { get; set; }
        public static Logger Log { get; set; }

        public LogaryApmApiFilterAttribute()
            : base(ApplicationName, AddResponseHeaders, Start, Finish)
        {
        }

        public static void Start(ApmWebApiStartInformation apmWebApiStartInformation)
        {
            object apmContextObject;
            if (!apmWebApiStartInformation.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                apmContextObject = new ApmContext();
                apmWebApiStartInformation.Request.Properties.Add(Constants.ApmContextPropertyKey, apmContextObject);
            }

            var apmContext = (IApmContext)apmContextObject;

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

            var message = string.Format("SR - Start - {0} - {1}", apmWebApiStartInformation.MethodIdentifier, apmWebApiStartInformation.TraceId);

            Log.Log(message, LogLevel.Info, apmContext);
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
                var message = string.Format("SS - Finish success - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);
                Log.Log(message, LogLevel.Info, apmContext);
            }
            else
            {
                var message = string.Format("SS - Finish failure - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);
                Log.Log(message, LogLevel.Error, apmContext, null, null, apmWebApiFinishInformation.Exception, null);
            }
        }
    }
}
