using System;
using Distracey.MethodHandler;

namespace Distracey.Tests
{
    public class TestApmMethodHandler : ApmMethodHandlerBase
    {
        public TestApmMethodHandler(IApmContext apmContext, string applicationName, Action<IApmContext, ApmMethodHandlerStartInformation> startAction, Action<IApmContext, ApmMethodHandlerFinishInformation> finishAction)
            : base(apmContext, applicationName, startAction, finishAction)
        {
        }
    }
}
