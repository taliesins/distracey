using System;
using System.Threading.Tasks;
using Distracey.Agent.Common.MethodHandler;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Agent.SystemWeb.HttpClient;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using log4net;
using log4net.Core;

namespace Distracey.Log4Net
{
    public class Log4NetApmEventLogger : IApmMethodHandlerLogger, IApmHttpClientLogger, IApmWebApiFilterLogger, IDisposable
    {
        private static readonly Type DeclaringType = typeof(Log4NetApmEventLogger);

        public Log4NetApmEventLogger(string applicationName, ILog log)
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
        public ILog Log { get; set; }

        public Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerStartInformation = apmEvent.Event;

            var message = string.Format("CS - Start - {0} - {1}", apmMethodHandlerStartInformation.EventName, apmMethodHandlerStartInformation.TraceId);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);

            return Task.FromResult(false);
        }

        public Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerFinishInformation = apmEvent.Event;

            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmMethodHandlerFinishInformation.EventName, apmMethodHandlerFinishInformation.TraceId, apmMethodHandlerFinishInformation.ResponseTime);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);

            return Task.FromResult(false);
        }

        public Task OnApmHttpClientStartInformation(Task<ApmEvent<ApmHttpClientStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmHttpClientStartInformation = apmEvent.Event;

            var message = string.Format("CS - Start - {0} - {1}", apmHttpClientStartInformation.EventName, apmHttpClientStartInformation.TraceId);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);

            return Task.FromResult(false);
        }

        public Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmHttpClientFinishInformation = apmEvent.Event;

            var message = string.Format("CR - Finish - {0} - {1} in {2} ms", apmHttpClientFinishInformation.EventName, apmHttpClientFinishInformation.TraceId, apmHttpClientFinishInformation.ResponseTime);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);

            return Task.FromResult(false);
        }

        public Task OnApmWebApiStartInformation(Task<ApmEvent<ApmWebApiStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiStartInformation = apmEvent.Event;

            var message = string.Format("SR - Start - {0} - {1}", apmWebApiStartInformation.MethodIdentifier, apmWebApiStartInformation.TraceId);
            var logger = Log.Logger;
            var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);

            foreach (var property in apmContext)
            {
                logEvent.Properties[property.Key] = property.Value;
            }

            logger.Log(logEvent);

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
                var logger = Log.Logger;
                var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Info, message, null);
                foreach (var property in apmContext)
                {
                    logEvent.Properties[property.Key] = property.Value;
                }
                logger.Log(logEvent);
            }
            else
            {
                var message = string.Format("SS - Finish failure - {0} - {1} in {2} ms", apmWebApiFinishInformation.MethodIdentifier, apmWebApiFinishInformation.TraceId, apmWebApiFinishInformation.ResponseTime);

                var logger = Log.Logger;
                var logEvent = new LoggingEvent(DeclaringType, logger.Repository, logger.Name, Level.Error, message, apmWebApiFinishInformation.Exception);
                foreach (var property in apmContext)
                {
                    logEvent.Properties[property.Key] = property.Value;
                }
                logger.Log(logEvent);
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
