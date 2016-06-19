using Distracey.Common;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class ApmContextTests
    {
        private const string NoParent = "0";
        private const int ShortGuidLength = 22;

        [Test]
        public void WhenAddingTrackingInformationWithNoPreviousTracing()
        {
            var apmContext = ApmContext.GetContext(clientName: "TestClient");
            var traceId = (string)apmContext[Constants.TraceIdHeaderKey];
            var parentSpanId = (string)apmContext[Constants.ParentSpanIdHeaderKey];
            var spanId = (string)apmContext[Constants.SpanIdHeaderKey];

            Assert.True(traceId.Length == ShortGuidLength);
            Assert.True(parentSpanId == NoParent);
            Assert.True(spanId.Length == ShortGuidLength);
        }

        [Test]
        public void WhenAddingTrackingInformationWithNoSpecifiedSpanIdAndNoParentSpanIdTracing()
        {
            var apmContext = ApmContext.GetContext(clientName: "TestClient", traceId: "PreviousTestClient=12345");
            var traceId = (string)apmContext[Constants.TraceIdHeaderKey];
            var parentSpanId = (string)apmContext[Constants.ParentSpanIdHeaderKey];
            var spanId = (string)apmContext[Constants.SpanIdHeaderKey];

            Assert.True(traceId == "PreviousTestClient=12345");
            Assert.True(parentSpanId == NoParent);
            Assert.True(spanId.Length == ShortGuidLength);
        }

        [Test]
        public void WhenAddingTrackingInformationWithSpecifiedSpanIdAndNoParentSpanIdTracing()
        {
            var apmContext = ApmContext.GetContext(traceId: "PreviousTestClient=12345", spanId: "PreviousTestClient=12345;PreviousTestClientB=12345", clientName: "TestClient");
            var traceId = (string)apmContext[Constants.TraceIdHeaderKey];
            var parentSpanId = (string)apmContext[Constants.ParentSpanIdHeaderKey];
            var spanId = (string)apmContext[Constants.SpanIdHeaderKey];

            Assert.True(traceId == "PreviousTestClient=12345");
            Assert.True(parentSpanId == NoParent);
            Assert.True(spanId == "PreviousTestClient=12345;PreviousTestClientB=12345");
        }

        [Test]
        public void WhenAddingTrackingInformationWithNoSpecifiedSpanIdAndParentSpanIdTracing()
        {
            var apmContext = ApmContext.GetContext(clientName: "TestClient", traceId: "PreviousTestClient=12345", parentSpanId: "Parent=12345");
            var traceId = (string)apmContext[Constants.TraceIdHeaderKey];
            var parentSpanId = (string)apmContext[Constants.ParentSpanIdHeaderKey];
            var spanId = (string)apmContext[Constants.SpanIdHeaderKey];

            Assert.True(traceId == "PreviousTestClient=12345");
            Assert.True(parentSpanId == "Parent=12345");
            Assert.True(spanId.Length == ShortGuidLength);
        }

        [Test]
        public void WhenAddingTrackingInformationWithSpecifiedSpanIdAndParentSpanIdTracing()
        {
            var apmContext = ApmContext.GetContext(traceId: "PreviousTestClient=12345", parentSpanId: "Parent=12345", spanId: "PreviousTestClient=12345;PreviousTestClientB=12345", clientName: "TestClient");
            var traceId = (string)apmContext[Constants.TraceIdHeaderKey];
            var parentSpanId = (string)apmContext[Constants.ParentSpanIdHeaderKey];
            var spanId = (string)apmContext[Constants.SpanIdHeaderKey];

            Assert.True(traceId == "PreviousTestClient=12345");
            Assert.True(parentSpanId == "Parent=12345");
            Assert.True(spanId == "PreviousTestClient=12345;PreviousTestClientB=12345");
        }
    }
}
