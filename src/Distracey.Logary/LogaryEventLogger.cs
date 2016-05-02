using System.Threading.Tasks;
using Distracey.Common.EventAggregator;
using Distracey.MethodHandler;
using Distracey.Web.HttpClient;
using Logary;

namespace Distracey.Logary
{
    public class LogaryEventLogger : IEventLogger
    {
        public LogaryEventLogger(string applicationName, Logger log)
        {
            ApplicationName = applicationName;
            Log = log;
            this.Subscribe<ApmEvent<ApmMethodHandlerStartInformation>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishInformation>>(OnApmMethodHandlerFinishInformation);
            this.Subscribe<ApmEvent<ApmHttpClientStartInformation>>(OnApmHttpClientStartInformation);
            this.Subscribe<ApmEvent<ApmHttpClientFinishInformation>>(OnApmHttpClientFinishInformation);
        }

        public string ApplicationName { get; set; }
        public static Logger Log { get; set; }

        private Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerStartInformation = apmEvent.Event;

            var message = string.Format("CS - Start - {0} - {1}", apmMethodHandlerStartInformation.EventName, apmMethodHandlerStartInformation.TraceId);
            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        private Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerFinishInformation = apmEvent.Event;

            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmMethodHandlerFinishInformation.EventName, apmMethodHandlerFinishInformation.TraceId, apmMethodHandlerFinishInformation.ResponseTime);

            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        private Task OnApmHttpClientStartInformation(Task<ApmEvent<ApmHttpClientStartInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiStartInformation = apmEvent.Event;

            var message = string.Format("CS - Start - {0} - {1}", apmWebApiStartInformation.EventName, apmWebApiStartInformation.TraceId);
            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        private Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiFinishInformation = apmEvent.Event;

            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmWebApiFinishInformation.EventName, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);

            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }
    }
}
