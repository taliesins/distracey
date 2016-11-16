using System;
using System.Threading.Tasks;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Agent.SystemWeb.HttpClient;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using Logary;
using Logary.CSharp;

namespace Distracey.Logary
{
    public class LogaryApmEventLogger : IApmMethodHandlerLogger, IApmHttpClientLogger, IApmWebApiFilterLogger, IDisposable
    {
        public LogaryApmEventLogger(string applicationName, Logger log)
        {
            ApplicationName = applicationName;
            Log = log;
            this.Subscribe<ApmEvent<ApmMethodHandlerStartedMessage>>(OnApmMethodHandlerStartedMessage);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>(OnApmMethodHandlerFinishedMessage);
            this.Subscribe<ApmEvent<ApmHttpClientStartedMessage>>(OnApmHttpClientStartedMessage);
            this.Subscribe<ApmEvent<ApmHttpClientFinishedMessage>>(OnApmHttpClientFinishedMessage);
            this.Subscribe<ApmEvent<ApmWebApiStartedMessage>>(OnApmWebApiStartedMessage);
            this.Subscribe<ApmEvent<ApmWebApiFinishedMessage>>(OnApmWebApiFinishedMessage);
        }

        public string ApplicationName { get; set; }
        public static Logger Log { get; set; }

        public Task OnApmMethodHandlerStartedMessage(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerStartInformation = apmEvent.Event;

            var message = string.Format("CS - StartSession - {0} - {1}", apmMethodHandlerStartInformation.EventName, apmMethodHandlerStartInformation.TraceId);

            return Log.LogEvent(LogLevel.Info, message, new
            {
                apmMethodHandlerStartInformation.EventName,
                apmMethodHandlerStartInformation.ClientName,
                apmMethodHandlerStartInformation.Duration,
                apmMethodHandlerStartInformation.Flags,
                apmMethodHandlerStartInformation.MethodIdentifier,
                apmMethodHandlerStartInformation.Offset,
                apmMethodHandlerStartInformation.ParentSpanId,
                apmMethodHandlerStartInformation.Sampled,
                apmMethodHandlerStartInformation.SpanId,
                apmMethodHandlerStartInformation.StartTime,
                apmMethodHandlerStartInformation.TraceId,
                apmContext
            });
        }

        public Task OnApmMethodHandlerFinishedMessage(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerFinishInformation = apmEvent.Event;

            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmMethodHandlerFinishInformation.EventName, apmMethodHandlerFinishInformation.TraceId, apmMethodHandlerFinishInformation.ResponseTime);

            return Log.LogEvent(LogLevel.Info, message, new
            {
                apmMethodHandlerFinishInformation.EventName,
                apmMethodHandlerFinishInformation.ClientName,
                apmMethodHandlerFinishInformation.Duration,
                apmMethodHandlerFinishInformation.Flags,
                apmMethodHandlerFinishInformation.MethodIdentifier,
                apmMethodHandlerFinishInformation.Offset,
                apmMethodHandlerFinishInformation.ParentSpanId,
                apmMethodHandlerFinishInformation.Sampled,
                apmMethodHandlerFinishInformation.SpanId,
                apmMethodHandlerFinishInformation.StartTime,
                apmMethodHandlerFinishInformation.TraceId,
                apmMethodHandlerFinishInformation.Exception,
                apmMethodHandlerFinishInformation.ResponseTime,
                apmContext
            });
        }

        public Task OnApmHttpClientStartedMessage(Task<ApmEvent<ApmHttpClientStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiStartInformation = apmEvent.Event;

            var message = string.Format("CS - StartSession - {0} - {1}", apmWebApiStartInformation.EventName, apmWebApiStartInformation.TraceId);
            return Log.LogEvent(LogLevel.Info, message, new
            {
                apmWebApiStartInformation.EventName,
                apmWebApiStartInformation.ClientName,
                apmWebApiStartInformation.Duration,
                apmWebApiStartInformation.Flags,
                apmWebApiStartInformation.MethodIdentifier,
                apmWebApiStartInformation.Offset,
                apmWebApiStartInformation.ParentSpanId,
                apmWebApiStartInformation.Request,
                apmWebApiStartInformation.Sampled,
                apmWebApiStartInformation.SpanId,
                apmWebApiStartInformation.StartTime,
                apmWebApiStartInformation.TraceId,     
                apmContext
            });
        }

        public Task OnApmHttpClientFinishedMessage(Task<ApmEvent<ApmHttpClientFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiFinishInformation = apmEvent.Event;

            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmWebApiFinishInformation.EventName, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.Duration.Milliseconds);

            return Log.LogEvent(LogLevel.Info, message, new
            {
                apmWebApiFinishInformation.EventName,
                apmWebApiFinishInformation.ClientName,
                apmWebApiFinishInformation.Duration,
                apmWebApiFinishInformation.Flags,
                apmWebApiFinishInformation.MethodIdentifier,
                apmWebApiFinishInformation.Offset,
                apmWebApiFinishInformation.ParentSpanId,
                apmWebApiFinishInformation.Request,
                apmWebApiFinishInformation.Response,
                apmWebApiFinishInformation.Sampled,
                apmWebApiFinishInformation.SpanId,
                apmWebApiFinishInformation.StartTime,
                apmWebApiFinishInformation.TraceId,
                apmContext
            });
        }

        public Task OnApmWebApiStartedMessage(Task<ApmEvent<ApmWebApiStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiStartInformation = apmEvent.Event;

            var message = string.Format("SR - StartSession - {0} - {1}", apmWebApiStartInformation.MethodIdentifier, apmWebApiStartInformation.TraceId);

            return Log.LogEvent(LogLevel.Info, message, new
            {
                apmWebApiStartInformation.EventName,
                apmWebApiStartInformation.Duration,
                apmWebApiStartInformation.Flags,
                apmWebApiStartInformation.MethodIdentifier,
                apmWebApiStartInformation.Offset,
                apmWebApiStartInformation.ParentSpanId,
                apmWebApiStartInformation.Request,
                apmWebApiStartInformation.Sampled,
                apmWebApiStartInformation.SpanId,
                apmWebApiStartInformation.StartTime,
                apmWebApiStartInformation.TraceId,
                apmContext
            });
        }

        public Task OnApmWebApiFinishedMessage(Task<ApmEvent<ApmWebApiFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiFinishInformation = apmEvent.Event;

            if (apmWebApiFinishInformation.Exception == null)
            {
                var message = string.Format("SS - Finish success - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.Duration.Milliseconds);
                return Log.LogEvent(LogLevel.Info, message, new
                {
                    apmWebApiFinishInformation.EventName,
                    apmWebApiFinishInformation.Duration,
                    apmWebApiFinishInformation.Flags,
                    apmWebApiFinishInformation.MethodIdentifier,
                    apmWebApiFinishInformation.Offset,
                    apmWebApiFinishInformation.ParentSpanId,
                    apmWebApiFinishInformation.Request,
                    apmWebApiFinishInformation.Response,
                    apmWebApiFinishInformation.Sampled,
                    apmWebApiFinishInformation.SpanId,
                    apmWebApiFinishInformation.StartTime,
                    apmWebApiFinishInformation.TraceId,
                    apmContext
                });
            }
            else
            {
                var message = string.Format("SS - Finish failure - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.Duration.Milliseconds);

                return Log.LogEvent(LogLevel.Error, message, new
                {
                    apmWebApiFinishInformation.EventName,
                    apmWebApiFinishInformation.Exception,
                    apmWebApiFinishInformation.Duration,
                    apmWebApiFinishInformation.Flags,
                    apmWebApiFinishInformation.MethodIdentifier,
                    apmWebApiFinishInformation.Offset,
                    apmWebApiFinishInformation.ParentSpanId,
                    apmWebApiFinishInformation.Request,
                    apmWebApiFinishInformation.Response,
                    apmWebApiFinishInformation.Sampled,
                    apmWebApiFinishInformation.SpanId,
                    apmWebApiFinishInformation.StartTime,
                    apmWebApiFinishInformation.TraceId,
                    apmContext
                });
            }
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
