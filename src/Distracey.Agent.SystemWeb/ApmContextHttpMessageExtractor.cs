using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using Distracey.Common;

namespace Distracey.Agent.SystemWeb
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
            if (request == null) return;

            object value;

            if (!apmContext.TryGetValue(Constants.IncomingTraceIdPropertyKey, out value) || value == null)
            {
                var incomingTraceId = ApmHttpRequestMessageParser.GetTraceId(request);

                if (!string.IsNullOrEmpty(incomingTraceId))
                {
                    apmContext[Constants.IncomingTraceIdPropertyKey] = incomingTraceId;
                }
            }

            if (!apmContext.TryGetValue(Constants.IncomingSpanIdPropertyKey, out value) || value == null)
            {
                var incomingSpanId = ApmHttpRequestMessageParser.GetSpanId(request);

                if (!string.IsNullOrEmpty(incomingSpanId))
                {
                    apmContext[Constants.IncomingSpanIdPropertyKey] = incomingSpanId;
                }
            }

            if (!apmContext.TryGetValue(Constants.IncomingParentSpanIdPropertyKey, out value) || value == null)
            {
                var incomingParentSpanId = ApmHttpRequestMessageParser.GetParentSpanId(request);

                if (!string.IsNullOrEmpty(incomingParentSpanId))
                {
                    apmContext[Constants.IncomingParentSpanIdPropertyKey] = incomingParentSpanId;
                }
            }

            if (!apmContext.TryGetValue(Constants.IncomingSampledPropertyKey, out value) || value == null)
            {
                var incomingSampled = ApmHttpRequestMessageParser.GetSampled(request);

                if (!string.IsNullOrEmpty(incomingSampled))
                {
                    apmContext[Constants.IncomingSampledPropertyKey] = incomingSampled;
                }
            }

            if (!apmContext.TryGetValue(Constants.IncomingFlagsPropertyKey, out value) || value == null)
            {
                var incomingFlags = ApmHttpRequestMessageParser.GetFlags(request);

                if (!string.IsNullOrEmpty(incomingFlags))
                {
                    apmContext[Constants.IncomingFlagsPropertyKey] = incomingFlags;
                }
            }
        }

        public void GetContext(IApmContext apmContext, MethodBase method)
        {
            if (HttpContext.Current == null) return;
            var request = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
            SetIncomingTracingForHttpRequestMessage(apmContext, request);
        }
    }
}
