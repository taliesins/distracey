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
        private readonly ApmIncomingRequestDecorator _apmIncomingRequestDecorator = new ApmIncomingRequestDecorator();
        private readonly ApmIncomingRequestParser _apmIncomingRequestParser = new ApmIncomingRequestParser();

        public ApmHttpClientDelegatingHandlerBase(IApmContext apmContext, string applicationName, Action<ApmHttpClientStartInformation> startAction, Action<ApmHttpClientFinishInformation> finishAction)
        {
            _apmContext = apmContext;
            _applicationName = applicationName;
            _startAction = startAction;
            _finishAction = finishAction;
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



        public void LogStopOfRequest(HttpRequestMessage request, HttpResponseMessage response, Action<ApmHttpClientFinishInformation> finishAction)
        {
            var applicationName = _apmIncomingRequestParser.GetApplicationName(request);
            var eventName = _apmIncomingRequestParser.GetEventName(request);
            var methodIdentifier = _apmIncomingRequestParser.GetMethodIdentifier(request);
            var responseTime = _apmIncomingRequestParser.GetResponseTime(request);
            var clientName = _apmIncomingRequestParser.GetClientName(request);
            var incomingTraceId = _apmIncomingRequestParser.GetIncomingTraceId(request);
            var incomingSpanId = _apmIncomingRequestParser.GetIncomingSpanId(request);
            var incomingParentSpanId = _apmIncomingRequestParser.GetIncomingParentSpanId(request);
            var incomingFlags = _apmIncomingRequestParser.GetIncomingFlags(request);
            var incomingSampled = _apmIncomingRequestParser.GetIncomingSampled(request);

            var traceId = _apmIncomingRequestParser.GetTraceId(request);
            var spanId = _apmIncomingRequestParser.GetSpanId(request);
            var parentSpanId = _apmIncomingRequestParser.GetParentSpanId(request);
            var flags = _apmIncomingRequestParser.GetFlags(request);
            var sampled = _apmIncomingRequestParser.GetSampled(request);

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

            _apmIncomingRequestDecorator.AddApplicationName(request, _apmContext, _applicationName);
            _apmIncomingRequestDecorator.AddEventName(request, _apmContext);
            _apmIncomingRequestDecorator.AddMethodIdentifier(request, _apmContext);

            _apmIncomingRequestDecorator.AddClientName(request, _apmContext);

            _apmIncomingRequestDecorator.AddIncomingTraceId(request, _apmContext);
            _apmIncomingRequestDecorator.AddIncomingSpanId(request, _apmContext);
            _apmIncomingRequestDecorator.AddIncomingParentSpanId(request, _apmContext);
            _apmIncomingRequestDecorator.AddIncomingSampled(request, _apmContext);
            _apmIncomingRequestDecorator.AddIncomingFlags(request, _apmContext);

            _apmIncomingRequestDecorator.AddTraceId(request, _apmContext);
            _apmIncomingRequestDecorator.AddSpanId(request, _apmContext);
            _apmIncomingRequestDecorator.AddParentSpanId(request, _apmContext);
            _apmIncomingRequestDecorator.AddSampled(request, _apmContext);
            _apmIncomingRequestDecorator.AddFlags(request, _apmContext);

            _apmIncomingRequestDecorator.StartResponseTime(request);
            LogStartOfRequest(request, _startAction);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            _apmIncomingRequestDecorator.StopResponseTime(request);
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