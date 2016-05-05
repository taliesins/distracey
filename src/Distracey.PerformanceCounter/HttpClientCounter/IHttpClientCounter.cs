using System;
using System.Diagnostics;
using Distracey.Agent.SystemWeb.HttpClient;
using Distracey.Common;

namespace Distracey.PerformanceCounter.HttpClientCounter
{
    public interface IHttpClientCounter : IDisposable
    {
        void Start(IApmContext apmContext, ApmHttpClientStartedMessage apmHttpClientStartedMessage);
        void Finish(IApmContext apmContext, ApmHttpClientFinishedMessage apmHttpClientFinishedMessage);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}