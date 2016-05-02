using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Distracey.Common.EventAggregator;
using Distracey.MethodHandler;
using Distracey.PerformanceCounter.HttpClientCounter;
using Distracey.PerformanceCounter.MethodCounter;
using Distracey.Web.HttpClient;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterEventLogger : IEventLogger
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<IMethodCounter> MethodCounterHandlers = new List<IMethodCounter>()
            {
                new MethodCounterAverageTimeHandler("Default", ApplicationName),
                new MethodCounterLastOperationExecutionTimeHandler("Default", ApplicationName),
                new MethodCounterNumberOfOperationsPerSecondHandler("Default", ApplicationName),
                new MethodCounterTotalCountHandler("Default", ApplicationName)
            };

        public PerformanceCounterEventLogger()
        {
            this.Subscribe<ApmEvent<ApmMethodHandlerStartInformation>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishInformation>>(OnApmMethodHandlerFinishInformation);
            this.Subscribe<ApmEvent<ApmHttpClientStartInformation>>(OnApmHttpClientStartInformation);
            this.Subscribe<ApmEvent<ApmHttpClientFinishInformation>>(OnApmHttpClientFinishInformation);
        }

        public static string ApplicationName { get; set; }

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
            return string.Format("{0}-Method", categoryName);
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
            var apmWebApiStartInformation = apmEvent.Event;

            foreach (var counter in HttpClientCounterHandlers)
            {
                counter.Start(apmContext, apmWebApiStartInformation);
            }

            return Task.FromResult(false);
        }

        private Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmWebApiFinishInformation = apmEvent.Event;

            foreach (var counter in HttpClientCounterHandlers)
            {
                counter.Finish(apmContext, apmWebApiFinishInformation);
            }

            return Task.FromResult(false);
        }

        public static string GetCategoryName(string categoryName)
        {
            return string.Format("{0}-HttpClient", categoryName);
        }
    }
}
