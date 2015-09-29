using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Distracey.Web.HttpClient
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
        private readonly ApmHttpRequestMessageParser _apmHttpRequestMessageParser = new ApmHttpRequestMessageParser();

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
            //Initialize ApmContext if it does not exist

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

            //Dispose ApmContext if it does not exist previously

            return response;
        }

        private void LogStartOfRequest(HttpRequestMessage request, Action<IApmContext, ApmHttpClientStartInformation> startAction)
        {
            var applicationName = _apmHttpRequestMessageParser.GetApplicationName(request);
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
            var applicationName = _apmHttpRequestMessageParser.GetApplicationName(request);
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
    }
}