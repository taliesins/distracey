using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class ApmWebApiFilterAttributeBaseTests
    {
        private string _applicationName;
        private bool _addResponseHeaders;
        private Action<ApmWebApiStartInformation> _startAction;
        private Action<ApmWebApiFinishInformation> _finishAction;
        private TestApmWebApiFilterAttribute _testApmWebApiFilterAttribute;
        private HttpRequestMessage _httpRequestMessage;
        
        [SetUp]
        public void Setup()
        {
            _applicationName = "ApplicationName";
            _addResponseHeaders = true;
            _startAction = information => { };
            _finishAction = information => { };
            _testApmWebApiFilterAttribute = new TestApmWebApiFilterAttribute(_applicationName, _addResponseHeaders, _startAction, _finishAction);
            _httpRequestMessage = new HttpRequestMessage();
        }

        [Test]
        public void WhenReceivingTracingInformationForTraceId()
        {
            _httpRequestMessage.Headers.Add(Constants.TraceIdHeaderKey, "TestClient=1234");

            var actionContext = ContextUtil.CreateActionContext();

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.TraceIdHeaderKey));
            var traceId = actionContext.Request.Properties[Constants.TraceIdHeaderKey];
            Assert.AreEqual("TestClient=1234", traceId);
        }
    }
}
