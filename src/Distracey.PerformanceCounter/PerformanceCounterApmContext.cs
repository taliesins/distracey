using System;
using System.Collections.Generic;

namespace Distracey.PerformanceCounter
{
    public class PerformanceCounterApmContext : Dictionary<string, object>, IApmContext
	{
        public static PerformanceCounterApmContext GetContext<T>(T eventName) where T : struct, IComparable, IFormattable, IConvertible
        {
            var apmContext = new PerformanceCounterApmContext();
            apmContext[Constants.EventNamePropertyKey] = eventName.ToString();
            return apmContext;
        }
	}
}