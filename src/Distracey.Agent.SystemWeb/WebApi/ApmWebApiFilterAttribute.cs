using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using Distracey.Common.Message;

namespace Distracey.Agent.SystemWeb.WebApi
{
    /// <summary>
    /// Used to track requests and responses made to webapi.
    /// </summary>
    public class ApmWebApiFilterAttribute : ActionFilterAttribute
    {
        private readonly bool _addResponseHeaders;
        private readonly PluralizationService _pluralizationService;

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
            //Initialize ApmContext if it does not exist
            //HttpContext.Current.SessionContext

            ApmWebApiRequestDecorator.AddEventName(actionContext, _pluralizationService);
            ApmWebApiRequestDecorator.AddMethodIdentifier(actionContext);
            ApmWebApiRequestDecorator.AddTracing(actionContext.Request);
            StartResponseTime(actionContext.Request);

            LogStartOfRequest(actionContext.Request);
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (_addResponseHeaders)
            {
                SetTracingResponseHeaders(actionExecutedContext);
            }
            base.OnActionExecuted(actionExecutedContext);
            StopResponseTime(actionExecutedContext);
            LogStopOfRequest(actionExecutedContext);

            //Dispose ApmContext if it does not exist
            //HttpContext.Current.SessionContext
        }

        private void StartResponseTime(HttpRequestMessage request)
        {
            object responseTimeObject;

            if (request.Properties.TryGetValue(Constants.ResponseTimePropertyKey,
                out responseTimeObject))
            {
                var stopWatch = (Stopwatch)responseTimeObject;
                if (!stopWatch.IsRunning)
                {
                    stopWatch.Start();
                }
            }
            else
            {
                request.Properties[Constants.ResponseTimePropertyKey] = Stopwatch.StartNew();
            }
        }

        private void StopResponseTime(HttpActionExecutedContext actionExecutedContext)
        {
            object responseTimeObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.ResponseTimePropertyKey,
                out responseTimeObject))
            {
                var stopWatch = (Stopwatch)responseTimeObject;
                if (stopWatch.IsRunning)
                {
                    stopWatch.Stop();
                }
            }
        }

        private void LogStartOfRequest(HttpRequestMessage request)
        {
            var eventName = ApmHttpRequestMessageParser.GetEventName(request);
            var methodIdentifier = ApmHttpRequestMessageParser.GetMethodIdentifier(request);
            var traceId = ApmHttpRequestMessageParser.GetTraceId(request);
            var spanId = ApmHttpRequestMessageParser.GetSpanId(request);
            var parentSpanId = ApmHttpRequestMessageParser.GetParentSpanId(request);
            var sampled = ApmHttpRequestMessageParser.GetSampled(request);
            var flags = ApmHttpRequestMessageParser.GetFlags(request);
            
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
            };

            object apmContextObject;
            if (!request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                apmContextObject = new ApmContext();
                request.Properties.Add(Constants.ApmContextPropertyKey, apmContextObject);
            }

            var apmContext = (IApmContext)apmContextObject;

            if (!apmContext.ContainsKey(Constants.EventNamePropertyKey))
            {
                apmContext[Constants.EventNamePropertyKey] = apmWebApiStartInformation.EventName;
            }

            if (!apmContext.ContainsKey(Constants.MethodIdentifierPropertyKey))
            {
                apmContext[Constants.MethodIdentifierPropertyKey] = apmWebApiStartInformation.EventName;
            }

            if (!apmContext.ContainsKey(Constants.RequestUriPropertyKey))
            {
                apmContext[Constants.RequestUriPropertyKey] = apmWebApiStartInformation.Request.RequestUri.ToString();
            }

            if (!apmContext.ContainsKey(Constants.RequestMethodPropertyKey))
            {
                apmContext[Constants.RequestMethodPropertyKey] = apmWebApiStartInformation.Request.Method.ToString();
            }

            apmWebApiStartInformation.PublishMessage(apmContext, this);
        }

        private void LogStopOfRequest(HttpActionExecutedContext actionExecutedContext)
        {
            var eventName = ApmHttpRequestMessageParser.GetEventName(actionExecutedContext.Request);
            var methodIdentifier = ApmHttpRequestMessageParser.GetMethodIdentifier(actionExecutedContext.Request);
            var traceId = ApmHttpRequestMessageParser.GetTraceId(actionExecutedContext.Request);
            var spanId = ApmHttpRequestMessageParser.GetSpanId(actionExecutedContext.Request);
            var parentSpanId = ApmHttpRequestMessageParser.GetParentSpanId(actionExecutedContext.Request);
            var sampled = ApmHttpRequestMessageParser.GetSampled(actionExecutedContext.Request);
            var flags = ApmHttpRequestMessageParser.GetFlags(actionExecutedContext.Request);
            var responseTime = ApmHttpRequestMessageParser.GetResponseTime(actionExecutedContext.Request);

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
                ResponseTime = responseTime,
                Exception = actionExecutedContext.Exception
            };

            object apmContextObject;
            if (!apmWebApiFinishInformation.Request.Properties.TryGetValue(Constants.ApmContextPropertyKey, out apmContextObject))
            {
                throw new Exception("Add global filter for ApmWebApiFilterAttribute");
            }

            var apmContext = (IApmContext)apmContextObject;
            if (!apmContext.ContainsKey(Constants.TimeTakeMsPropertyKey))
            {
                apmContext[Constants.TimeTakeMsPropertyKey] = apmWebApiFinishInformation.ResponseTime.ToString();
            }

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