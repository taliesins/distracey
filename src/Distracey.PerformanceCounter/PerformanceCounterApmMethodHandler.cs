using System.Collections.Generic;
using Distracey.MethodHandler;
using Distracey.PerformanceCounter.MethodCounter;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmMethodHandler : ApmMethodHandlerBase
    {
// ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<IMethodCounter> CounterHandlers = new List<IMethodCounter>()
            {
                new MethodCounterAverageTimeHandler("Default"),
                new MethodCounterLastOperationExecutionTimeHandler("Default"),
                new MethodCounterNumberOfOperationsPerSecondHandler("Default"),
                new MethodCounterTotalCountHandler("Default")
            };

        public PerformanceCounterApmMethodHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {
        }

        public static string ApplicationName { get; set; }

        public static void Start(IApmContext apmContext, ApmMethodHandlerStartInformation apmMethodHandlerStartInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Start(apmContext, apmMethodHandlerStartInformation);
            }
        }

        public static void Finish(IApmContext apmContext, ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Finish(apmContext, apmMethodHandlerFinishInformation);
            }
        }

        public static string GetCategoryName(string categoryName)
        {
            return string.Format("{0}-Method", categoryName);
        }
    }
}