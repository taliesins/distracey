using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Agent.SystemWeb.HttpClient;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using Distracey.PerformanceCounter.ApiFilterCounter;
using Distracey.PerformanceCounter.HttpClientCounter;
using Distracey.PerformanceCounter.MethodCounter;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmEventLogger : IApmMethodHandlerLogger, IApmHttpClientLogger, IApmWebApiFilterLogger, IDisposable
    {
        public PerformanceCounterApmEventLogger(string applicationName)
        {
            ApplicationName = applicationName;
            this.Subscribe<ApmEvent<ApmMethodHandlerStartedMessage>>(OnApmMethodHandlerStartedMessage);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>(OnApmMethodHandlerFinishedMessage);
            this.Subscribe<ApmEvent<ApmHttpClientStartedMessage>>(OnApmHttpClientStartedMessage);
            this.Subscribe<ApmEvent<ApmHttpClientFinishedMessage>>(OnApmHttpClientFinishedMessage);
            this.Subscribe<ApmEvent<ApmWebApiStartedMessage>>(OnApmWebApiStartedMessage);
            this.Subscribe<ApmEvent<ApmWebApiFinishedMessage>>(OnApmWebApiFinishedMessage);
        }

        public static string ApplicationName { get; set; }

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<IMethodCounter> MethodCounterHandlers = new List<IMethodCounter>()
            {
                new MethodCounterAverageTimeHandler("Default", ApplicationName),
                new MethodCounterLastOperationExecutionTimeHandler("Default", ApplicationName),
                new MethodCounterNumberOfOperationsPerSecondHandler("Default", ApplicationName),
                new MethodCounterTotalCountHandler("Default", ApplicationName)
            };

        public Task OnApmMethodHandlerStartedMessage(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerStartInformation = apmEvent.Event;

            foreach (var counter in MethodCounterHandlers)
            {
                counter.Start(apmContext, apmMethodHandlerStartInformation);
            }

            return Task.FromResult(false);
        }

        public Task OnApmMethodHandlerFinishedMessage(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerFinishInformation = apmEvent.Event;

            foreach (var counter in MethodCounterHandlers)
            {
                counter.Finish(apmContext, apmMethodHandlerFinishInformation);
            }

            return Task.FromResult(false);
        }

        public static string GetMethodCategoryName(string categoryName)
        {
            return String.Format("{0}-Method", categoryName);
        }


        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<IHttpClientCounter> HttpClientCounterHandlers = new List<IHttpClientCounter>()
            {
                new HttpClientCounterAverageTimeHandler("Default", ApplicationName),
                new HttpClientCounterLastOperationExecutionTimeHandler("Default", ApplicationName),
                new HttpClientCounterNumberOfOperationsPerSecondHandler("Default", ApplicationName),
                new HttpClientCounterTotalCountHandler("Default", ApplicationName)
            };

        public Task OnApmHttpClientStartedMessage(Task<ApmEvent<ApmHttpClientStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmHttpClientStartInformation = apmEvent.Event;

            foreach (var counter in HttpClientCounterHandlers)
            {
                counter.Start(apmContext, apmHttpClientStartInformation);
            }

            return Task.FromResult(false);
        }

        public Task OnApmHttpClientFinishedMessage(Task<ApmEvent<ApmHttpClientFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmHttpClientFinishInformation = apmEvent.Event;

            foreach (var counter in HttpClientCounterHandlers)
            {
                counter.Finish(apmContext, apmHttpClientFinishInformation);
            }

            return Task.FromResult(false);
        }

        public static string GetHttpClientCategoryName(string categoryName)
        {
            return String.Format("{0}-HttpClient", categoryName);
        }

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<IApiFilterCounter> ApiFilterCounterHandlers = new List<IApiFilterCounter>
        {
            new ApiFilterCounterAverageTimeCounter("Default", ApplicationName),
            new ApiFilterCounterLastOperationExecutionTimeHandler("Default", ApplicationName),
            new ApiFilterCounterNumberOfOperationsPerSecondHandler("Default", ApplicationName),
            new ApiFilterCounterTotalCountHandler("Default", ApplicationName)
        };

        public Task OnApmWebApiStartedMessage(Task<ApmEvent<ApmWebApiStartedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiStartInformation = apmEvent.Event;

            foreach (var counter in ApiFilterCounterHandlers)
            {
                counter.Start(apmContext, apmWebApiStartInformation);
            }

            return Task.FromResult(false);
        }

        public Task OnApmWebApiFinishedMessage(Task<ApmEvent<ApmWebApiFinishedMessage>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiFinishInformation = apmEvent.Event;

            foreach (var counter in ApiFilterCounterHandlers)
            {
                counter.Finish(apmContext, apmWebApiFinishInformation);
            }

            return Task.FromResult(false);
        }

        public static string GetApiFilterCategoryName(string categoryName)
        {
            return String.Format("{0}-ApiFilter", categoryName);
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
