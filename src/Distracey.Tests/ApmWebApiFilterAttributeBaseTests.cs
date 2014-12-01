using System;
using System.Net.Http;
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

            _testApmWebApiFilterAttribute.AddTracing(_httpRequestMessage);

            var traceId = _httpRequestMessage.Properties[Constants.TraceIdHeaderKey];
            Assert.AreEqual("TestClient=1234", traceId);
        }
    }
}
