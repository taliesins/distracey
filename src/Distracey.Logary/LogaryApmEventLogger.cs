using System;
using System.Threading.Tasks;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using Distracey.MethodHandler;
using Distracey.Web.HttpClient;
using Distracey.Web.WebApi;
using Logary;

namespace Distracey.Logary
{
    public class LogaryApmEventLogger : IEventLogger, IDisposable
    {
        public LogaryApmEventLogger(string applicationName, Logger log)
        {
            ApplicationName = applicationName;
            Log = log;
            this.Subscribe<ApmEvent<ApmMethodHandlerStartInformation>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishInformation>>(OnApmMethodHandlerFinishInformation);
            this.Subscribe<ApmEvent<ApmHttpClientStartInformation>>(OnApmHttpClientStartInformation);
            this.Subscribe<ApmEvent<ApmHttpClientFinishInformation>>(OnApmHttpClientFinishInformation);
            this.Subscribe<ApmEvent<ApmWebApiStartInformation>>(OnApmWebApiStartInformation);
            this.Subscribe<ApmEvent<ApmWebApiFinishInformation>>(OnApmWebApiFinishInformation);
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

        private Task OnApmWebApiStartInformation(Task<ApmEvent<ApmWebApiStartInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiStartInformation = apmEvent.Event;

            var message = string.Format("SR - Start - {0} - {1}", apmWebApiStartInformation.MethodIdentifier, apmWebApiStartInformation.TraceId);

            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        private Task OnApmWebApiFinishInformation(Task<ApmEvent<ApmWebApiFinishInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiFinishInformation = apmEvent.Event;

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

            return Task.FromResult(false);
        }

        public void Dispose()
        {
            this.Unsubscribe<ApmEvent<ApmMethodHandlerStartInformation>>();
            this.Unsubscribe<ApmEvent<ApmMethodHandlerFinishInformation>>();
            this.Unsubscribe<ApmEvent<ApmHttpClientStartInformation>>();
            this.Unsubscribe<ApmEvent<ApmHttpClientFinishInformation>>();
            this.Unsubscribe<ApmEvent<ApmWebApiStartInformation>>();
            this.Unsubscribe<ApmEvent<ApmWebApiFinishInformation>>();
        }
    }
}
