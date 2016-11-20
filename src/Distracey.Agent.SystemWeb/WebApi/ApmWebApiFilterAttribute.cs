using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Distracey.Common;
using Distracey.Common.Message;
using Distracey.Common.Session;
using Distracey.Common.Timer;

namespace Distracey.Agent.SystemWeb.WebApi
{
    /// <summary>
    /// Used to track requests and responses made to webapi.
    /// </summary>
    public class ApmWebApiFilterAttribute : ActionFilterAttribute
    {
        private readonly bool _addResponseHeaders;
        private readonly PluralizationService _pluralizationService;
        private IApmContext _apmContext;
        private IExecutionTimer _executionTimer;
        private TimeSpan _offset;

        private static readonly ApmWebApiRequestDecorator ApmWebApiRequestDecorator = new ApmWebApiRequestDecorator();
        private static readonly ApmOutgoingResponseDecorator ApmOutgoingResponseDecorator = new ApmOutgoingResponseDecorator();
        private static readonly ApmHttpRequestMessageParser ApmHttpRequestMessageParser = new ApmHttpRequestMessageParser();

        public ApmWebApiFilterAttribute(bool addResponseHeaders)
            : this(addResponseHeaders, PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us")))
        {
        }

        public ApmWebApiFilterAttribute(bool addResponseHeaders, PluralizationService pluralizationService)
        {
            _addResponseHeaders = addResponseHeaders;
            _pluralizationService = pluralizationService;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            SessionContext.StartSession();

            _executionTimer = new ExecutionTimer(new Stopwatch());
            _offset = _executionTimer.Start();

            SetTracingRequestHeaders(actionContext, _pluralizationService);

             _apmContext = ExtractContextFromHttpRequest(actionContext.Request);

            //configure
            var spanId = _apmContext.GetSpanId();
            var traceId = _apmContext.GetTraceId();
            var sampled = _apmContext.GetSampled();
            var flags = _apmContext.GetFlags();

            ApmContext.StartActivityServerReceived(spanId, traceId, sampled, flags);

            LogStartOfRequest(_apmContext, actionContext.Request, _offset);
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                if (_addResponseHeaders)
                {
                    SetTracingResponseHeaders(actionExecutedContext);
                }
                base.OnActionExecuted(actionExecutedContext);
                LogStopOfRequest(_apmContext, actionExecutedContext, _offset);
            }
            finally
            {
                ApmContext.StopActivityServerSend();
                SessionContext.StopSession();
            }
        }

        private void LogStartOfRequest(IApmContext apmContext, HttpRequestMessage request, TimeSpan offset)
        {
            var apmWebApiStartInformation = new ApmWebApiStartedMessage
            {
                Request = request
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmWebApiStartInformation.PublishMessage(apmContext, this);
        }

        private void LogStopOfRequest(IApmContext apmContext, HttpActionExecutedContext actionExecutedContext, TimeSpan offset)
        {
            var apmWebApiFinishInformation = new ApmWebApiFinishedMessage
            {
                Request = actionExecutedContext.Request,
                Response = actionExecutedContext.Response,
                Exception = actionExecutedContext.Exception
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmWebApiFinishInformation.PublishMessage(apmContext, this);
        }

        private static void SetTracingRequestHeaders(HttpActionContext actionContext, PluralizationService pluralizationService)
        {
            ApmWebApiRequestDecorator.AddEventName(actionContext, pluralizationService);
            ApmWebApiRequestDecorator.AddMethodIdentifier(actionContext);
            ApmWebApiRequestDecorator.AddMethodArgs(actionContext);
            ApmWebApiRequestDecorator.AddTracing(actionContext.Request);
        }

        private static IApmContext ExtractContextFromHttpRequest(HttpRequestMessage request)
        {
            var eventName = ApmHttpRequestMessageParser.GetEventName(request);
            var methodIdentifier = ApmHttpRequestMessageParser.GetMethodIdentifier(request);
            var traceId = ApmHttpRequestMessageParser.GetTraceId(request);
            var parentSpanId = ApmHttpRequestMessageParser.GetParentSpanId(request);
            var spanId = ApmHttpRequestMessageParser.GetSpanId(request);
            var sampled = ApmHttpRequestMessageParser.GetSampled(request);
            var flags = ApmHttpRequestMessageParser.GetFlags(request);

            var apmContext = ApmContext.GetContext(
                eventName:eventName, 
                methodIdentifier:methodIdentifier, 
                traceId:traceId, 
                spanId:spanId, 
                parentSpanId:parentSpanId, 
                sampled:sampled, 
                flags:flags);
            
            if (!apmContext.ContainsKey(Constants.RequestUriPropertyKey))
            {
                apmContext[Constants.RequestUriPropertyKey] = request.RequestUri.ToString();
            }

            if (!apmContext.ContainsKey(Constants.RequestMethodPropertyKey))
            {
                apmContext[Constants.RequestMethodPropertyKey] = request.Method.ToString();
            }

            return apmContext;
        }

        private static void SetTracingResponseHeaders(HttpActionExecutedContext actionContext)
        {
            ApmOutgoingResponseDecorator.AddTraceId(actionContext);
            ApmOutgoingResponseDecorator.AddSpanId(actionContext);
            ApmOutgoingResponseDecorator.AddParentSpanId(actionContext);
            ApmOutgoingResponseDecorator.AddSampled(actionContext);
            ApmOutgoingResponseDecorator.AddFlags(actionContext);
        }

        public static string GetMethodIdentifier(HttpMethod methodType, string controllerName, string actionName, string arguments)
        {
            return ApmWebApiRequestDecorator.GetMethodIdentifier(methodType, controllerName, actionName, arguments);
        }

        public static string GetEventName(HttpMethod methodType, string actionName, string controllerName)
        {
            return ApmWebApiRequestDecorator.GetEventName(methodType, actionName, controllerName);
        }
    }
}