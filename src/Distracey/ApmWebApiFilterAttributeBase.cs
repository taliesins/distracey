﻿using System;
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

        private static ApmWebApiRequestDecorator _apmWebApiRequestDecorator = new ApmWebApiRequestDecorator();
        private static ApmOutgoingResponseDecorator _apmOutgoingResponseDecorator = new ApmOutgoingResponseDecorator();
        private static ApmRequestParser _apmRequestParser = new ApmRequestParser();

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

        public void LogStartOfRequest(HttpRequestMessage request, Action<IApmContext, ApmWebApiStartInformation> startAction)
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

        public void LogStopOfRequest(HttpActionExecutedContext actionExecutedContext, Action<IApmContext, ApmWebApiFinishInformation> finishAction)
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

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _apmWebApiRequestDecorator.AddApplicationName(actionContext.Request, _applicationName);
            _apmWebApiRequestDecorator.AddEventName(actionContext, _pluralizationService);
            _apmWebApiRequestDecorator.AddMethodIdentifier(actionContext);
            _apmWebApiRequestDecorator.AddTracing(actionContext.Request);
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