using System.Diagnostics;
using System.Linq;
using Distracey.PerformanceCounter;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class PerformanceCounterApmRuntimeTests
    {


        [Test]
        [Ignore]
        public void GetPerformanceCounter()
        {
            var counter = new System.Diagnostics.PerformanceCounter
            {
                CategoryName = "Interxion.Services.Order-ApiFilter",
                InstanceName = "Default",
                CounterName = "Order.GetAll(OrderFilter filter) - GET - Average seconds taken to execute",
                ReadOnly = false,
                InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
            };
            counter.RawValue = 0;

            const int value = 100;

            counter.IncrementBy(value);

            System.Diagnostics.PerformanceCounter.CloseSharedResources();

            Assert.AreEqual(value, counter.RawValue);

            counter.RemoveInstance();
            counter.Dispose();
        }
    }
}