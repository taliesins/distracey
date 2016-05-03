using System;
using System.Diagnostics;
using Distracey.Agent.Common.MethodHandler;
using Distracey.Common;

namespace Distracey.PerformanceCounter.MethodCounter
{
    public interface IMethodCounter : IDisposable
    {
        void Start(IApmContext apmContext, ApmMethodHandlerStartInformation apmMethodHandlerStartInformation);
        void Finish(IApmContext apmContext, ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}