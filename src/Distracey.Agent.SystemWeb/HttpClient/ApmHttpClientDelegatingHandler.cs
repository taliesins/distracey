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
        private readonly IExecutionTimer _executionTimer;
        private static readonly ApmHttpClientRequestDecorator ApmHttpClientRequestDecorator = new ApmHttpClientRequestDecorator();

        public ApmHttpClientDelegatingHandler() : this(new HttpClientHandler())
        {
        }

        public ApmHttpClientDelegatingHandler(HttpMessageHandler httpMessageHandler)
        {
            _executionTimer = new ExecutionTimer(new Stopwatch());
            InnerHandler = httpMessageHandler ?? new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apmContext = ApmContext.GetContext(string.Format("HttpClient.SendAsync.{0}", request.RequestUri));
            var activityId = ApmContext.StartActivityClientSend(apmContext);

            //Initialize ApmContext if it does not exist
            SetTracingRequestHeaders(apmContext, request);

            var offset = _executionTimer.Start();

            LogStartOfRequest(apmContext, request, offset);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            LogStopOfRequest(apmContext, request, response, offset);

            return response;
        }

        private void SetTracingRequestHeaders(IApmContext apmContext, HttpRequestMessage request)
        {
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

            ApmContext.StopActivityClientReceived();
        }
    }
}