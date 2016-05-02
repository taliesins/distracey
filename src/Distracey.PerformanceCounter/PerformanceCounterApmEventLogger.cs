using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distracey.Common.EventAggregator;
using Distracey.MethodHandler;
using Distracey.PerformanceCounter.ApiFilterCounter;
using Distracey.PerformanceCounter.HttpClientCounter;
using Distracey.PerformanceCounter.MethodCounter;
using Distracey.Web.HttpClient;
using Distracey.Web.WebApi;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmEventLogger : IEventLogger
    {
        public PerformanceCounterApmEventLogger()
        {
            this.Subscribe<ApmEvent<ApmMethodHandlerStartInformation>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishInformation>>(OnApmMethodHandlerFinishInformation);
            this.Subscribe<ApmEvent<ApmHttpClientStartInformation>>(OnApmHttpClientStartInformation);
            this.Subscribe<ApmEvent<ApmHttpClientFinishInformation>>(OnApmHttpClientFinishInformation);
            this.Subscribe<ApmEvent<ApmWebApiStartInformation>>(OnApmWebApiStartInformation);
            this.Subscribe<ApmEvent<ApmWebApiFinishInformation>>(OnApmWebApiFinishInformation);
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

        private Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartInformation>> task)
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

        private Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishInformation>> task)
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

        private Task OnApmHttpClientStartInformation(Task<ApmEvent<ApmHttpClientStartInformation>> task)
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

        private Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishInformation>> task)
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

        private Task OnApmWebApiStartInformation(Task<ApmEvent<ApmWebApiStartInformation>> task)
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

        private Task OnApmWebApiFinishInformation(Task<ApmEvent<ApmWebApiFinishInformation>> task)
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
    }
}
