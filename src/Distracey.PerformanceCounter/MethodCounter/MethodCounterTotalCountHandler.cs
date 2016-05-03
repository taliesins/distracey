using System.Collections.Concurrent;
using System.Diagnostics;
using Distracey.Agent.Common.MethodHandler;
using Distracey.Common;

namespace Distracey.PerformanceCounter.MethodCounter
{
    public class MethodCounterTotalCountHandler : IMethodCounter
    {
        private readonly string _applicationName;
        private readonly string _instanceName;

        private const string TotalCountCounter = "MethodCounterTotalCountCounter";

        private static readonly ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter> Counters = new ConcurrentDictionary<string, System.Diagnostics.PerformanceCounter>();

        public MethodCounterTotalCountHandler(string applicationName, string instanceName)
        {
            _applicationName = applicationName;
            _instanceName = instanceName;
        }

        public void Start(IApmContext apmContext, ApmMethodHandlerStartInformation apmMethodHandlerStartInformation)
        {
            var key = string.Empty;
            
            object counterProperty;

            if (!apmContext.TryGetValue(TotalCountCounter, out counterProperty))
            {
                var categoryName = PerformanceCounterApmEventLogger.GetMethodCategoryName(_applicationName);
                var counterName = GetCounterName(apmMethodHandlerStartInformation.MethodIdentifier);
                var counter = Counters.GetOrAdd(key, s => GetCounter(categoryName, _instanceName, counterName));
                apmContext.Add(TotalCountCounter, counter);
            }
        }

        public void Finish(IApmContext apmContext, ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation)
        {
            object counterProperty;

            if (apmContext.TryGetValue(TotalCountCounter, out counterProperty))
            {
                var counter = (System.Diagnostics.PerformanceCounter)counterProperty;
                counter.Increment();
            }
        }

        private string GetCounterName(string methodIdentifier)
        {
            return string.Format("{0} - Total number of operations", methodIdentifier);
        }

        public CounterCreationData[] GetCreationData(string methodIdentifier)
        {
            var counterCreationDatas = new CounterCreationData[1];
            counterCreationDatas[0] = new CounterCreationData
            {
                CounterType = PerformanceCounterType.NumberOfItems32,
                CounterName = GetCounterName(methodIdentifier),
                CounterHelp = "Total # of operations"
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