using System.Collections.Generic;
using Distracey.PerformanceCounter.ApiFilterCounter;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmApiFilterAttribute : ApmWebApiFilterAttributeBase
    {
        public static List<IApiFilterCounter> CounterHandlers = new List<IApiFilterCounter>
        {
            new ApiFilterCounterAverageTimeCounter("Default"),
            new ApiFilterCounterLastOperationExecutionTimeHandler("Default"),
            new ApiFilterCounterNumberOfOperationsPerSecondHandler("Default"),
            new ApiFilterCounterTotalCountHandler("Default")
        }; 

        public PerformanceCounterApmApiFilterAttribute()
            : base(ApplicationName, AddResponseHeaders, Start, Finish)
        {
        }

        public static string ApplicationName { get; set; }
        public static bool AddResponseHeaders { get; set; }

        public static void Start(ApmWebApiStartInformation apmWebApiStartInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Start(apmWebApiStartInformation);
            }
        }

        public static void Finish(ApmWebApiFinishInformation apmWebApiFinishInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Finish(apmWebApiFinishInformation);
            }
        }

        public static string GetCategoryName(string categoryName)
        {
            return string.Format("{0}-ApiFilter", categoryName);
        }
    }
}