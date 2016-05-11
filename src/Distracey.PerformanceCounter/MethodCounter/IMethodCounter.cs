using System;
using System.Diagnostics;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Common;

namespace Distracey.PerformanceCounter.MethodCounter
{
    public interface IMethodCounter : IDisposable
    {
        void Start(IApmContext apmContext, ApmMethodHandlerStartedMessage apmMethodHandlerStartedMessage);
        void Finish(IApmContext apmContext, ApmMethodHandlerFinishedMessage apmMethodHandlerFinishedMessage);
        CounterCreationData[] GetCreationData(string methodIdentifier);
    }
}