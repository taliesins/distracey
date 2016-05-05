using System;
using System.Threading.Tasks;
using Distracey.Agent.Common.MethodHandler;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Agent.SystemWeb.HttpClient;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using Logary;

namespace Distracey.Logary
{
    public class LogaryApmEventLogger : IApmMethodHandlerLogger, IApmHttpClientLogger, IApmWebApiFilterLogger, IDisposable
    {
        public LogaryApmEventLogger(string applicationName, Logger log)
        {
            ApplicationName = applicationName;
            Log = log;
            this.Subscribe<ApmEvent<ApmMethodHandlerStartedMessage>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>(OnApmMethodHandlerFinishInformation);
            this.Subscribe<ApmEvent<ApmHttpClientStartedMessage>>(OnApmHttpClientStartInformation);
            this.Subscribe<ApmEvent<ApmHttpClientFinishedMessage>>(OnApmHttpClientFinishInformation);
            this.Subscribe<ApmEvent<ApmWebApiStartedMessage>>(OnApmWebApiStartInformation);
            this.Subscribe<ApmEvent<ApmWebApiFinishedMessage>>(OnApmWebApiFinishInformation);
        }

        public string ApplicationName { get; set; }
        public static Logger Log { get; set; }

        public Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerStartInformation = apmEvent.Event;

            var message = string.Format("CS - Start - {0} - {1}", apmMethodHandlerStartInformation.EventName, apmMethodHandlerStartInformation.TraceId);
            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        public Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerFinishInformation = apmEvent.Event;

            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmMethodHandlerFinishInformation.EventName, apmMethodHandlerFinishInformation.TraceId, apmMethodHandlerFinishInformation.ResponseTime);

            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        public Task OnApmHttpClientStartInformation(Task<ApmEvent<ApmHttpClientStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiStartInformation = apmEvent.Event;

            var message = string.Format("CS - Start - {0} - {1}", apmWebApiStartInformation.EventName, apmWebApiStartInformation.TraceId);
            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        public Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiFinishInformation = apmEvent.Event;

            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmWebApiFinishInformation.EventName, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);

            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        public Task OnApmWebApiStartInformation(Task<ApmEvent<ApmWebApiStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiStartInformation = apmEvent.Event;

            var message = string.Format("SR - Start - {0} - {1}", apmWebApiStartInformation.MethodIdentifier, apmWebApiStartInformation.TraceId);

            Log.Log(message, LogLevel.Info, apmContext);

            return Task.FromResult(false);
        }

        public Task OnApmWebApiFinishInformation(Task<ApmEvent<ApmWebApiFinishedMessage>> task)
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
            this.Unsubscribe<ApmEvent<ApmMethodHandlerStartedMessage>>();
            this.Unsubscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>();
            this.Unsubscribe<ApmEvent<ApmHttpClientStartedMessage>>();
            this.Unsubscribe<ApmEvent<ApmHttpClientFinishedMessage>>();
            this.Unsubscribe<ApmEvent<ApmWebApiStartedMessage>>();
            this.Unsubscribe<ApmEvent<ApmWebApiFinishedMessage>>();
        }
    }
}
