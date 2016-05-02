using System.Collections.Concurrent;
using System.Diagnostics;
using Distracey.MethodHandler;

namespace Distracey.PerformanceCounter.MethodCounter
{
    public class MethodCounterNumberOfOperationsPerSecondHandler : IMethodCounter
    {
        private readonly string _instanceName;

        private const string LastOperationExecutionTimeMsCounter = "MethodCounterNumberOfOperationsPerSecondCounter";

        private static readonly ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter> Counters = new ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter>();

        public MethodCounterNumberOfOperationsPerSecondHandler(string instanceName)
        {
            _instanceName = instanceName;
        }

        public void Start(IApmContext apmContext, ApmMethodHandlerStartInformation apmMethodHandlerStartInformation)
        {
            var key = string.Empty;
            
            object counterProperty;

            if (!apmContext.TryGetValue(LastOperationExecutionTimeMsCounter, out counterProperty))
            {
                var categoryName = PerformanceCounterEventLogger.GetCategoryName(apmMethodHandlerStartInformation.ApplicationName);
                var counterName = GetCounterName(apmMethodHandlerStartInformation.MethodIdentifier);
                var counter = Counters.GetOrAdd(key, s => GetCounter(categoryName, _instanceName, counterName));
                apmContext.Add(LastOperationExecutionTimeMsCounter, counter);
            }
        }

        public void Finish(IApmContext apmContext, ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation)
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