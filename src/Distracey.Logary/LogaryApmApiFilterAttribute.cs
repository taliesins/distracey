using Distracey.Web.WebApi;
using Logary;

namespace Distracey.Logary
{
    public class LogaryApmApiFilterAttribute : ApmWebApiFilterAttributeBase
    {
        public static string ApplicationName { get; set; }
        public static bool AddResponseHeaders { get; set; }
        public static Logger Log { get; set; }

        public LogaryApmApiFilterAttribute()
            : base(ApplicationName, AddResponseHeaders, Start, Finish)
        {
        }

        public static void Start(IApmContext apmContext, ApmWebApiStartInformation apmWebApiStartInformation)
        {
            var message = string.Format("SR - Start - {0} - {1}", apmWebApiStartInformation.MethodIdentifier, apmWebApiStartInformation.TraceId);

            Log.Log(message, LogLevel.Info, apmContext);
        }

        public static void Finish(IApmContext apmContext, ApmWebApiFinishInformation apmWebApiFinishInformation)
        {
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
