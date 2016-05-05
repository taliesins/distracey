using System.Collections.Concurrent;
using System.Diagnostics;
using Distracey.Agent.SystemWeb.HttpClient;
using Distracey.Common;

namespace Distracey.PerformanceCounter.HttpClientCounter
{
    public class HttpClientCounterNumberOfOperationsPerSecondHandler : IHttpClientCounter
    {
        private readonly string _applicationName;
        private readonly string _instanceName;

        private const string LastOperationExecutionTimeMsCounter = "HttpClientCounterNumberOfOperationsPerSecondCounter";

        private static readonly ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter> Counters = new ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter>();

        public HttpClientCounterNumberOfOperationsPerSecondHandler(string applicationName, string instanceName)
        {
            _applicationName = applicationName;
            _instanceName = instanceName;
        }

        public void Start(IApmContext apmContext, ApmHttpClientStartedMessage apmHttpClientStartedMessage)
        {
            var key = string.Empty;
            
            object counterProperty;

            if (!apmContext.TryGetValue(LastOperationExecutionTimeMsCounter, out counterProperty))
            {
                var categoryName = PerformanceCounterApmEventLogger.GetHttpClientCategoryName(_applicationName);
                var counterName = GetCounterName(apmHttpClientStartedMessage.MethodIdentifier);
                var counter = Counters.GetOrAdd(key, s => GetCounter(categoryName, _instanceName, counterName));
                apmContext.Add(LastOperationExecutionTimeMsCounter, counter);
            }
        }

        public void Finish(IApmContext apmContext, ApmHttpClientFinishedMessage apmHttpClientFinishedMessage)
        {
            object counterProperty;

            if (apmContext.TryGetValue(LastOperationExecutionTimeMsCounter, out counterProperty))
            {
                var counter = (System.Diagnostics.PerformanceCounter)counterProperty;
                counter.Increment();
            }
        }

        private string GetCounterName(string methodIdentifier)
        {
            return string.Format("{0} - Number of operations per a second", methodIdentifier);
        }

        public CounterCreationData[] GetCreationData(string methodIdentifier)
        {
            var counterCreationDatas = new CounterCreationData[1];
            counterCreationDatas[0] = new CounterCreationData
            {
                CounterType = PerformanceCounterType.RateOfCountsPerSecond32,
                CounterName = GetCounterName(methodIdentifier),
                CounterHelp = "# of operations / sec"
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