using System.Diagnostics;
using System.Net.Http;

namespace Distracey
{
    public class ApmHttpClientRequestDecorator
    {
        public void StartResponseTime(HttpRequestMessage request)
        {
            object responseTimeObject;

            if (request.Properties.TryGetValue(Constants.ResponseTimePropertyKey, out responseTimeObject))
            {
                var stopWatch = (Stopwatch)responseTimeObject;
                if (!stopWatch.IsRunning)
                {
                    stopWatch.Start();
                }
            }
            else
            {
                request.Properties.Add(Constants.ResponseTimePropertyKey, Stopwatch.StartNew());
            }
        }

        public void StopResponseTime(HttpRequestMessage request)
        {
            object responseTimeObject;

            if (request.Properties.TryGetValue(Constants.ResponseTimePropertyKey, out responseTimeObject))
            {
                var stopWatch = (Stopwatch)responseTimeObject;
                if (stopWatch.IsRunning)
                {
                    stopWatch.Stop();
                }
            }
        }

        public void AddApplicationName(HttpRequestMessage request, IApmContext apmContext, string applicationName)
        {
            object applicationNameProperty;

            if (request.Properties.TryGetValue(Constants.ApplicationNamePropertyKey, out applicationNameProperty))
            {
                if (!apmContext.ContainsKey(Constants.ApplicationNamePropertyKey))
                {
                    apmContext[Constants.ApplicationNamePropertyKey] = (string)applicationNameProperty;
                }
            }
            else
            {
                if (!apmContext.ContainsKey(Constants.ApplicationNamePropertyKey))
                {
                    request.Properties[Constants.ApplicationNamePropertyKey] = applicationName;
                }
                else
                {
                    request.Properties[Constants.ApplicationNamePropertyKey] = apmContext[Constants.ApplicationNamePropertyKey];
                }
            }
        }

        public void AddEventName(HttpRequestMessage request, IApmContext apmContext)
        {
            object eventNameProperty;

            if (request.Properties.TryGetValue(Constants.EventNamePropertyKey, out eventNameProperty))
            {
                if (!apmContext.ContainsKey(Constants.EventNamePropertyKey))
                {
                    apmContext[Constants.EventNamePropertyKey] = (string)eventNameProperty;
                }
            }
            else
            {
                request.Properties[Constants.EventNamePropertyKey] = apmContext[Constants.EventNamePropertyKey];
            }
        }

        public void AddMethodIdentifier(HttpRequestMessage request, IApmContext apmContext)
        {
            object methodIdentifierProperty;

            if (request.Properties.TryGetValue(Constants.MethodIdentifierPropertyKey, out methodIdentifierProperty))
            {
                if (!apmContext.ContainsKey(Constants.MethodIdentifierPropertyKey))
                {
                    apmContext[Constants.MethodIdentifierPropertyKey] = (string)methodIdentifierProperty;
                }
            }
            else
            {
                request.Properties[Constants.MethodIdentifierPropertyKey] = apmContext[Constants.MethodIdentifierPropertyKey];
            }
        }

        public void AddClientName(HttpRequestMessage request, IApmContext apmContext)
        {
            object clientNameProperty;
            var clientName = string.Empty;

            if (request.Properties.TryGetValue(Constants.ClientNamePropertyKey, out clientNameProperty))
            {
                clientName = (string)clientNameProperty;
                if (!apmContext.ContainsKey(Constants.ClientNamePropertyKey))
                {
                    apmContext[Constants.ClientNamePropertyKey] = clientName;
                }
            }
            else
            {
                clientName = apmContext[Constants.ClientNamePropertyKey];
                request.Properties[Constants.ClientNamePropertyKey] = clientName;
            }
        }

        public void AddIncomingTraceId(HttpRequestMessage request, IApmContext apmContext)
        {
            object incomingTraceIdProperty;
            var incomingTraceId = string.Empty;

            if (request.Properties.TryGetValue(Constants.IncomingTraceIdPropertyKey, out incomingTraceIdProperty))
            {
                incomingTraceId = (string)incomingTraceIdProperty;
                apmContext[Constants.IncomingTraceIdPropertyKey] = incomingTraceId;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.IncomingTraceIdPropertyKey))
                {
                    incomingTraceId = apmContext[Constants.IncomingTraceIdPropertyKey];
                    request.Properties[Constants.IncomingTraceIdPropertyKey] = incomingTraceId;
                }
                else
                {
                    incomingTraceId = string.Empty;
                    apmContext[Constants.IncomingTraceIdPropertyKey] = incomingTraceId;
                    request.Properties[Constants.IncomingTraceIdPropertyKey] = incomingTraceId;
                }
            }
        }

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
                    flags = apmContext[Constants.FlagsHeaderKey];
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
                    sampled = apmContext[Constants.SampledHeaderKey];
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
                    parentSpanId = apmContext[Constants.ParentSpanIdHeaderKey];
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
                    spanId = apmContext[Constants.SpanIdHeaderKey];
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
                    traceId = apmContext[Constants.TraceIdHeaderKey];
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

        public void AddIncomingFlags(HttpRequestMessage request, IApmContext apmContext)
        {
            object incomingFlagsProperty;
            var incomingFlags = string.Empty;

            if (request.Properties.TryGetValue(Constants.IncomingFlagsPropertyKey, out incomingFlagsProperty))
            {
                incomingFlags = (string)incomingFlagsProperty;
                apmContext[Constants.IncomingSampledPropertyKey] = incomingFlags;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.IncomingSampledPropertyKey))
                {
                    incomingFlags = apmContext[Constants.IncomingSampledPropertyKey];
                    request.Properties[Constants.IncomingSampledPropertyKey] = incomingFlags;
                }
                else
                {
                    incomingFlags = string.Empty;
                    apmContext[Constants.IncomingSampledPropertyKey] = incomingFlags;
                    request.Properties[Constants.IncomingSampledPropertyKey] = incomingFlags;
                }
            }
        }

        public void AddIncomingSampled(HttpRequestMessage request, IApmContext apmContext)
        {
            object incomingSampledProperty;
            var incomingSampled = string.Empty;

            if (request.Properties.TryGetValue(Constants.IncomingSampledPropertyKey, out incomingSampledProperty))
            {
                incomingSampled = (string)incomingSampledProperty;
                apmContext[Constants.IncomingSampledPropertyKey] = incomingSampled;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.IncomingSampledPropertyKey))
                {
                    incomingSampled = apmContext[Constants.IncomingSampledPropertyKey];
                    request.Properties[Constants.IncomingSampledPropertyKey] = incomingSampled;
                }
                else
                {
                    incomingSampled = string.Empty;
                    apmContext[Constants.IncomingSampledPropertyKey] = incomingSampled;
                    request.Properties[Constants.IncomingSampledPropertyKey] = incomingSampled;
                }
            }
        }

        public void AddIncomingParentSpanId(HttpRequestMessage request, IApmContext apmContext)
        {
            object incomingParentSpanIdProperty;
            var incomingParentSpanId = string.Empty;

            if (request.Properties.TryGetValue(Constants.IncomingParentSpanIdPropertyKey, out incomingParentSpanIdProperty))
            {
                incomingParentSpanId = (string)incomingParentSpanIdProperty;
                apmContext[Constants.IncomingParentSpanIdPropertyKey] = incomingParentSpanId;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.IncomingParentSpanIdPropertyKey))
                {
                    incomingParentSpanId = apmContext[Constants.IncomingParentSpanIdPropertyKey];
                    request.Properties[Constants.IncomingParentSpanIdPropertyKey] = incomingParentSpanId;
                }
                else
                {
                    incomingParentSpanId = string.Empty;
                    apmContext[Constants.IncomingParentSpanIdPropertyKey] = incomingParentSpanId;
                    request.Properties[Constants.IncomingParentSpanIdPropertyKey] = incomingParentSpanId;
                }
            }
        }

        public void AddIncomingSpanId(HttpRequestMessage request, IApmContext apmContext)
        {
            object incomingSpanIdProperty;
            string incomingSpanId;
            if (request.Properties.TryGetValue(Constants.IncomingSpanIdPropertyKey, out incomingSpanIdProperty))
            {
                incomingSpanId = (string)incomingSpanIdProperty;
                apmContext[Constants.IncomingSpanIdPropertyKey] = incomingSpanId;
            }
            else
            {
                if (apmContext.ContainsKey(Constants.IncomingSpanIdPropertyKey))
                {
                    incomingSpanId = apmContext[Constants.IncomingSpanIdPropertyKey];
                    request.Properties[Constants.IncomingSpanIdPropertyKey] = incomingSpanId;
                }
                else
                {
                    incomingSpanId = string.Empty;
                    apmContext[Constants.IncomingSpanIdPropertyKey] = incomingSpanId;
                    request.Properties[Constants.IncomingSpanIdPropertyKey] = incomingSpanId;
                }
            }
        }
    }
}
