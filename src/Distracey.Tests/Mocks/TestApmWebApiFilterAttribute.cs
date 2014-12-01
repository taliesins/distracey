using System;

namespace Distracey.Tests
{
    public class TestApmWebApiFilterAttribute : ApmWebApiFilterAttributeBase
    {
        public TestApmWebApiFilterAttribute(string applicationName, bool addResponseHeaders, Action<ApmWebApiStartInformation> startAction, Action<ApmWebApiFinishInformation> finishAction)
            : base(applicationName, addResponseHeaders, startAction, finishAction)
        {
        }
    }
}
