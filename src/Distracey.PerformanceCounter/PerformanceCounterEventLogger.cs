using System.Collections.Generic;
using System.Threading.Tasks;
using Distracey.Common.EventAggregator;
using Distracey.MethodHandler;
using Distracey.PerformanceCounter.MethodCounter;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterEventLogger : IEventLogger
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<IMethodCounter> CounterHandlers = new List<IMethodCounter>()
            {
                new MethodCounterAverageTimeHandler("Default"),
                new MethodCounterLastOperationExecutionTimeHandler("Default"),
                new MethodCounterNumberOfOperationsPerSecondHandler("Default"),
                new MethodCounterTotalCountHandler("Default")
            };

        public PerformanceCounterEventLogger(string applicationName)
        {
            ApplicationName = applicationName;
            this.Subscribe<ApmEvent<ApmMethodHandlerStartInformation>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishInformation>>(OnApmMethodHandlerFinishInformation);
        }

        public string ApplicationName { get; set; }

        private Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartInformation>> task)
        {
            var apmEvent = task.Result;
            var apmContext = apmEvent.ApmContext;
            var apmMethodHandlerStartInformation = apmEvent.Event;

            foreach (var counter in CounterHandlers)
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

            foreach (var counter in CounterHandlers)
            {
                counter.Finish(apmContext, apmMethodHandlerFinishInformation);
            }

            return Task.FromResult(false);
        }

        public static string GetCategoryName(string categoryName)
        {
            return string.Format("{0}-Method", categoryName);
        }
    }
}
