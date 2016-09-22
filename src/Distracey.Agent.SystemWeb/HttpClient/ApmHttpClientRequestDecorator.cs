using System.Net.Http;
using Distracey.Common;

namespace Distracey.Agent.SystemWeb.HttpClient
{
    public class ApmHttpClientRequestDecorator
    {
        public void AddFlags(HttpRequestMessage request, IApmContext apmContext)
        {
            object flagsProperty;
            var flags = string.Empty;

            if (request.Properties.TryGetValue(Constants.FlagsHeaderKey, out flagsProperty))
            {
                flags = (string)flagsProperty;
                apmContext[Constants.FlagsHeaderKey] = flags;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.FlagsHeaderKey))
                {
                    flags = (string)apmContext[Constants.FlagsHeaderKey];
                    request.Properties[Constants.FlagsHeaderKey] = flags;
                }
                else
                {
                    flags = string.Empty;
                    apmContext[Constants.FlagsHeaderKey] = flags;
                    request.Properties[Constants.FlagsHeaderKey] = flags;
                }
            }
        }

        public void AddSampled(HttpRequestMessage request, IApmContext apmContext)
        {
            object sampledProperty;
            var sampled = string.Empty;

            if (request.Properties.TryGetValue(Constants.SampledHeaderKey, out sampledProperty))
            {
                sampled = (string)sampledProperty;
                apmContext[Constants.SampledHeaderKey] = sampled;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.SampledHeaderKey))
                {
                    sampled = (string)apmContext[Constants.SampledHeaderKey];
                    request.Properties[Constants.SampledHeaderKey] = sampled;
                }
                else
                {
                    sampled = string.Empty;
                    apmContext[Constants.SampledHeaderKey] = sampled;
                    request.Properties[Constants.SampledHeaderKey] = sampled;
                }
            }
        }

        public void AddParentSpanId(HttpRequestMessage request, IApmContext apmContext)
        {
            object parentSpanIdProperty;
            var parentSpanId = string.Empty;

            if (request.Properties.TryGetValue(Constants.ParentSpanIdHeaderKey, out parentSpanIdProperty))
            {
                parentSpanId = (string)parentSpanIdProperty;
                apmContext[Constants.ParentSpanIdHeaderKey] = parentSpanId;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.ParentSpanIdHeaderKey))
                {
                    parentSpanId = (string)apmContext[Constants.ParentSpanIdHeaderKey];
                    request.Properties[Constants.ParentSpanIdHeaderKey] = parentSpanId;
                }
                else
                {
                    parentSpanId = string.Empty;
                    apmContext[Constants.ParentSpanIdHeaderKey] = parentSpanId;
                    request.Properties[Constants.ParentSpanIdHeaderKey] = parentSpanId;
                }
            }
        }

        public void AddSpanId(HttpRequestMessage request, IApmContext apmContext)
        {
            object spanIdProperty;
            var spanId = string.Empty;

            if (request.Properties.TryGetValue(Constants.SpanIdHeaderKey, out spanIdProperty))
            {
                spanId = (string)spanIdProperty;
                apmContext[Constants.SpanIdHeaderKey] = spanId;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.SpanIdHeaderKey))
                {
                    spanId = (string)apmContext[Constants.SpanIdHeaderKey];
                    request.Properties[Constants.SpanIdHeaderKey] = spanId;
                }
                else
                {
                    spanId = string.Empty;
                    apmContext[Constants.SpanIdHeaderKey] = spanId;
                    request.Properties[Constants.SpanIdHeaderKey] = spanId;
                }
            }
        }

        public void AddTraceId(HttpRequestMessage request, IApmContext apmContext)
        {
            object traceIdProperty;
            var traceId = string.Empty;

            if (request.Properties.TryGetValue(Constants.TraceIdHeaderKey, out traceIdProperty))
            {
                traceId = (string)traceIdProperty;
                apmContext[Constants.TraceIdHeaderKey] = traceId;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.TraceIdHeaderKey))
                {
                    traceId = (string)apmContext[Constants.TraceIdHeaderKey];
                    request.Properties[Constants.TraceIdHeaderKey] = traceId;
                }
                else
                {
                    traceId = string.Empty;
                    apmContext[Constants.TraceIdHeaderKey] = traceId;
                    request.Properties[Constants.TraceIdHeaderKey] = traceId;
                }
            }
        }
    }
}
