using System;
using System.Diagnostics;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;

namespace Distracey.PerformanceCounter.ApiFilterCounter
{
    public interface IApiFilterCounter : IDisposable
    {
        void Start(IApmContext apmContext, ApmWebApiStartInformation apmWebApiStartInformation);
        void Finish(IApmContext apmContext, ApmWebApiFinishInformation apmWebApiFinishInformation);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}