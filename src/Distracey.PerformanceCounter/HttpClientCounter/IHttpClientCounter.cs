using System;
using System.Diagnostics;
using Distracey.Common;
using Distracey.Web.HttpClient;

namespace Distracey.PerformanceCounter.HttpClientCounter
{
    public interface IHttpClientCounter : IDisposable
    {
        void Start(IApmContext apmContext, ApmHttpClientStartInformation apmHttpClientStartInformation);
        void Finish(IApmContext apmContext, ApmHttpClientFinishInformation apmHttpClientFinishInformation);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}