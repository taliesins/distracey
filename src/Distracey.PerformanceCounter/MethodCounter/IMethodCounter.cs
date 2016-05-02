using System;
using System.Diagnostics;
using Distracey.Common;
using Distracey.MethodHandler;

namespace Distracey.PerformanceCounter.MethodCounter
{
    public interface IMethodCounter : IDisposable
    {
        void Start(IApmContext apmContext, ApmMethodHandlerStartInformation apmMethodHandlerStartInformation);
        void Finish(IApmContext apmContext, ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}