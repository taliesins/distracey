using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Distracey.Common;

namespace Distracey.Agent.SystemWeb
{
    public class ApmHttpRequestMessageParser
    {
        public string GetEventName(HttpRequestMessage request)
        {
            var eventName = string.Empty;
            object eventNameObject;

            if (request.Properties.TryGetValue(Constants.EventNamePropertyKey,
                out eventNameObject))
            {
                eventName = (string)eventNameObject;
            }

            return eventName;
        }

        public string GetMethodIdentifier(HttpRequestMessage request)
        {
            var methodIdentifier = string.Empty;
            object methodIdentifierObject;

            if (request.Properties.TryGetValue(Constants.MethodIdentifierPropertyKey,
                out methodIdentifierObject))
            {
                methodIdentifier = (string)methodIdentifierObject;
            }

            return methodIdentifier;
        }

        public string GetClientName(HttpRequestMessage request)
        {
            var clientName = string.Empty;
            object clientNameObject;

            if (request.Properties.TryGetValue(Constants.ClientNamePropertyKey,
                out clientNameObject))
            {
                clientName = (string)clientNameObject;
            }

            return clientName;
        }

        public string GetTraceId(HttpRequestMessage request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var traceId = string.Empty;
            object traceIdProperty;

            if (request.Properties.TryGetValue(Constants.TraceIdHeaderKey, out traceIdProperty))
            {
                traceId = (string)traceIdProperty;
            }
            else
            {
                IEnumerable<string> traceIds = null;
                if (request.Headers.TryGetValues(Constants.TraceIdHeaderKey, out traceIds))
                {
                    traceId = traceIds.First();
                    request.Properties[Constants.TraceIdHeaderKey] = traceId;
                }
            }

            return traceId;
        }

        public string GetSpanId(HttpRequestMessage request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var spanId = string.Empty;
            object spanIdProperty;

            if (request.Properties.TryGetValue(Constants.SpanIdHeaderKey, out spanIdProperty))
            {
                spanId = (string)spanIdProperty;
            }
            else
            {
                IEnumerable<string> spanIds = null;
                if (request.Headers.TryGetValues(Constants.SpanIdHeaderKey, out spanIds))
                {
                    spanId = spanIds.First();
                    request.Properties[Constants.SpanIdHeaderKey] = spanId;
                }
            }

            return spanId;
        }

        public string GetParentSpanId(HttpRequestMessage request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var parentSpanId = string.Empty;
            object parentSpanIdProperty;

            if (request.Properties.TryGetValue(Constants.ParentSpanIdHeaderKey, out parentSpanIdProperty))
            {
                parentSpanId = (string)parentSpanIdProperty;
            }
            else
            {
                IEnumerable<string> parentSpanIds = null;
                if (request.Headers.TryGetValues(Constants.ParentSpanIdHeaderKey, out parentSpanIds))
                {
                    parentSpanId = parentSpanIds.First();
                    request.Properties[Constants.ParentSpanIdHeaderKey] = parentSpanId;
                }
            }

            return parentSpanId;
        }

        public string GetFlags(HttpRequestMessage request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var flags = string.Empty;
            object flagsProperty;

            if (request.Properties.TryGetValue(Constants.FlagsHeaderKey, out flagsProperty))
            {
                flags = (string)flagsProperty;
            }
            else
            {
                IEnumerable<string> flagItems = null;
                if (request.Headers.TryGetValues(Constants.FlagsHeaderKey, out flagItems))
                {
                    flags = flagItems.First();
                    request.Properties[Constants.FlagsHeaderKey] = flags;
                }
            }

            return flags;
        }

        public string GetSampled(HttpRequestMessage request)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var sampled = string.Empty;
            object sampledProperty;

            if (request.Properties.TryGetValue(Constants.SampledHeaderKey, out sampledProperty))
            {
                sampled = (string)sampledProperty;
            }
            else
            {
                IEnumerable<string> sampledItems = null;
                if (request.Headers.TryGetValues(Constants.SampledHeaderKey, out sampledItems))
                {
                    sampled = sampledItems.First();
                    request.Properties[Constants.SampledHeaderKey] = sampled;
                }
            }

            return sampled;
        }
    }
}
