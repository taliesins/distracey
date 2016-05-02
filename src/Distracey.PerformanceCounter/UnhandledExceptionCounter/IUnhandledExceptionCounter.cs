using System.Diagnostics;
using Distracey.Common;

namespace Distracey.PerformanceCounter.UnhandledExceptionCounter
{
    public interface IUnhandledExceptionCounter
    {
        void Error(IApmContext apmContext);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}
