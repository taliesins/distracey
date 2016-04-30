using System.Net.Http;
using Distracey.Web;
using NUnit.Framework;

namespace Distracey.Tests
{
    [TestFixture]
    public class ApmRequestParserTests
    {
        [Test]
        public void GetTraceIdWhenOneIsNotSpecified()
        {
            var request = new HttpRequestMessage();
            var apmRequestParser = new ApmHttpRequestMessageParser();
            var traceId = apmRequestParser.GetTraceId(request);

            Assert.IsEmpty(traceId);
        }

        [Test]
        public void GetTraceIdWhenAValidOneIsSpecifiedInHeader()
        {
            var request = new HttpRequestMessage();
            request.Headers.Add(Constants.TraceIdHeaderKey, "clienta=ADSFFF");
            var apmRequestParser = new ApmHttpRequestMessageParser();
            var traceId = apmRequestParser.GetTraceId(request);

            Assert.AreEqual("clienta=ADSFFF", traceId);
        }

        [Test]
        public void GetTraceIdWhenAnInvalidOneIsSpecifiedInHeader()
        {
            var request = new HttpRequestMessage();
            request.Headers.Add(Constants.TraceIdHeaderKey, "jimmm,bobby,susan");
            var apmRequestParser = new ApmHttpRequestMessageParser();
            var traceId = apmRequestParser.GetTraceId(request);

            Assert.AreEqual("jimmm,bobby,susan", traceId);
        }

        [Test]
        public void GetTraceIdWhenAValidOneIsSpecifiedInProperties()
        {
            var request = new HttpRequestMessage();
            request.Properties.Add(Constants.TraceIdHeaderKey, "clienta=ADSFFF");
            var apmRequestParser = new ApmHttpRequestMessageParser();
            var traceId = apmRequestParser.GetTraceId(request);

            Assert.AreEqual("clienta=ADSFFF", traceId);
        }

        [Test]
        public void GetTraceIdWhenAnInvalidOneIsSpecifiedInProperties()
        {
            var request = new HttpRequestMessage();
            request.Properties.Add(Constants.TraceIdHeaderKey, "jimmm,bobby,susan");
            var apmRequestParser = new ApmHttpRequestMessageParser();
            var traceId = apmRequestParser.GetTraceId(request);

            Assert.AreEqual("jimmm,bobby,susan", traceId);
        }
    }
}
