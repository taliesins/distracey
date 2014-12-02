using System.Collections.Concurrent;
using System.Diagnostics;

namespace Distracey.PerformanceCounter.HttpClientCounter
{
    public class HttpClientCounterLastOperationExecutionTimeHandler : IHttpClientCounter
    {
        private readonly string _instanceName;
        private const string LastOperationExecutionTimeMsCounter = "HttpClientCounterLastOperationExecutionTimeCounter";

        private readonly ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter> Counters = new ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter>();

        public HttpClientCounterLastOperationExecutionTimeHandler(string instanceName)
        {
            _instanceName = instanceName;
        }

        public void Start(ApmHttpClientStartInformation apmHttpClientStartInformation)
        {
            var key = string.Empty;
            
            object counterProperty;

            if (!apmHttpClientStartInformation.Request.Properties.TryGetValue(LastOperationExecutionTimeMsCounter, out counterProperty))
            {
                var categoryName = PerformanceCounterApmHttpClientDelegatingHandler.GetCategoryName(apmHttpClientStartInformation.ApplicationName);
                var counterName = GetCounterName(apmHttpClientStartInformation.MethodIdentifier);
                var counter = Counters.GetOrAdd(key, s => GetCounter(categoryName, _instanceName, counterName));
                apmHttpClientStartInformation.Request.Properties.Add(LastOperationExecutionTimeMsCounter, counter);
            }
        }

        public void Finish(ApmHttpClientFinishInformation apmHttpClientFinishInformation)
        {
            object counterProperty;

            if (apmHttpClientFinishInformation.Request.Properties.TryGetValue(LastOperationExecutionTimeMsCounter, out counterProperty))
            {
                var counter = (System.Diagnostics.PerformanceCounter)counterProperty;
                counter.RawValue = apmHttpClientFinishInformation.ResponseTime;
            }
        }

        private string GetCounterName(string methodIdentifier)
        {
            return string.Format("{0} - Last Operation Execution Time", methodIdentifier);
        }

        public CounterCreationData[] GetCreationData(string methodIdentifier)
        {
            var counterCreationDatas = new CounterCreationData[1];
            counterCreationDatas[0] = new CounterCreationData
            {
                CounterType = PerformanceCounterType.NumberOfItems32,
                CounterName = GetCounterName(methodIdentifier),
                CounterHelp = "Time in ms to run last request"
            };
            return counterCreationDatas;
        }

        private System.Diagnostics.PerformanceCounter GetCounter(string categoryName, string instanceName, string counterName)
        {
            var counter = new System.Diagnostics.PerformanceCounter
            {
                CategoryName = categoryName,
                CounterName = counterName,
                InstanceName = instanceName,
                ReadOnly = false,
                InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
            };
            counter.RawValue = 0;

            return counter;
        }

        public void Dispose()
        {
            foreach (var counter in Counters)
            {
                counter.Value.RemoveInstance();
                counter.Value.Dispose();
            }
            Counters.Clear();
        }
    }
}