using Distracey.Web.HttpClient;
using Logary;

namespace Distracey.Logary
{
    public class LogaryApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        public static string ApplicationName { get; set; }
        public static Logger Log { get; set; }

        public LogaryApmHttpClientDelegatingHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {    
        }
        
        public static void Start(IApmContext apmContext, ApmHttpClientStartInformation apmWebApiStartInformation)
        {
            var message = string.Format("CS - Start - {0} - {1}", apmWebApiStartInformation.EventName, apmWebApiStartInformation.TraceId);
            Log.Log(message, LogLevel.Info, apmContext);
        }

        public static void Finish(IApmContext apmContext, ApmHttpClientFinishInformation apmWebApiFinishInformation)
        {
            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmWebApiFinishInformation.EventName,
                apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);

            Log.Log(message, LogLevel.Info, apmContext);
        }
    }
}