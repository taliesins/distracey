using System;
using System.Diagnostics;

namespace Distracey.PerformanceCounter.HttpClientCounter
{
    public interface IHttpClientCounter : IDisposable
    {
        void Start(ApmHttpClientStartInformation apmHttpClientStartInformation);
        void Finish(ApmHttpClientFinishInformation apmHttpClientFinishInformation);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}