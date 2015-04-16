using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Distracey
{
    /// <summary>
    /// Used to track requests and responses made to webapi.
    /// </summary>
    public abstract class ApmWebApiFilterAttributeBase : ActionFilterAttribute
    {
        private readonly bool _addResponseHeaders;
        private readonly string _applicationName;
        private readonly Action<IApmContext, ApmWebApiStartInformation> _startAction;
        private readonly Action<IApmContext, ApmWebApiFinishInformation> _finishAction;
        private readonly PluralizationService _pluralizationService;

        private static readonly ApmWebApiRequestDecorator ApmWebApiRequestDecorator = new ApmWebApiRequestDecorator();
        private static readonly ApmOutgoingResponseDecorator ApmOutgoingResponseDecorator = new ApmOutgoingResponseDecorator();
        private static readonly ApmRequestParser ApmRequestParser = new ApmRequestParser();

        public ApmWebApiFilterAttributeBase(string applicationName, bool addResponseHeaders, Action<IApmContext, ApmWebApiStartInformation> startAction, Action<IApmContext, ApmWebApiFinishInformation> finishAction)
            : this(applicationName, addResponseHeaders, startAction, finishAction, PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us")))
        {
        }

        public ApmWebApiFilterAttributeBase(string applicationName, bool addResponseHeaders, Action<IApmContext, ApmWebApiStartInformation> startAction, Action<IApmContext, ApmWebApiFinishInformation> finishAction, PluralizationService pluralizationService)
        {
            _applicationName = applicationName;
            _addResponseHeaders = addResponseHeaders;
            _startAction = startAction;
            _finishAction = finishAction;
            _pluralizationService = pluralizationService;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ApmWebApiRequestDecorator.AddApplicationName(actionContext.Request, _applicationName);
            ApmWebApiRequestDecorator.AddEventName(actionContext, _pluralizationService);
            ApmWebApiRequestDecorator.AddMethodIdentifier(actionContext);
            ApmWebApiRequestDecorator.AddTracing(actionContext.Request);
            StartResponseTime(actionContext.Request);

            LogStartOfRequest(actionContext.Request, _startAction);
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
            LogStopOfRequest(actionExecutedContext, _finishAction);
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

        private void LogStartOfRequest(HttpRequestMessage request, Action<IApmContext, ApmWebApiStartInformation> startAction)
        {
            var applicationName = ApmRequestParser.GetApplicationName(request);
            var eventName = ApmRequestParser.GetEventName(request);
            var methodIdentifier = ApmRequestParser.GetMethodIdentifier(request);
            var traceId = ApmRequestParser.GetTraceId(request);
            var spanId = ApmRequestParser.GetSpanId(request);
            var parentSpanId = ApmRequestParser.GetParentSpanId(request);
            var sampled = ApmRequestParser.GetSampled(request);
            var flags = ApmRequestParser.GetFlags(request);
            
            var apmWebApiStartInformation = new ApmWebApiStartInformation
            {
                ApplicationName = applicationName,
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

            startAction(apmContext, apmWebApiStartInformation);
        }

        private void LogStopOfRequest(HttpActionExecutedContext actionExecutedContext, Action<IApmContext, ApmWebApiFinishInformation> finishAction)
        {
            var applicationName = ApmRequestParser.GetApplicationName(actionExecutedContext.Request);
            var eventName = ApmRequestParser.GetEventName(actionExecutedContext.Request);
            var methodIdentifier = ApmRequestParser.GetMethodIdentifier(actionExecutedContext.Request);
            var traceId = ApmRequestParser.GetTraceId(actionExecutedContext.Request);
            var spanId = ApmRequestParser.GetSpanId(actionExecutedContext.Request);
            var parentSpanId = ApmRequestParser.GetParentSpanId(actionExecutedContext.Request);
            var sampled = ApmRequestParser.GetSampled(actionExecutedContext.Request);
            var flags = ApmRequestParser.GetFlags(actionExecutedContext.Request);
            var responseTime = ApmRequestParser.GetResponseTime(actionExecutedContext.Request);

            var apmWebApiFinishInformation = new ApmWebApiFinishInformation
            {
                ApplicationName = applicationName,
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
                throw new Exception("Add global filter for ApmWebApiFilterAttributeBase");
            }

            var apmContext = (IApmContext)apmContextObject;
            if (!apmContext.ContainsKey(Constants.TimeTakeMsPropertyKey))
            {
                apmContext[Constants.TimeTakeMsPropertyKey] = apmWebApiFinishInformation.ResponseTime.ToString();
            }

            finishAction(apmContext, apmWebApiFinishInformation);
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