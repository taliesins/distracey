using System.Collections.Generic;
using Distracey.PerformanceCounter.ApiFilterCounter;
using Distracey.Web.WebApi;

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

        public static void Start(IApmContext apmContext, ApmWebApiStartInformation apmWebApiStartInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Start(apmContext, apmWebApiStartInformation);
            }
        }

        public static void Finish(IApmContext apmContext, ApmWebApiFinishInformation apmWebApiFinishInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Finish(apmContext, apmWebApiFinishInformation);
            }
        }

        public static string GetCategoryName(string categoryName)
        {
            return string.Format("{0}-ApiFilter", categoryName);
        }
    }
}