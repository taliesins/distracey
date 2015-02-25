using System;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class ApmWebApiFilterAttributeBaseTracingTests
    {
        private string _applicationName;
        private bool _addResponseHeaders;
        private Action<ApmWebApiStartInformation> _startAction;
        private Action<ApmWebApiFinishInformation> _finishAction;
        private TestApmWebApiFilterAttribute _testApmWebApiFilterAttribute;

        [SetUp]
        public void Setup()
        {
            _applicationName = "ApplicationName";
            _addResponseHeaders = true;
            _startAction = information => { };
            _finishAction = information => { };
            _testApmWebApiFilterAttribute = new TestApmWebApiFilterAttribute(_applicationName, _addResponseHeaders, _startAction, _finishAction);
        }

        [Test]
        public void WhenReceivingSampledHeader()
        {
            var actionContext = ContextUtil.CreateActionContext();
            actionContext.Request.Headers.Add(Constants.SampledHeaderKey, "Sampled");

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.SampledHeaderKey));
            var sampled = (string)actionContext.Request.Properties[Constants.SampledHeaderKey];
            Assert.IsNotEmpty(sampled);
        }

        [Test]
        public void WhenReceivingNoSampledHeader()
        {
            var actionContext = ContextUtil.CreateActionContext();

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsFalse(actionContext.Request.Properties.ContainsKey(Constants.SampledHeaderKey));
        }

        [Test]
        public void WhenReceivingFlagsHeader()
        {
            var actionContext = ContextUtil.CreateActionContext();
            actionContext.Request.Headers.Add(Constants.FlagsHeaderKey, "Flags");

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.FlagsHeaderKey));
            var flags = (string)actionContext.Request.Properties[Constants.FlagsHeaderKey];
            Assert.IsNotEmpty(flags);
        }

        [Test]
        public void WhenReceivingNoFlagsHeader()
        {
            var actionContext = ContextUtil.CreateActionContext();

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsFalse(actionContext.Request.Properties.ContainsKey(Constants.FlagsHeaderKey));
        }

        [Test]
        public void WhenReceivingNoTracingInformation()
        {
            var actionContext = ContextUtil.CreateActionContext();

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.TraceIdHeaderKey));
            var traceId = (string)actionContext.Request.Properties[Constants.TraceIdHeaderKey];
            Assert.IsNotEmpty(traceId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.SpanIdHeaderKey));
            var spanId = (string)actionContext.Request.Properties[Constants.SpanIdHeaderKey];
            Assert.IsNotEmpty(spanId);

            Assert.AreEqual(traceId, spanId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.ParentSpanIdHeaderKey));
            var parentSpanId = (string)actionContext.Request.Properties[Constants.ParentSpanIdHeaderKey];
            Assert.AreEqual(ApmWebApiRequestDecorator.NoParent, parentSpanId);
        }

        [Test]
        public void WhenReceivingTracingInformationForTraceId()
        {
            var actionContext = ContextUtil.CreateActionContext();
            actionContext.Request.Headers.Add(Constants.TraceIdHeaderKey, "TestClient=1234");

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.TraceIdHeaderKey));
            var traceId = (string)actionContext.Request.Properties[Constants.TraceIdHeaderKey];
            Assert.AreEqual("TestClient=1234", traceId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.SpanIdHeaderKey));
            var spanId = (string)actionContext.Request.Properties[Constants.SpanIdHeaderKey];
            Assert.IsNotEmpty(spanId);

            Assert.AreNotEqual(traceId, spanId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.ParentSpanIdHeaderKey));
            var parentSpanId = (string)actionContext.Request.Properties[Constants.ParentSpanIdHeaderKey];
            Assert.AreEqual(ApmWebApiRequestDecorator.NoParent, parentSpanId);
        }

        [Test]
        public void WhenReceivingTracingInformationForTraceIdAndSpanId()
        {
            var actionContext = ContextUtil.CreateActionContext();
            actionContext.Request.Headers.Add(Constants.TraceIdHeaderKey, "TestClient=1234");
            actionContext.Request.Headers.Add(Constants.SpanIdHeaderKey, "SpecialProcess=4321");
            
            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.TraceIdHeaderKey));
            var traceId = (string)actionContext.Request.Properties[Constants.TraceIdHeaderKey];
            Assert.AreEqual("TestClient=1234", traceId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.SpanIdHeaderKey));
            var spanId = (string)actionContext.Request.Properties[Constants.SpanIdHeaderKey];
            Assert.AreEqual("SpecialProcess=4321", spanId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.ParentSpanIdHeaderKey));
            var parentSpanId = (string)actionContext.Request.Properties[Constants.ParentSpanIdHeaderKey];
            Assert.AreEqual(ApmWebApiRequestDecorator.NoParent, parentSpanId);
        }

        [Test]
        public void WhenReceivingTracingInformationForTraceIdAndSpanIdAndParentId()
        {
            var actionContext = ContextUtil.CreateActionContext();
            actionContext.Request.Headers.Add(Constants.TraceIdHeaderKey, "TestClient=1234");
            actionContext.Request.Headers.Add(Constants.SpanIdHeaderKey, "SpecialProcess=4321");
            actionContext.Request.Headers.Add(Constants.ParentSpanIdHeaderKey, "ParentSpecialProcess=5678");

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.TraceIdHeaderKey));
            var traceId = (string)actionContext.Request.Properties[Constants.TraceIdHeaderKey];
            Assert.AreEqual("TestClient=1234", traceId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.SpanIdHeaderKey));
            var spanId = (string)actionContext.Request.Properties[Constants.SpanIdHeaderKey];
            Assert.AreEqual("SpecialProcess=4321", spanId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.ParentSpanIdHeaderKey));
            var parentSpanId = (string)actionContext.Request.Properties[Constants.ParentSpanIdHeaderKey];
            Assert.AreEqual("ParentSpecialProcess=5678", parentSpanId);
        }

        [Test]
        public void WhenReceivingTracingInformationForParentIdOnly()
        {
            var actionContext = ContextUtil.CreateActionContext();
            actionContext.Request.Headers.Add(Constants.ParentSpanIdHeaderKey, "ParentSpecialProcess=5678");

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.TraceIdHeaderKey));
            var traceId = (string)actionContext.Request.Properties[Constants.TraceIdHeaderKey];
            Assert.IsNotEmpty(traceId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.SpanIdHeaderKey));
            var spanId = (string)actionContext.Request.Properties[Constants.SpanIdHeaderKey];
            Assert.IsNotEmpty(spanId);

            Assert.AreEqual(traceId, spanId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.ParentSpanIdHeaderKey));
            var parentSpanId = (string)actionContext.Request.Properties[Constants.ParentSpanIdHeaderKey];
            Assert.AreEqual(ApmWebApiRequestDecorator.NoParent, parentSpanId);
        }

        [Test]
        public void WhenReceivingTracingInformationForSpanIdOnly()
        {
            var actionContext = ContextUtil.CreateActionContext();
            actionContext.Request.Headers.Add(Constants.SpanIdHeaderKey, "SpecialProcess=4321");

            _testApmWebApiFilterAttribute.OnActionExecuting(actionContext);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.TraceIdHeaderKey));
            var traceId = (string)actionContext.Request.Properties[Constants.TraceIdHeaderKey];
            Assert.IsNotEmpty(traceId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.SpanIdHeaderKey));
            var spanId = (string)actionContext.Request.Properties[Constants.SpanIdHeaderKey];
            Assert.IsNotEmpty(spanId);

            Assert.AreEqual(traceId, spanId);

            Assert.IsTrue(actionContext.Request.Properties.ContainsKey(Constants.ParentSpanIdHeaderKey));
            var parentSpanId = (string)actionContext.Request.Properties[Constants.ParentSpanIdHeaderKey];
            Assert.AreEqual(ApmWebApiRequestDecorator.NoParent, parentSpanId);
        }
    }
}
