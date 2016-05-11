using System.Diagnostics;

namespace Distracey.Common
{
    public static class IApmContextExtensions
    {
        public static string GetEventName(this IApmContext apmContext)
        {
            var eventName = string.Empty;
            object eventNameObject;

            if (apmContext.TryGetValue(Constants.EventNamePropertyKey,
                out eventNameObject))
            {
                eventName = (string)eventNameObject;
            }

            return eventName;
        }

        public static string GetMethodIdentifier(this IApmContext apmContext)
        {
            var methodIdentifier = string.Empty;
            object methodIdentifierObject;

            if (apmContext.TryGetValue(Constants.MethodIdentifierPropertyKey,
                out methodIdentifierObject))
            {
                methodIdentifier = (string)methodIdentifierObject;
            }

            return methodIdentifier;
        }

        public static long GetResponseTime(this IApmContext apmContext)
        {
            object responseTimeObject;

            var responseTime = 0L;
            if (apmContext.TryGetValue(Constants.ResponseTimePropertyKey,
                out responseTimeObject))
            {
                responseTime = ((Stopwatch)responseTimeObject).ElapsedMilliseconds;
            }

            return responseTime;
        }

        public static string GetClientName(this IApmContext apmContext)
        {
            var clientName = string.Empty;
            object clientNameObject;

            if (apmContext.TryGetValue(Constants.ClientNamePropertyKey,
                out clientNameObject))
            {
                clientName = (string)clientNameObject;
            }

            return clientName;
        }

        public static string GetIncomingTraceId(this IApmContext apmContext)
        {
            var incomingTraceId = string.Empty;
            object incomingTraceIdObject;

            if (apmContext.TryGetValue(Constants.IncomingTraceIdPropertyKey,
                out incomingTraceIdObject))
            {
                incomingTraceId = (string)incomingTraceIdObject;
            }

            return incomingTraceId;
        }

        public static string GetIncomingSpanId(this IApmContext apmContext)
        {
            var incomingSpanId = string.Empty;
            object incomingSpanIdObject;

            if (apmContext.TryGetValue(Constants.IncomingSpanIdPropertyKey,
                out incomingSpanIdObject))
            {
                incomingSpanId = (string)incomingSpanIdObject;
            }

            return incomingSpanId;
        }

        public static string GetIncomingParentSpanId(this IApmContext apmContext)
        {
            var incomingParentSpanId = string.Empty;
            object incomingParentSpanIdObject;

            if (apmContext.TryGetValue(Constants.IncomingParentSpanIdPropertyKey,
                out incomingParentSpanIdObject))
            {
                incomingParentSpanId = (string)incomingParentSpanIdObject;
            }

            return incomingParentSpanId;
        }

        public static string GetIncomingFlags(this IApmContext apmContext)
        {
            var incomingFlags = string.Empty;
            object incomingFlagsObject;

            if (apmContext.TryGetValue(Constants.IncomingFlagsPropertyKey,
                out incomingFlagsObject))
            {
                incomingFlags = (string)incomingFlagsObject;
            }

            return incomingFlags;
        }

        public static string GetIncomingSampled(this IApmContext apmContext)
        {
            var incomingSampled = string.Empty;
            object incomingSampledObject;

            if (apmContext.TryGetValue(Constants.IncomingSampledPropertyKey,
                out incomingSampledObject))
            {
                incomingSampled = (string)incomingSampledObject;
            }

            return incomingSampled;
        }

        public static string GetTraceId(this IApmContext apmContext)
        {
            var traceId = string.Empty;
            object traceIdObject;

            if (apmContext.TryGetValue(Constants.TraceIdHeaderKey,
                out traceIdObject))
            {
                traceId = (string)traceIdObject;
            }

            return traceId;
        }

        public static string GetSpanId(this IApmContext apmContext)
        {
            var spanId = string.Empty;
            object spanIdObject;

            if (apmContext.TryGetValue(Constants.SpanIdHeaderKey,
                out spanIdObject))
            {
                spanId = (string)spanIdObject;
            }

            return spanId;
        }

        public static string GetParentSpanId(this IApmContext apmContext)
        {
            var parentSpanId = string.Empty;
            object parentSpanIdObject;

            if (apmContext.TryGetValue(Constants.ParentSpanIdHeaderKey,
                out parentSpanIdObject))
            {
                parentSpanId = (string)parentSpanIdObject;
            }

            return parentSpanId;
        }

        public static string GetFlags(this IApmContext apmContext)
        {
            var flags = string.Empty;
            object flagsObject;

            if (apmContext.TryGetValue(Constants.FlagsHeaderKey,
                out flagsObject))
            {
                flags = (string)flagsObject;
            }

            return flags;
        }

        public static string GetSampled(this IApmContext apmContext)
        {
            var sampled = string.Empty;
            object sampledObject;

            if (apmContext.TryGetValue(Constants.FlagsHeaderKey,
                out sampledObject))
            {
                sampled = (string)sampledObject;
            }

            return sampled;
        }
    }
}
