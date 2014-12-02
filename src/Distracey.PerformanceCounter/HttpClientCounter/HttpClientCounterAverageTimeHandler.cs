using System.Collections.Concurrent;
using System.Diagnostics;

namespace Distracey.PerformanceCounter.HttpClientCounter
{
    public class HttpClientCounterAverageTimeHandler : IHttpClientCounter
    {
        private readonly string _instanceName;
        private const string AverageTimeTakenMsCounter = "HttpClientCounterAverageTimeCounter";
        private const string AverageTimeTakenMsBaseCounter = "HttpClientCounterAverageTimeBaseCounter";

        private readonly ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter> Counters = new ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter>();
        private readonly ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter> BaseCounters = new ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter>();

        public HttpClientCounterAverageTimeHandler(string instanceName)
        {
            _instanceName = instanceName;
        }

        public void Start(ApmHttpClientStartInformation apmHttpClientStartInformation)
        {
            var key = string.Empty;
            
            object counterProperty;

            if (!apmHttpClientStartInformation.Request.Properties.TryGetValue(AverageTimeTakenMsCounter, out counterProperty))
            {
                var categoryName = PerformanceCounterApmHttpClientDelegatingHandler.GetCategoryName(apmHttpClientStartInformation.ApplicationName);
                var counterName = GetCounterName(apmHttpClientStartInformation.MethodIdentifier);

                var counter = Counters.GetOrAdd(key, s => GetCounter(categoryName, _instanceName, counterName));
                apmHttpClientStartInformation.Request.Properties.Add(AverageTimeTakenMsCounter, counter);
            }

            object baseCounterProperty;

            if (!apmHttpClientStartInformation.Request.Properties.TryGetValue(AverageTimeTakenMsBaseCounter, out baseCounterProperty))
            {
                var categoryName = PerformanceCounterApmHttpClientDelegatingHandler.GetCategoryName(apmHttpClientStartInformation.ApplicationName);
                var counterName = GetBaseCounterName(apmHttpClientStartInformation.MethodIdentifier);
                var baseCounter = BaseCounters.GetOrAdd(key, s => GetBaseCounter(categoryName, _instanceName, counterName));
                apmHttpClientStartInformation.Request.Properties.Add(AverageTimeTakenMsBaseCounter, baseCounter);
            }
        }

        public void Finish(ApmHttpClientFinishInformation apmHttpClientFinishInformation)
        {
            object counterProperty;

            if (apmHttpClientFinishInformation.Request.Properties.TryGetValue(AverageTimeTakenMsCounter, out counterProperty))
            {
                var counter = (System.Diagnostics.PerformanceCounter)counterProperty;
                counter.IncrementBy(apmHttpClientFinishInformation.ResponseTime);
            }

            object baseCounterProperty;

            if (apmHttpClientFinishInformation.Request.Properties.TryGetValue(AverageTimeTakenMsBaseCounter, out baseCounterProperty))
            {
                var baseCounter = (System.Diagnostics.PerformanceCounter)baseCounterProperty;
                baseCounter.Increment();
            }
        }

        private string GetCounterName(string methodIdentifier)
        {
            return string.Format("{0} - Average seconds taken to execute", methodIdentifier);
        }

        private string GetBaseCounterName(string methodIdentifier)
        {
            return string.Format("{0} - Average seconds taken to execute (base)", methodIdentifier);
        }

        public CounterCreationData[] GetCreationData(string methodIdentifier)
        {
            var counterCreationDatas = new CounterCreationData[2];
            counterCreationDatas[0] = new CounterCreationData
                                          {
                                              CounterType = PerformanceCounterType.AverageTimer32,
                                              CounterName = GetCounterName(methodIdentifier),
                                              CounterHelp = "Average seconds taken to execute"
                                          };
            counterCreationDatas[1] = new CounterCreationData
                                          {
                                              CounterType = PerformanceCounterType.AverageBase,
                                              CounterName = GetBaseCounterName(methodIdentifier),
                                              CounterHelp = "Average seconds taken to execute"
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

        private System.Diagnostics.PerformanceCounter GetBaseCounter(string categoryName, string instanceName, string counterName)
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

            foreach (var counter in BaseCounters)
            {
                counter.Value.RemoveInstance();
                counter.Value.Dispose();
            }
            BaseCounters.Clear();
        }
    }
}