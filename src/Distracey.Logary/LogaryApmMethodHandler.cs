using Logary;

namespace Distracey.Logary
{
    public class LogaryApmMethodHandler : ApmMethodHandlerBase
    {
        public static string ApplicationName { get; set; }
        public static Logger Log { get; set; }

        public LogaryApmMethodHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {    
        }

        public static void Start(IApmContext apmContext, ApmMethodHandlerStartInformation apmMethodHandlerStartInformation)
        {
            var message = string.Format("CS - Start - {0} - {1}", apmMethodHandlerStartInformation.EventName, apmMethodHandlerStartInformation.TraceId);
            Log.Log(message, LogLevel.Info, apmContext);
        }

        public static void Finish(IApmContext apmContext, ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation)
        {
            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmMethodHandlerFinishInformation.EventName,
                apmMethodHandlerFinishInformation.TraceId, apmMethodHandlerFinishInformation.ResponseTime);

            Log.Log(message, LogLevel.Info, apmContext);
        }
    }
}