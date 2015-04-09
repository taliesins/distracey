using System;

namespace Distracey.Tests
{
    public class TestApmMethodHandler : ApmMethodHandlerBase
    {
        public TestApmMethodHandler(string applicationName, Action<IApmContext, ApmMethodHandlerStartInformation> startAction, Action<IApmContext, ApmMethodHandlerFinishInformation> finishAction)
            : base(applicationName, startAction, finishAction)
        {
        }
    }
}
