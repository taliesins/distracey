using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Distracey.Common;
using Distracey.Common.EventAggregator;

namespace Distracey.Agent.SystemWeb.HttpClient
{
    /// <summary>
    /// Used to track requests and responses made by an http client.
    /// </summary>
    public class ApmHttpClientDelegatingHandler : DelegatingHandler
    {
        private readonly IApmContext _apmContext;
        private readonly ApmHttpClientRequestDecorator _apmHttpClientRequestDecorator = new ApmHttpClientRequestDecorator();
        private readonly ApmHttpRequestMessageParser _apmHttpRequestMessageParser = new ApmHttpRequestMessageParser();

        public ApmHttpClientDelegatingHandler(IApmContext apmContext, HttpMessageHandler httpMessageHandler)
        {
            _apmContext = apmContext;
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

            _apmHttpClientRequestDecorator.StartResponseTime(request);
            LogStartOfRequest(request);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            _apmHttpClientRequestDecorator.StopResponseTime(request);
            LogStopOfRequest(request, response);

            //Dispose ApmContext if it does not exist previously

            return response;
        }

        private void LogStartOfRequest(HttpRequestMessage request)
        {
            var eventName = _apmHttpRequestMessageParser.GetEventName(request);
            var methodIdentifier = _apmHttpRequestMessageParser.GetMethodIdentifier(request);
            var clientName = _apmHttpRequestMessageParser.GetClientName(request);

            var incomingTraceId = _apmHttpRequestMessageParser.GetIncomingTraceId(request);
            var incomingSpanId = _apmHttpRequestMessageParser.GetIncomingSpanId(request);
            var incomingParentSpanId = _apmHttpRequestMessageParser.GetIncomingParentSpanId(request);
            var incomingFlags = _apmHttpRequestMessageParser.GetIncomingFlags(request);
            var incomingSampled = _apmHttpRequestMessageParser.GetIncomingSampled(request);

            var traceId = _apmHttpRequestMessageParser.GetTraceId(request);
            var spanId = _apmHttpRequestMessageParser.GetSpanId(request);
            var parentSpanId = _apmHttpRequestMessageParser.GetParentSpanId(request);
            var flags = _apmHttpRequestMessageParser.GetFlags(request);
            var sampled = _apmHttpRequestMessageParser.GetSampled(request);

            var apmHttpClientStartInformation = new ApmHttpClientStartInformation
            {
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

            var eventContext = new ApmEvent<ApmHttpClientStartInformation>
            {
                ApmContext = _apmContext,
                Event = apmHttpClientStartInformation
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfRequest(HttpRequestMessage request, HttpResponseMessage response)
        {
            var eventName = _apmHttpRequestMessageParser.GetEventName(request);
            var methodIdentifier = _apmHttpRequestMessageParser.GetMethodIdentifier(request);
            var responseTime = _apmHttpRequestMessageParser.GetResponseTime(request);
            var clientName = _apmHttpRequestMessageParser.GetClientName(request);

            var incomingTraceId = _apmHttpRequestMessageParser.GetIncomingTraceId(request);
            var incomingSpanId = _apmHttpRequestMessageParser.GetIncomingSpanId(request);
            var incomingParentSpanId = _apmHttpRequestMessageParser.GetIncomingParentSpanId(request);
            var incomingFlags = _apmHttpRequestMessageParser.GetIncomingFlags(request);
            var incomingSampled = _apmHttpRequestMessageParser.GetIncomingSampled(request);

            var traceId = _apmHttpRequestMessageParser.GetTraceId(request);
            var spanId = _apmHttpRequestMessageParser.GetSpanId(request);
            var parentSpanId = _apmHttpRequestMessageParser.GetParentSpanId(request);
            var flags = _apmHttpRequestMessageParser.GetFlags(request);
            var sampled = _apmHttpRequestMessageParser.GetSampled(request);

            var apmHttpClientFinishInformation = new ApmHttpClientFinishInformation
            {
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

            var eventContext = new ApmEvent<ApmHttpClientFinishInformation>
            {
                ApmContext = _apmContext,
                Event = apmHttpClientFinishInformation
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}