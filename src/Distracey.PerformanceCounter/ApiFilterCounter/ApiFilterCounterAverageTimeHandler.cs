using System.Collections.Concurrent;
using System.Diagnostics;

namespace Distracey.PerformanceCounter.ApiFilterCounter
{
    public class ApiFilterCounterAverageTimeCounter : IApiFilterCounter
    {
        private readonly string _instanceName;

        private const string AverageTimeTakenMsCounter = "ApiFilterCounterAverageTimeCounter";
        private const string AverageTimeTakenMsBaseCounter = "ApiFilterCounterAverageTimeBaseCounter";

        private readonly ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter> Counters = new ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter>();
        private readonly ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter> BaseCounters = new ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter>();

        public ApiFilterCounterAverageTimeCounter(string instanceName)
        {
            _instanceName = instanceName;
        }

        public void Start(ApmWebApiStartInformation apmWebApiStartInformation)
        {
            var key = string.Empty;
            
            object counterProperty;

            if (!apmWebApiStartInformation.Request.Properties.TryGetValue(AverageTimeTakenMsCounter, out counterProperty))
            {
                var categoryName = PerformanceCounterApmApiFilterAttribute.GetCategoryName(apmWebApiStartInformation.ApplicationName);
                var counterName = GetCounterName(apmWebApiStartInformation.MethodIdentifier);

                var counter = Counters.GetOrAdd(key, s => GetCounter(categoryName, _instanceName, counterName));
                apmWebApiStartInformation.Request.Properties.Add(AverageTimeTakenMsCounter, counter);
            }

            object baseCounterProperty;

            if (!apmWebApiStartInformation.Request.Properties.TryGetValue(AverageTimeTakenMsBaseCounter, out baseCounterProperty))
            {
                var categoryName = PerformanceCounterApmApiFilterAttribute.GetCategoryName(apmWebApiStartInformation.ApplicationName);
                var counterName = GetBaseCounterName(apmWebApiStartInformation.MethodIdentifier);
                var baseCounter = BaseCounters.GetOrAdd(key, s => GetBaseCounter(categoryName, _instanceName, counterName));
                apmWebApiStartInformation.Request.Properties.Add(AverageTimeTakenMsBaseCounter, baseCounter);
            }
        }

        public void Finish(ApmWebApiFinishInformation apmWebApiFinishInformation)
        {
            object counterProperty;

            if (apmWebApiFinishInformation.Request.Properties.TryGetValue(AverageTimeTakenMsCounter, out counterProperty))
            {
                var counter = (System.Diagnostics.PerformanceCounter)counterProperty;
                counter.IncrementBy(apmWebApiFinishInformation.ResponseTime);
            }

            object baseCounterProperty;

            if (apmWebApiFinishInformation.Request.Properties.TryGetValue(AverageTimeTakenMsBaseCounter, out baseCounterProperty))
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
            var counter = new System.Diagnostics.PerformanceCounter {
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