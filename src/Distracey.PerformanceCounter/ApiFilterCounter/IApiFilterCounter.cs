using System;
using System.Diagnostics;

namespace Distracey.PerformanceCounter.ApiFilterCounter
{
    public interface IApiFilterCounter : IDisposable
    {
        void Start(IApmContext apmContext, ApmWebApiStartInformation apmWebApiStartInformation);
        void Finish(IApmContext apmContext, ApmWebApiFinishInformation apmWebApiFinishInformation);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}