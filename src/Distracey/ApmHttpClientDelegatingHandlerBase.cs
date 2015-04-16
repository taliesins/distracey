﻿using System;
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
        private readonly Action<IApmContext, ApmHttpClientStartInformation> _startAction;
        private readonly Action<IApmContext, ApmHttpClientFinishInformation> _finishAction;
        private readonly ApmHttpClientRequestDecorator _apmHttpClientRequestDecorator = new ApmHttpClientRequestDecorator();
        private readonly ApmRequestParser _apmRequestParser = new ApmRequestParser();

        public ApmHttpClientDelegatingHandlerBase(IApmContext apmContext, string applicationName, Action<IApmContext, ApmHttpClientStartInformation> startAction, Action<IApmContext, ApmHttpClientFinishInformation> finishAction)
        {
            _apmContext = apmContext;
            _applicationName = applicationName;
            _startAction = startAction;
            _finishAction = finishAction;
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

        private void LogStartOfRequest(HttpRequestMessage request, Action<IApmContext, ApmHttpClientStartInformation> startAction)
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

            object apmContextObject;
            if (!request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                apmContextObject = new ApmContext();
                request.Properties.Add(Constants.ApmContextPropertyKey, apmContextObject);
            }

            var apmContext = (IApmContext)apmContextObject;

            if (!apmContext.ContainsKey(Constants.RequestUriPropertyKey))
            {
                apmContext[Constants.RequestUriPropertyKey] = request.RequestUri.ToString();
            }

            if (!apmContext.ContainsKey(Constants.RequestMethodPropertyKey))
            {
                apmContext[Constants.RequestMethodPropertyKey] = request.Method.ToString();
            }

            startAction(apmContext, apmHttpClientStartInformation);
        }

        private void LogStopOfRequest(HttpRequestMessage request, HttpResponseMessage response, Action<IApmContext, ApmHttpClientFinishInformation> finishAction)
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

            object apmContextObject;
            if (!request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                throw new Exception("Add delegating handler filter");
            }

            var apmContext = (IApmContext)apmContextObject;

            if (!apmContext.ContainsKey(Constants.TimeTakeMsPropertyKey))
            {
                apmContext[Constants.TimeTakeMsPropertyKey] = responseTime.ToString();
            }

            if (!apmContext.ContainsKey(Constants.ResponseStatusCodePropertyKey))
            {
                apmContext[Constants.ResponseStatusCodePropertyKey] = response.StatusCode.ToString();
            }

            finishAction(apmContext, apmHttpClientFinishInformation);
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
    }
}