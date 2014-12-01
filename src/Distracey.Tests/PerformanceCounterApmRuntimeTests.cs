using System.Diagnostics;
using System.Linq;
using Distracey.Examples.Website.Controllers;
using Distracey.PerformanceCounter;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class PerformanceCounterApmRuntimeTests
    {
        [Test]
        public void GetHttpClientsToMonitor()
        {
            var methods = PerformanceCounterApmRuntime.FindAllApmContextUsage(typeof(ValuesController).Assembly).ToList();
            Assert.AreEqual(0, methods.Count());
        }

        [Test]
        public void GetControllersToMonitor()
        {
            var methods = PerformanceCounterApmRuntime.FindAllHttpActionDescriptors(typeof(ValuesController).Assembly).ToList();
            Assert.AreEqual(5, methods.Count());
        }

        [Test]
        [Ignore]
        public void InstallPerformanceCounter()
        {
            var assembly = typeof(ValuesController).Assembly;
            var applicationName = assembly.GetName().Name;
            PerformanceCounterApmRuntime.Install(assembly, applicationName);
        }

        [Test]
        [Ignore]
        public void UninstallPerformanceCounter()
        {
            var assembly = typeof(ValuesController).Assembly;
            var applicationName = assembly.GetName().Name;
            PerformanceCounterApmRuntime.Uninstall(assembly, applicationName);
        }

        [Test]
        [Ignore]
        public void GetPerformanceCounter()
        {
            var counter = new System.Diagnostics.PerformanceCounter
            {
                CategoryName = "Distracey.Examples.Website-ApiFilter",
                InstanceName = "Default",
                CounterName = "ValuesController.Get(Int32 id) - GET - Average seconds taken to execute",
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