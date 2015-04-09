using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class ApmContextTests
    {
        private IApmContext _apmContext;

        [SetUp]
        public void Setup()
        {
            _apmContext = new ApmContext();
        }

        [Test]
        public void WhenAddingTrackingInformationWithNoPreviousTracing()
        {
            _apmContext.Add(Constants.ClientNamePropertyKey, "TestClient");

            ApmContext.SetTracing(_apmContext);

            var traceId = (string)_apmContext[Constants.TraceIdHeaderKey];

            var regex = new Regex("TestClient=[^;]+");

            Assert.True(regex.IsMatch(traceId));
        }

        [Test]
        public void WhenAddingTrackingInformationWithNoSpecifiedSpanIdTracing()
        {
            _apmContext.Add(Constants.IncomingTraceIdPropertyKey, "PreviousTestClient=12345");
            _apmContext.Add(Constants.ClientNamePropertyKey, "TestClient");

            ApmContext.SetTracing(_apmContext);

            var spanId = (string)_apmContext[Constants.SpanIdHeaderKey];

            var regex = new Regex("PreviousTestClient=12345;TestClient=[^;]+");

            Assert.True(regex.IsMatch(spanId));
        }

        [Test]
        public void WhenAddingTrackingInformationWithSpecifiedSpanIdTracing()
        {
            _apmContext.Add(Constants.IncomingTraceIdPropertyKey, "PreviousTestClient=12345");
            _apmContext.Add(Constants.IncomingSpanIdPropertyKey, "PreviousTestClient=12345;PreviousTestClientB=12345");
            _apmContext.Add(Constants.ClientNamePropertyKey, "TestClient");

            ApmContext.SetTracing(_apmContext);

            var spanId = (string)_apmContext[Constants.SpanIdHeaderKey];

            var regex = new Regex("PreviousTestClient=12345;PreviousTestClientB=12345;TestClient=[^;]+");

            Assert.True(regex.IsMatch(spanId));
        }
    }
}
