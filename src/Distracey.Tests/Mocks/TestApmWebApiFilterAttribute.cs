using System;
using Distracey.Web.WebApi;

namespace Distracey.Tests
{
    public class TestApmWebApiFilterAttribute : ApmWebApiFilterAttributeBase
    {
        public TestApmWebApiFilterAttribute(string applicationName, bool addResponseHeaders, Action<IApmContext, ApmWebApiStartInformation> startAction, Action<IApmContext, ApmWebApiFinishInformation> finishAction)
            : base(applicationName, addResponseHeaders, startAction, finishAction)
        {
        }
    }
}
