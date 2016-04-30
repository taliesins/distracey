using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;

namespace Distracey.Web
{
    public class ApmContextHttpMessageExtractor : IApmContextExtractor
    {
        private static readonly ApmHttpRequestMessageParser ApmHttpRequestMessageParser = new ApmHttpRequestMessageParser();

        public static void AddExtractor()
        {
            if (!ApmContext.ApmContextExtractors.Any(x => x is ApmContextHttpMessageExtractor))
            {
                ApmContext.ApmContextExtractors.Add(new ApmContextHttpMessageExtractor());
            }
        }

        private static void SetIncomingTracingForHttpRequestMessage(IApmContext apmContext, HttpRequestMessage request)
        {
            var incomingTraceId = string.Empty;
            var incomingSpanId = string.Empty;
            var incomingParentSpanId = string.Empty;
            var incomingSampled = string.Empty;
            var incomingFlags = string.Empty;

            if (request != null)
            {
                incomingTraceId = ApmHttpRequestMessageParser.GetTraceId(request);
                incomingSpanId = ApmHttpRequestMessageParser.GetSpanId(request);
                incomingParentSpanId = ApmHttpRequestMessageParser.GetParentSpanId(request);
                incomingSampled = ApmHttpRequestMessageParser.GetSampled(request);
                incomingFlags = ApmHttpRequestMessageParser.GetFlags(request);
            }

            apmContext[Constants.IncomingTraceIdPropertyKey] = incomingTraceId;
            apmContext[Constants.IncomingSpanIdPropertyKey] = incomingSpanId;
            apmContext[Constants.IncomingParentSpanIdPropertyKey] = incomingParentSpanId;
            apmContext[Constants.IncomingSampledPropertyKey] = incomingSampled;
            apmContext[Constants.IncomingFlagsPropertyKey] = incomingFlags;
        }

        public void GetContext(IApmContext apmContext, MethodBase method)
        {
            if (HttpContext.Current == null) return;
            var request = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
            SetIncomingTracingForHttpRequestMessage(apmContext, request);
        }
    }
}
