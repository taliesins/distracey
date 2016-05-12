using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Distracey.Common;
using Distracey.Common.Message;
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
        private readonly IExecutionTimer _executionTimer;
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
            _executionTimer = new ExecutionTimer(new Stopwatch());
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //Initialize ApmContext if it does not exist
            //HttpContext.Current.SessionContext

            ApmWebApiRequestDecorator.AddEventName(actionContext, _pluralizationService);
            ApmWebApiRequestDecorator.AddMethodIdentifier(actionContext);
            ApmWebApiRequestDecorator.AddTracing(actionContext.Request);

            _offset = _executionTimer.Start();

            LogStartOfRequest(actionContext.Request, _offset);
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (_addResponseHeaders)
            {
                SetTracingResponseHeaders(actionExecutedContext);
            }
            base.OnActionExecuted(actionExecutedContext);
            LogStopOfRequest(actionExecutedContext, _offset);

            //Dispose ApmContext if it does not exist
            //HttpContext.Current.SessionContext
        }

        private void LogStartOfRequest(HttpRequestMessage request, TimeSpan offset)
        {
            var eventName = ApmHttpRequestMessageParser.GetEventName(request);
            var methodIdentifier = ApmHttpRequestMessageParser.GetMethodIdentifier(request);
            var traceId = ApmHttpRequestMessageParser.GetTraceId(request);
            var spanId = ApmHttpRequestMessageParser.GetSpanId(request);
            var parentSpanId = ApmHttpRequestMessageParser.GetParentSpanId(request);
            var sampled = ApmHttpRequestMessageParser.GetSampled(request);
            var flags = ApmHttpRequestMessageParser.GetFlags(request);
            
            object apmContextObject;
            if (!request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                apmContextObject = new ApmContext();
                request.Properties.Add(Constants.ApmContextPropertyKey, apmContextObject);
            }

            var apmContext = (IApmContext)apmContextObject;

            if (!apmContext.ContainsKey(Constants.EventNamePropertyKey))
            {
                apmContext[Constants.EventNamePropertyKey] = eventName;
            }

            if (!apmContext.ContainsKey(Constants.MethodIdentifierPropertyKey))
            {
                apmContext[Constants.MethodIdentifierPropertyKey] = eventName;
            }

            if (!apmContext.ContainsKey(Constants.RequestUriPropertyKey))
            {
                apmContext[Constants.RequestUriPropertyKey] = request.RequestUri.ToString();
            }

            if (!apmContext.ContainsKey(Constants.RequestMethodPropertyKey))
            {
                apmContext[Constants.RequestMethodPropertyKey] = request.Method.ToString();
            }

            var apmWebApiStartInformation = new ApmWebApiStartedMessage
            {
                EventName = eventName,
                MethodIdentifier = methodIdentifier,
                Flags = flags,
                ParentSpanId = parentSpanId,
                Sampled = sampled,
                SpanId = spanId,
                TraceId = traceId,
                Request = request
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmWebApiStartInformation.PublishMessage(apmContext, this);
        }

        private void LogStopOfRequest(HttpActionExecutedContext actionExecutedContext, TimeSpan offset)
        {
            var eventName = ApmHttpRequestMessageParser.GetEventName(actionExecutedContext.Request);
            var methodIdentifier = ApmHttpRequestMessageParser.GetMethodIdentifier(actionExecutedContext.Request);
            var traceId = ApmHttpRequestMessageParser.GetTraceId(actionExecutedContext.Request);
            var spanId = ApmHttpRequestMessageParser.GetSpanId(actionExecutedContext.Request);
            var parentSpanId = ApmHttpRequestMessageParser.GetParentSpanId(actionExecutedContext.Request);
            var sampled = ApmHttpRequestMessageParser.GetSampled(actionExecutedContext.Request);
            var flags = ApmHttpRequestMessageParser.GetFlags(actionExecutedContext.Request);

            object apmContextObject;
            if (!actionExecutedContext.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                throw new Exception("Add global filter for ApmWebApiFilterAttribute");
            }

            var apmContext = (IApmContext)apmContextObject;

            var apmWebApiFinishInformation = new ApmWebApiFinishedMessage
            {
                EventName = eventName,
                MethodIdentifier = methodIdentifier,
                Flags = flags,
                ParentSpanId = parentSpanId,
                Sampled = sampled,
                SpanId = spanId,
                TraceId = traceId,
                Request = actionExecutedContext.Request,
                Response = actionExecutedContext.Response,
                Exception = actionExecutedContext.Exception
            }.AsMessage(apmContext)
            .AsTimedMessage(offset);

            apmWebApiFinishInformation.PublishMessage(apmContext, this);
        }

        public static void SetTracingResponseHeaders(HttpActionExecutedContext actionContext)
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