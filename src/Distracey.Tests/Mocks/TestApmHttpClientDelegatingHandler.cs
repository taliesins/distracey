using System;

namespace Distracey.Tests
{
    public class TestApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        public TestApmHttpClientDelegatingHandler(IApmContext apmContext, string applicationName, Action<ApmHttpClientStartInformation> startAction, Action<ApmHttpClientFinishInformation> finishAction) : base(apmContext, applicationName, startAction, finishAction)
        {
        }
    }
}
