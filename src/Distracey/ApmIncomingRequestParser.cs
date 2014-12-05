using System.Diagnostics;
using System.Net.Http;

namespace Distracey
{
    public class ApmIncomingRequestParser
    {
        public string GetApplicationName(HttpRequestMessage request)
        {
            var applicationName = string.Empty;
            object applicationNameObject;

            if (request.Properties.TryGetValue(Constants.ApplicationNamePropertyKey,
                out applicationNameObject))
            {
                applicationName = (string)applicationNameObject;
            }

            return applicationName;
        }

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

        public long GetResponseTime(HttpRequestMessage request)
        {
            object responseTimeObject;

            var responseTime = 0L;
            if (request.Properties.TryGetValue(Constants.ResponseTimePropertyKey,
                out responseTimeObject))
            {
                responseTime = ((Stopwatch)responseTimeObject).ElapsedMilliseconds;
            }

            return responseTime;
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

        public string GetIncomingTraceId(HttpRequestMessage request)
        {
            var incomingTraceId = string.Empty;
            object incomingTraceIdObject;

            if (request.Properties.TryGetValue(Constants.IncomingTraceIdPropertyKey,
                out incomingTraceIdObject))
            {
                incomingTraceId = (string)incomingTraceIdObject;
            }

            return incomingTraceId;
        }

        public string GetIncomingSpanId(HttpRequestMessage request)
        {
            var incomingSpanId = string.Empty;
            object incomingSpanIdObject;

            if (request.Properties.TryGetValue(Constants.IncomingSpanIdPropertyKey,
                out incomingSpanIdObject))
            {
                incomingSpanId = (string)incomingSpanIdObject;
            }

            return incomingSpanId;
        }

        public string GetIncomingParentSpanId(HttpRequestMessage request)
        {
            var incomingParentSpanId = string.Empty;
            object incomingParentSpanIdObject;

            if (request.Properties.TryGetValue(Constants.IncomingParentSpanIdPropertyKey,
                out incomingParentSpanIdObject))
            {
                incomingParentSpanId = (string)incomingParentSpanIdObject;
            }

            return incomingParentSpanId;
        }

        public string GetIncomingFlags(HttpRequestMessage request)
        {
            var incomingFlags = string.Empty;
            object incomingFlagsObject;

            if (request.Properties.TryGetValue(Constants.IncomingFlagsPropertyKey,
                out incomingFlagsObject))
            {
                incomingFlags = (string)incomingFlagsObject;
            }

            return incomingFlags;
        }

        public string GetIncomingSampled(HttpRequestMessage request)
        {
            var incomingSampled = string.Empty;
            object incomingSampledObject;

            if (request.Properties.TryGetValue(Constants.IncomingSampledPropertyKey,
                out incomingSampledObject))
            {
                incomingSampled = (string)incomingSampledObject;
            }

            return incomingSampled;
        }

        public string GetTraceId(HttpRequestMessage request)
        {
            var traceId = string.Empty;
            object traceIdObject;

            if (request.Properties.TryGetValue(Constants.TraceIdHeaderKey,
                out traceIdObject))
            {
                traceId = (string)traceIdObject;
            }

            return traceId;
        }

        public string GetSpanId(HttpRequestMessage request)
        {
            var spanId = string.Empty;
            object spanIdObject;

            if (request.Properties.TryGetValue(Constants.SpanIdHeaderKey,
                out spanIdObject))
            {
                spanId = (string)spanIdObject;
            }

            return spanId;
        }

        public string GetParentSpanId(HttpRequestMessage request)
        {
            var parentSpanId = string.Empty;
            object parentSpanIdObject;

            if (request.Properties.TryGetValue(Constants.ParentSpanIdHeaderKey,
                out parentSpanIdObject))
            {
                parentSpanId = (string)parentSpanIdObject;
            }

            return parentSpanId;
        }

        public string GetFlags(HttpRequestMessage request)
        {
            var flags = string.Empty;
            object flagsObject;

            if (request.Properties.TryGetValue(Constants.FlagsHeaderKey,
                out flagsObject))
            {
                flags = (string)flagsObject;
            }

            return flags;
        }

        public string GetSampled(HttpRequestMessage request)
        {
            var sampled = string.Empty;
            object sampledObject;

            if (request.Properties.TryGetValue(Constants.IncomingSampledPropertyKey,
                out sampledObject))
            {
                sampled = (string)sampledObject;
            }

            return sampled;
        }
    }
}
