using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Distracey
{
    /// <summary>
    /// Used to track requests and responses made by an http client.
    /// </summary>
    public abstract class ApmHttpClientDelegatingHandlerBase : DelegatingHandler
    {
        private readonly IApmContext _apmContext;
        private readonly string _applicationName;
        private readonly Action<ApmHttpClientStartInformation> _startAction;
        private readonly Action<ApmHttpClientFinishInformation> _finishAction;

        public ApmHttpClientDelegatingHandlerBase(IApmContext apmContext, string applicationName, Action<ApmHttpClientStartInformation> startAction, Action<ApmHttpClientFinishInformation> finishAction)
        {
            _apmContext = apmContext;
            _applicationName = applicationName;
            _startAction = startAction;
            _finishAction = finishAction;
        }

        public void AddApplicationName(HttpRequestMessage request, IApmContext apmContext, string applicationName)
        {
            object applicationNameProperty;

            if (request.Properties.TryGetValue(Constants.ApplicationNamePropertyKey, out applicationNameProperty))
            {
                if (!apmContext.ContainsKey(Constants.ApplicationNamePropertyKey))
                {
                    apmContext[Constants.ApplicationNamePropertyKey] = (string) applicationNameProperty;
                }
            }
            else
            {
                if (!apmContext.ContainsKey(Constants.ApplicationNamePropertyKey))
                {
                    request.Properties.Add(Constants.ApplicationNamePropertyKey, applicationName);
                }
                else
                {
                    request.Properties.Add(Constants.ApplicationNamePropertyKey, apmContext[Constants.ApplicationNamePropertyKey]);
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
                request.Properties.Add(Constants.EventNamePropertyKey, apmContext[Constants.EventNamePropertyKey]);
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
                request.Properties.Add(Constants.MethodIdentifierPropertyKey, apmContext[Constants.MethodIdentifierPropertyKey]);
            }
        }

        public void AddTracing(HttpRequestMessage request, IApmContext apmContext)
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
                request.Properties.Add(Constants.ClientNamePropertyKey, clientName);
            }

            object incomingTraceIdProperty;
            var incomingTraceId=string.Empty;

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

            object incomingSpanIdProperty;
            var incomingSpanId = string.Empty;

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

        public void StartResponseTime(HttpRequestMessage request)
        {
            object responseTimeObject;

            if (request.Properties.TryGetValue(Constants.ResponseTimePropertyKey, out responseTimeObject))
            {
                var stopWatch = (Stopwatch) responseTimeObject;
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

        public void StopResponseTime(HttpResponseMessage response)
        {
            object responseTimeObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.ResponseTimePropertyKey, out responseTimeObject))
            {
                var stopWatch = (Stopwatch)responseTimeObject;
                if (stopWatch.IsRunning)
                {
                    stopWatch.Stop();
                }
            }
        }

        public void LogStartOfRequest(HttpRequestMessage request, Action<ApmHttpClientStartInformation> startAction)
        {
            var applicationName = string.Empty;
            object applicationNameObject;

            if (request.Properties.TryGetValue(Constants.ApplicationNamePropertyKey,
                out applicationNameObject))
            {
                applicationName = (string)applicationNameObject;
            }

            var eventName = string.Empty;
            object eventNameObject;

            if (request.Properties.TryGetValue(Constants.EventNamePropertyKey,
                out eventNameObject))
            {
                eventName = (string)eventNameObject;
            }

            var methodIdentifier = string.Empty;
            object methodIdentifierObject;

            if (request.Properties.TryGetValue(Constants.MethodIdentifierPropertyKey,
                out methodIdentifierObject))
            {
                methodIdentifier = (string)methodIdentifierObject;
            }

            var clientName = string.Empty;
            object clientNameObject;

            if (request.Properties.TryGetValue(Constants.ClientNamePropertyKey,
                out clientNameObject))
            {
                clientName = (string)clientNameObject;
            }

            var incomingTraceId = string.Empty;
            object incomingTraceIdObject;

            if (request.Properties.TryGetValue(Constants.IncomingTraceIdPropertyKey,
                out incomingTraceIdObject))
            {
                incomingTraceId = (string) incomingTraceIdObject;
            }

            var incomingSpanId = string.Empty;
            object incomingSpanIdObject;

            if (request.Properties.TryGetValue(Constants.IncomingSpanIdPropertyKey,
                out incomingSpanIdObject))
            {
                incomingSpanId = (string)incomingSpanIdObject;
            }

            var incomingParentSpanId = string.Empty;
            object incomingParentSpanIdObject;

            if (request.Properties.TryGetValue(Constants.IncomingParentSpanIdPropertyKey,
                out incomingParentSpanIdObject))
            {
                incomingParentSpanId = (string)incomingParentSpanIdObject;
            }

            var incomingFlags = string.Empty;
            object incomingFlagsObject;

            if (request.Properties.TryGetValue(Constants.IncomingFlagsPropertyKey,
                out incomingFlagsObject))
            {
                incomingFlags = (string)incomingFlagsObject;
            }

            var incomingSampled = string.Empty;
            object incomingSampledObject;

            if (request.Properties.TryGetValue(Constants.IncomingSampledPropertyKey,
                out incomingSampledObject))
            {
                incomingSampled = (string)incomingSampledObject;
            }

            var traceId = string.Empty;
            object traceIdObject;

            if (request.Properties.TryGetValue(Constants.TraceIdHeaderKey,
                out traceIdObject))
            {
                traceId = (string)traceIdObject;
            }

            var spanId = string.Empty;
            object spanIdObject;

            if (request.Properties.TryGetValue(Constants.SpanIdHeaderKey,
                out spanIdObject))
            {
                spanId = (string)spanIdObject;
            }

            var parentSpanId = string.Empty;
            object parentSpanIdObject;

            if (request.Properties.TryGetValue(Constants.ParentSpanIdHeaderKey,
                out parentSpanIdObject))
            {
                parentSpanId = (string)parentSpanIdObject;
            }

            var flags = string.Empty;
            object flagsObject;

            if (request.Properties.TryGetValue(Constants.FlagsHeaderKey,
                out flagsObject))
            {
                flags = (string)flagsObject;
            }

            var sampled = string.Empty;
            object sampledObject;

            if (request.Properties.TryGetValue(Constants.IncomingSampledPropertyKey,
                out sampledObject))
            {
                sampled = (string)sampledObject;
            }

            var apmHttpClientStartInformation = new ApmHttpClientStartInformation
            {
                ApplicationName = applicationName,
                EventName = eventName,
                MethodIdentifier = methodIdentifier,
                Request = request,
                ClientName = clientName,
                IncomingTraceId = incomingTraceId,
                IncomingSpanId = incomingSpanId,
                IncomingParentSpanId = incomingParentSpanId,
                IncomingSampled = incomingSampled,
                IncomingFlags = incomingFlags,
                TraceId = traceId,
                SpanId = spanId,
                ParentSpanId = parentSpanId,
                Sampled = sampled,
                Flags = flags
            };

            startAction(apmHttpClientStartInformation);
        }

        public void LogStopOfRequest(HttpResponseMessage response, Action<ApmHttpClientFinishInformation> finishAction)
        {
            var applicationName = string.Empty;
            object applicationNameObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.ApplicationNamePropertyKey,
                out applicationNameObject))
            {
                applicationName = (string)applicationNameObject;
            }

            var eventName = string.Empty;
            object eventNameObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.EventNamePropertyKey,
                out eventNameObject))
            {
                eventName = (string)eventNameObject;
            }

            var methodIdentifier = string.Empty;
            object methodIdentifierObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.MethodIdentifierPropertyKey,
                out methodIdentifierObject))
            {
                methodIdentifier = (string)methodIdentifierObject;
            }

            object responseTimeObject;

            var responseTime = 0L;
            if (response.RequestMessage.Properties.TryGetValue(Constants.ResponseTimePropertyKey,
                out responseTimeObject))
            {
                responseTime = ((Stopwatch)responseTimeObject).ElapsedMilliseconds;
            }

            var clientName = string.Empty;
            object clientNameObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.ClientNamePropertyKey,
                out clientNameObject))
            {
                clientName = (string)clientNameObject;
            }

            var incomingTraceId = string.Empty;
            object incomingTraceIdObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.IncomingTraceIdPropertyKey,
                out incomingTraceIdObject))
            {
                incomingTraceId = (string)incomingTraceIdObject;
            }

            var incomingSpanId = string.Empty;
            object incomingSpanIdObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.IncomingSpanIdPropertyKey,
                out incomingSpanIdObject))
            {
                incomingSpanId = (string)incomingSpanIdObject;
            }

            var incomingParentSpanId = string.Empty;
            object incomingParentSpanIdObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.IncomingParentSpanIdPropertyKey,
                out incomingParentSpanIdObject))
            {
                incomingParentSpanId = (string)incomingParentSpanIdObject;
            }

            var incomingFlags = string.Empty;
            object incomingFlagsObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.IncomingFlagsPropertyKey,
                out incomingFlagsObject))
            {
                incomingFlags = (string)incomingFlagsObject;
            }

            var incomingSampled = string.Empty;
            object incomingSampledObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.IncomingSampledPropertyKey,
                out incomingSampledObject))
            {
                incomingSampled = (string)incomingSampledObject;
            }

            var traceId = string.Empty;
            object traceIdObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.TraceIdHeaderKey,
                out traceIdObject))
            {
                traceId = (string)traceIdObject;
            }

            var spanId = string.Empty;
            object spanIdObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.SpanIdHeaderKey,
                out spanIdObject))
            {
                spanId = (string)spanIdObject;
            }

            var parentSpanId = string.Empty;
            object parentSpanIdObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.ParentSpanIdHeaderKey,
                out parentSpanIdObject))
            {
                parentSpanId = (string)parentSpanIdObject;
            }

            var flags = string.Empty;
            object flagsObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.FlagsHeaderKey,
                out flagsObject))
            {
                flags = (string)flagsObject;
            }

            var sampled = string.Empty;
            object sampledObject;

            if (response.RequestMessage.Properties.TryGetValue(Constants.IncomingSampledPropertyKey,
                out sampledObject))
            {
                sampled = (string)sampledObject;
            }

            var apmHttpClientFinishInformation = new ApmHttpClientFinishInformation
            {
                ApplicationName = applicationName,
                EventName = eventName,
                MethodIdentifier = methodIdentifier,
                Response = response,
                ResponseTime = responseTime,
                ClientName = clientName,
                IncomingTraceId = incomingTraceId,
                IncomingSpanId = incomingSpanId,
                IncomingParentSpanId = incomingParentSpanId,
                IncomingSampled = incomingSampled,
                IncomingFlags = incomingFlags,
                TraceId = traceId,
                SpanId = spanId,
                ParentSpanId = parentSpanId,
                Sampled = sampled,
                Flags = flags
            };

            finishAction(apmHttpClientFinishInformation);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Properties.Add(Constants.ApmContextPropertyKey, _apmContext);

            AddApplicationName(request, _apmContext, _applicationName);
            AddEventName(request, _apmContext);
            AddMethodIdentifier(request, _apmContext);
            AddTracing(request, _apmContext);
            LogStartOfRequest(request, _startAction);
            StartResponseTime(request);

            return base.SendAsync(request, cancellationToken)
                .Then(response =>
                {
                    StopResponseTime(response);
                    LogStopOfRequest(response, _finishAction);
                    return response;

                }, cancellationToken);
        }

        public static string GetMethodIdentifier(MethodBase methodInfo)
        {
            var param = methodInfo.GetParameters()
                             .Select(parameter => string.Format("{0} {1}", parameter.ParameterType.Name, parameter.Name))
                             .ToArray();

            var arguments = string.Join(", ", param);

            return string.Format("{0}.{1}({2})", methodInfo.DeclaringType.FullName, methodInfo.Name, arguments);
        }

        public static string GetEventName(MethodBase methodInfo)
        {
            return string.Format("{0}.{1}", methodInfo.DeclaringType.Name, methodInfo.Name);
        }

        public static string GetClientName()
        {
            var clientName = ConfigurationManager.AppSettings[Constants.ClientNamePropertyKey];
            return clientName;
        }

        public static string GetTraceId(HttpRequestMessage request)
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
                    request.Properties.Add(Constants.TraceIdHeaderKey, traceId);
                }
            }

            return traceId;
        }

        public static string GetSpanId(HttpRequestMessage request)
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
                    request.Properties.Add(Constants.SpanIdHeaderKey, spanId);
                }
            }

            return spanId;
        }

        public static string GetParentSpanId(HttpRequestMessage request)
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
                    request.Properties.Add(Constants.ParentSpanIdHeaderKey, parentSpanId);
                }
            }

            return parentSpanId;
        }

        public static string GetSampled(HttpRequestMessage request)
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
                    request.Properties.Add(Constants.SampledHeaderKey, sampled);
                }
            }

            return sampled;
        }

        public static string GetFlags(HttpRequestMessage request)
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
                    request.Properties.Add(Constants.FlagsHeaderKey, flags);
                }
            }

            return flags;
        }
    }
}