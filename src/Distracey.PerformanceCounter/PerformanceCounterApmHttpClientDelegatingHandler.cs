using System.Collections.Generic;
using Distracey.PerformanceCounter.HttpClientCounter;
using Distracey.Web.HttpClient;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
// ReSharper disable once FieldCanBeMadeReadOnly.Global
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

        public static void Start(IApmContext apmContext, ApmHttpClientStartInformation apmWebApiStartInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Start(apmContext, apmWebApiStartInformation);
            }
        }

        public static void Finish(IApmContext apmContext, ApmHttpClientFinishInformation apmWebApiFinishInformation)
        {
            foreach (var counter in CounterHandlers)
            {
                counter.Finish(apmContext, apmWebApiFinishInformation);
            }
        }

        public static string GetCategoryName(string categoryName)
        {
            return string.Format("{0}-HttpClient", categoryName);
        }
    }
}