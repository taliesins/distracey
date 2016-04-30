using System;
using Distracey.Web.HttpClient;

namespace Distracey.Tests
{
    public class TestApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        public TestApmHttpClientDelegatingHandler(IApmContext apmContext, string applicationName, Action<IApmContext, ApmHttpClientStartInformation> startAction, Action<IApmContext, ApmHttpClientFinishInformation> finishAction)
            : base(apmContext, applicationName, startAction, finishAction)
        {
        }
    }
}
