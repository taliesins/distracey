using System.Collections.Generic;
using Distracey.PerformanceCounter.HttpClientCounter;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        public static List<IHttpClientCounter> CounterHandlers = new List<IHttpClientCounter>()
            {
                new HttpClientCounterAverageTimeHandler("Default"),
                new HttpClientCounterLastOperationExecutionTimeHandler("Default"),
                new HttpClientCounterNumberOfOperationsPerSecondHandler("Default"),
                new HttpClientCounterTotalCountHandler("Default")
            };
        
        public PerformanceCounterApmHttpClientDelegatingHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {
        }

        public static string ApplicationName { get; set; }

        public static void Start(ApmHttpClientStartInformation apmWebApiStartInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Start(apmWebApiStartInformation);
            }
        }

        public static void Finish(ApmHttpClientFinishInformation apmWebApiFinishInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Finish(apmWebApiFinishInformation);
            }
        }

        public static string GetCategoryName(string categoryName)
        {
            return string.Format("{0}-HttpClient", categoryName);
        }
    }
}