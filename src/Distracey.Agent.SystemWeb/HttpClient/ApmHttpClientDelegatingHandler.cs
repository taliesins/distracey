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
        private static readonly ApmHttpClientRequestDecorator ApmHttpClientRequestDecorator = new ApmHttpClientRequestDecorator();

        public ApmHttpClientDelegatingHandler(IApmContext apmContext, HttpMessageHandler httpMessageHandler)
        {
            _apmContext = apmContext;
            _executionTimer = new ExecutionTimer(new Stopwatch());
            InnerHandler = httpMessageHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Initialize ApmContext if it does not exist

            SetTracingRequestHeaders(_apmContext, request);

            var offset = _executionTimer.Start();

            LogStartOfRequest(_apmContext, request, offset);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            LogStopOfRequest(_apmContext, request, response, offset);

            //Dispose ApmContext if it does not exist previously

            return response;
        }

        private void SetTracingRequestHeaders(IApmContext apmContext, HttpRequestMessage request)
        {
            ApmHttpClientRequestDecorator.AddEventName(request, apmContext);
            ApmHttpClientRequestDecorator.AddMethodIdentifier(request, apmContext);

            ApmHttpClientRequestDecorator.AddClientName(request, apmContext);

            ApmHttpClientRequestDecorator.AddIncomingTraceId(request, apmContext);
            ApmHttpClientRequestDecorator.AddIncomingSpanId(request, apmContext);
            ApmHttpClientRequestDecorator.AddIncomingParentSpanId(request, apmContext);
            ApmHttpClientRequestDecorator.AddIncomingSampled(request, apmContext);
            ApmHttpClientRequestDecorator.AddIncomingFlags(request, apmContext);

            ApmHttpClientRequestDecorator.AddTraceId(request, apmContext);
            ApmHttpClientRequestDecorator.AddSpanId(request, apmContext);
            ApmHttpClientRequestDecorator.AddParentSpanId(request, apmContext);
            ApmHttpClientRequestDecorator.AddSampled(request, apmContext);
            ApmHttpClientRequestDecorator.AddFlags(request, apmContext);
        }

        private void LogStartOfRequest(IApmContext apmContext, HttpRequestMessage request, TimeSpan offset)
        {
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
                Request = request,
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmHttpClientStartInformation.PublishMessage(apmContext, this);
        }

        private void LogStopOfRequest(IApmContext apmContext, HttpRequestMessage request, HttpResponseMessage response, TimeSpan offset)
        {
            if (!apmContext.ContainsKey(Constants.ResponseStatusCodePropertyKey))
            {
                apmContext[Constants.ResponseStatusCodePropertyKey] = response.StatusCode.ToString();
            }

            var apmHttpClientFinishInformation = new ApmHttpClientFinishedMessage
            {
                Request = request,
                Response = response,
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmHttpClientFinishInformation.PublishMessage(apmContext, this);
        }
    }
}