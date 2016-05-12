using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Distracey.Common;
using Distracey.Common.Message;
using Distracey.Common.Timer;

namespace Distracey.Agent.SystemWeb.HttpClient
{
    /// <summary>
    /// Used to track requests and responses made by an http client.
    /// </summary>
    public class ApmHttpClientDelegatingHandler : DelegatingHandler
    {
        private readonly IApmContext _apmContext;
        private readonly IExecutionTimer _executionTimer;
        private readonly ApmHttpClientRequestDecorator _apmHttpClientRequestDecorator = new ApmHttpClientRequestDecorator();
        private readonly ApmHttpRequestMessageParser _apmHttpRequestMessageParser = new ApmHttpRequestMessageParser();

        public ApmHttpClientDelegatingHandler(IApmContext apmContext, HttpMessageHandler httpMessageHandler)
        {
            _apmContext = apmContext;
            _executionTimer = new ExecutionTimer(new Stopwatch());
            InnerHandler = httpMessageHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Initialize ApmContext if it does not exist

            request.Properties[Constants.ApmContextPropertyKey] = _apmContext;

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

            var offset = _executionTimer.Start();

            LogStartOfRequest(request, offset);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            LogStopOfRequest(request, response, offset);

            //Dispose ApmContext if it does not exist previously

            return response;
        }

        private void LogStartOfRequest(HttpRequestMessage request, TimeSpan offset)
        {
            var eventName = _apmHttpRequestMessageParser.GetEventName(request);
            var methodIdentifier = _apmHttpRequestMessageParser.GetMethodIdentifier(request);

            var clientName = _apmHttpRequestMessageParser.GetClientName(request);

            var traceId = _apmHttpRequestMessageParser.GetTraceId(request);
            var spanId = _apmHttpRequestMessageParser.GetSpanId(request);
            var parentSpanId = _apmHttpRequestMessageParser.GetParentSpanId(request);
            var flags = _apmHttpRequestMessageParser.GetFlags(request);
            var sampled = _apmHttpRequestMessageParser.GetSampled(request);

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

            var apmHttpClientStartInformation = new ApmHttpClientStartedMessage
            {
                EventName = eventName,
                MethodIdentifier = methodIdentifier,
                Request = request,
                ClientName = clientName,
                TraceId = traceId,
                SpanId = spanId,
                ParentSpanId = parentSpanId,
                Sampled = sampled,
                Flags = flags
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmHttpClientStartInformation.PublishMessage(_apmContext, this);
        }

        private void LogStopOfRequest(HttpRequestMessage request, HttpResponseMessage response, TimeSpan offset)
        {
            var eventName = _apmHttpRequestMessageParser.GetEventName(request);
            var methodIdentifier = _apmHttpRequestMessageParser.GetMethodIdentifier(request);
            var clientName = _apmHttpRequestMessageParser.GetClientName(request);

            var traceId = _apmHttpRequestMessageParser.GetTraceId(request);
            var spanId = _apmHttpRequestMessageParser.GetSpanId(request);
            var parentSpanId = _apmHttpRequestMessageParser.GetParentSpanId(request);
            var flags = _apmHttpRequestMessageParser.GetFlags(request);
            var sampled = _apmHttpRequestMessageParser.GetSampled(request);

            object apmContextObject;
            if (!request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                throw new Exception("Add delegating handler filter");
            }

            var apmContext = (IApmContext)apmContextObject;

            if (!apmContext.ContainsKey(Constants.ResponseStatusCodePropertyKey))
            {
                apmContext[Constants.ResponseStatusCodePropertyKey] = response.StatusCode.ToString();
            }

            var apmHttpClientFinishInformation = new ApmHttpClientFinishedMessage
            {
                EventName = eventName,
                MethodIdentifier = methodIdentifier,
                Request = request,
                Response = response,
                ClientName = clientName,
                TraceId = traceId,
                SpanId = spanId,
                ParentSpanId = parentSpanId,
                Sampled = sampled,
                Flags = flags
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmHttpClientFinishInformation.PublishMessage(_apmContext, this);
        }
    }
}