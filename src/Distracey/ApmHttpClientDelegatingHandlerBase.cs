using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly ApmHttpClientRequestDecorator _apmHttpClientRequestDecorator = new ApmHttpClientRequestDecorator();
        private readonly ApmRequestParser _apmRequestParser = new ApmRequestParser();

        public ApmHttpClientDelegatingHandlerBase(IApmContext apmContext, string applicationName, Action<ApmHttpClientStartInformation> startAction, Action<ApmHttpClientFinishInformation> finishAction)
        {
            _apmContext = apmContext;
            _applicationName = applicationName;
            _startAction = startAction;
            _finishAction = finishAction;
        }

        public void LogStartOfRequest(HttpRequestMessage request, Action<ApmHttpClientStartInformation> startAction)
        {
            var applicationName = _apmRequestParser.GetApplicationName(request);
            var eventName = _apmRequestParser.GetEventName(request);
            var methodIdentifier = _apmRequestParser.GetMethodIdentifier(request);
            var clientName = _apmRequestParser.GetClientName(request);

            var incomingTraceId = _apmRequestParser.GetIncomingTraceId(request);
            var incomingSpanId = _apmRequestParser.GetIncomingSpanId(request);
            var incomingParentSpanId = _apmRequestParser.GetIncomingParentSpanId(request);
            var incomingFlags = _apmRequestParser.GetIncomingFlags(request);
            var incomingSampled = _apmRequestParser.GetIncomingSampled(request);

            var traceId = _apmRequestParser.GetTraceId(request);
            var spanId = _apmRequestParser.GetSpanId(request);
            var parentSpanId = _apmRequestParser.GetParentSpanId(request);
            var flags = _apmRequestParser.GetFlags(request);
            var sampled = _apmRequestParser.GetSampled(request);

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

        public void LogStopOfRequest(HttpRequestMessage request, HttpResponseMessage response, Action<ApmHttpClientFinishInformation> finishAction)
        {
            var applicationName = _apmRequestParser.GetApplicationName(request);
            var eventName = _apmRequestParser.GetEventName(request);
            var methodIdentifier = _apmRequestParser.GetMethodIdentifier(request);
            var responseTime = _apmRequestParser.GetResponseTime(request);
            var clientName = _apmRequestParser.GetClientName(request);

            var incomingTraceId = _apmRequestParser.GetIncomingTraceId(request);
            var incomingSpanId = _apmRequestParser.GetIncomingSpanId(request);
            var incomingParentSpanId = _apmRequestParser.GetIncomingParentSpanId(request);
            var incomingFlags = _apmRequestParser.GetIncomingFlags(request);
            var incomingSampled = _apmRequestParser.GetIncomingSampled(request);

            var traceId = _apmRequestParser.GetTraceId(request);
            var spanId = _apmRequestParser.GetSpanId(request);
            var parentSpanId = _apmRequestParser.GetParentSpanId(request);
            var flags = _apmRequestParser.GetFlags(request);
            var sampled = _apmRequestParser.GetSampled(request);

            var apmHttpClientFinishInformation = new ApmHttpClientFinishInformation
            {
                ApplicationName = applicationName,
                EventName = eventName,
                MethodIdentifier = methodIdentifier,
                Request = request,
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

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Properties[Constants.ApmContextPropertyKey] = _apmContext;

            _apmHttpClientRequestDecorator.AddApplicationName(request, _apmContext, _applicationName);
            _apmHttpClientRequestDecorator.AddEventName(request, _apmContext);
            _apmHttpClientRequestDecorator.AddMethodIdentifier(request, _apmContext);

            _apmHttpClientRequestDecorator.AddClientName(request, _apmContext);

            _apmHttpClientRequestDecorator.AddIncomingTraceId(request, _apmContext);
            _apmHttpClientRequestDecorator.AddIncomingSpanId(request, _apmContext);
            _apmHttpClientRequestDecorator.AddIncomingParentSpanId(request, _apmContext);
            _apmHttpClientRequestDecorator.AddIncomingSampled(request, _apmContext);
            _apmHttpClientRequestDecorator.AddIncomingFlags(request, _apmContext);

            _apmHttpClientRequestDecorator.AddTraceId(request, _apmContext);
            _apmHttpClientRequestDecorator.AddSpanId(request, _apmContext);
            _apmHttpClientRequestDecorator.AddParentSpanId(request, _apmContext);
            _apmHttpClientRequestDecorator.AddSampled(request, _apmContext);
            _apmHttpClientRequestDecorator.AddFlags(request, _apmContext);

            _apmHttpClientRequestDecorator.StartResponseTime(request);
            LogStartOfRequest(request, _startAction);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            _apmHttpClientRequestDecorator.StopResponseTime(request);
            LogStopOfRequest(request, response, _finishAction);

            return response;
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
                    request.Properties[Constants.TraceIdHeaderKey] = traceId;
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
                    request.Properties[Constants.SpanIdHeaderKey] = spanId;
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
                    request.Properties[Constants.ParentSpanIdHeaderKey] = parentSpanId;
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
                    request.Properties[Constants.SampledHeaderKey] = sampled;
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
                    request.Properties[Constants.FlagsHeaderKey] = flags;
                }
            }

            return flags;
        }
    }
}