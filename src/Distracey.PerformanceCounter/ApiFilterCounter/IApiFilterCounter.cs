using System;
using System.Diagnostics;

namespace Distracey.PerformanceCounter.ApiFilterCounter
{
    public interface IApiFilterCounter : IDisposable
    {
        void Start(ApmWebApiStartInformation apmWebApiStartInformation);
        void Finish(ApmWebApiFinishInformation apmWebApiFinishInformation);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}