using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        private readonly Action<ApmWebApiStartInformation> _startAction;
        private readonly Action<ApmWebApiFinishInformation> _finishAction;
        private readonly PluralizationService _pluralizationService;

        private static ApmWebApiRequestDecorator _apmWebApiRequestDecorator = new ApmWebApiRequestDecorator();
        private static ApmOutgoingResponseDecorator _apmOutgoingResponseDecorator = new ApmOutgoingResponseDecorator();
        private static ApmRequestParser _apmRequestParser = new ApmRequestParser();

        public ApmWebApiFilterAttributeBase(string applicationName, bool addResponseHeaders, Action<ApmWebApiStartInformation> startAction, Action<ApmWebApiFinishInformation> finishAction)
            : this(applicationName, addResponseHeaders, startAction, finishAction, PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us")))
        {
        }

        public ApmWebApiFilterAttributeBase(string applicationName, bool addResponseHeaders, Action<ApmWebApiStartInformation> startAction, Action<ApmWebApiFinishInformation> finishAction, PluralizationService pluralizationService)
        {
            _applicationName = applicationName;
            _addResponseHeaders = addResponseHeaders;
            _startAction = startAction;
            _finishAction = finishAction;
            _pluralizationService = pluralizationService;
        }



        public void StartResponseTime(HttpRequestMessage request)
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

        public void StopResponseTime(HttpActionExecutedContext actionExecutedContext)
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

        public void LogStartOfRequest(HttpRequestMessage request, Action<ApmWebApiStartInformation> startAction)
        {
            var applicationName = _apmRequestParser.GetApplicationName(request);
            var eventName = _apmRequestParser.GetEventName(request);
            var methodIdentifier = _apmRequestParser.GetMethodIdentifier(request);
            var traceId = _apmRequestParser.GetTraceId(request);
            var spanId = _apmRequestParser.GetSpanId(request);
            var parentSpanId = _apmRequestParser.GetParentSpanId(request);
            var sampled = _apmRequestParser.GetSampled(request);
            var flags = _apmRequestParser.GetFlags(request);
            
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

            startAction(apmWebApiStartInformation);
        }

        public void LogStopOfRequest(HttpActionExecutedContext actionExecutedContext, Action<ApmWebApiFinishInformation> finishAction)
        {
            var applicationName = _apmRequestParser.GetApplicationName(actionExecutedContext.Request);
            var eventName = _apmRequestParser.GetEventName(actionExecutedContext.Request);
            var methodIdentifier = _apmRequestParser.GetMethodIdentifier(actionExecutedContext.Request);
            var traceId = _apmRequestParser.GetTraceId(actionExecutedContext.Request);
            var spanId = _apmRequestParser.GetSpanId(actionExecutedContext.Request);
            var parentSpanId = _apmRequestParser.GetParentSpanId(actionExecutedContext.Request);
            var sampled = _apmRequestParser.GetSampled(actionExecutedContext.Request);
            var flags = _apmRequestParser.GetFlags(actionExecutedContext.Request);
            var responseTime = _apmRequestParser.GetResponseTime(actionExecutedContext.Request);

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

            finishAction(apmWebApiFinishInformation);
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _apmWebApiRequestDecorator.AddApplicationName(actionContext.Request, _applicationName);
            _apmWebApiRequestDecorator.AddEventName(actionContext, _pluralizationService);
            _apmWebApiRequestDecorator.AddMethodIdentifier(actionContext);
            _apmWebApiRequestDecorator.AddTracing(actionContext.Request);
            LogStartOfRequest(actionContext.Request, _startAction);
            StartResponseTime(actionContext.Request);
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

        public static void SetTracingResponseHeaders(HttpActionExecutedContext actionContext)
        {
            _apmOutgoingResponseDecorator.AddTraceId(actionContext);
            _apmOutgoingResponseDecorator.AddSpanId(actionContext);
            _apmOutgoingResponseDecorator.AddParentSpanId(actionContext);
            _apmOutgoingResponseDecorator.AddSampled(actionContext);
            _apmOutgoingResponseDecorator.AddFlags(actionContext);
        }

        public static string GetMethodIdentifier(HttpMethod methodType, string controllerName, string actionName, string arguments)
        {
            return _apmWebApiRequestDecorator.GetMethodIdentifier(methodType, controllerName, actionName, arguments);
        }

        public static string GetEventName(HttpMethod methodType, string actionName, string controllerName)
        {
            return _apmWebApiRequestDecorator.GetEventName(methodType, actionName, controllerName);
        }
    }
}