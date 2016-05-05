using System;
using System.Diagnostics;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;

namespace Distracey.PerformanceCounter.ApiFilterCounter
{
    public interface IApiFilterCounter : IDisposable
    {
        void Start(IApmContext apmContext, ApmWebApiStartedMessage apmWebApiStartedMessage);
        void Finish(IApmContext apmContext, ApmWebApiFinishedMessage apmWebApiFinishedMessage);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}