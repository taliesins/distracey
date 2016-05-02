using System;
using System.Threading.Tasks;
using Distracey.Common.EventAggregator;
using Distracey.MethodHandler;
using Distracey.Web.HttpClient;
using Distracey.Web.WebApi;
using log4net;
using log4net.Core;

namespace Distracey.Log4Net
{
    public class Log4NetEventLogger : IEventLogger
    {
        private static readonly Type DeclaringType = typeof(Log4NetEventLogger);

        public Log4NetEventLogger(string applicationName, ILog log)
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
        public ILog Log { get; set; }

        private Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartInformation>> task)
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

        private Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishInformation>> task)
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

        private Task OnApmHttpClientStartInformation(Task<ApmEvent<ApmHttpClientStartInformation>> task)
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

        private Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishInformation>> task)
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

        private Task OnApmWebApiStartInformation(Task<ApmEvent<ApmWebApiStartInformation>> task)
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

        private Task OnApmWebApiFinishInformation(Task<ApmEvent<ApmWebApiFinishInformation>> task)
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
    }
}
