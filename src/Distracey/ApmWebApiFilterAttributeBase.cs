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
        private static readonly Type EnumerableType = typeof(IEnumerable);

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

        public void AddApplicationName(HttpRequestMessage request, string applicationName)
        {
            object applicationNameProperty;

            if (request.Properties.TryGetValue(Constants.ApplicationNamePropertyKey,
                out applicationNameProperty))
            {
                applicationName = (string)applicationNameProperty;
            }
            else
            {
                request.Properties[Constants.ApplicationNamePropertyKey] = applicationName;
            }
        }

        public void AddEventName(HttpActionContext actionContext, PluralizationService pluralizationService)
        {
            string eventName;

            object eventNameProperty;

            if (actionContext.Request.Properties.TryGetValue(Constants.EventNamePropertyKey, out eventNameProperty))
            {
                eventName = (string)eventNameProperty;
            }
            else
            {
                var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;

                if (actionContext.ActionDescriptor.ReturnType != null && EnumerableType.IsAssignableFrom(actionContext.ActionDescriptor.ReturnType))
                {
                    controllerName = pluralizationService.Pluralize(controllerName);
                }

                var methodType = actionContext.Request.Method;
                var actionName = actionContext.ActionDescriptor.ActionName;

                eventName = GetEventName(methodType, actionName, controllerName);

                actionContext.Request.Properties[Constants.EventNamePropertyKey] = eventName;
            }
        }

        public void AddMethodIdentifier(HttpActionContext actionContext)
        {
            string methodIdentifier;

            object methodIdentifierProperty;

            if (actionContext.Request.Properties.TryGetValue(Constants.MethodIdentifierPropertyKey, out methodIdentifierProperty))
            {
                methodIdentifier = (string)methodIdentifierProperty;
            }
            else
            {
                var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
                var methodType = actionContext.Request.Method;
                var actionName = actionContext.ActionDescriptor.ActionName;

                var methodInfo = ((ReflectedHttpActionDescriptor) (actionContext.ActionDescriptor)).MethodInfo;

                var param = methodInfo.GetParameters()
                   .Select(parameter => string.Format("{0} {1}", parameter.ParameterType.Name, parameter.Name))
                   .ToArray();

                var arguments = string.Join(", ", param);

                methodIdentifier = GetMethodIdentifier(methodType, controllerName, actionName, arguments);

                actionContext.Request.Properties[Constants.MethodIdentifierPropertyKey] = methodIdentifier;
            }
        }

        public void AddTracing(HttpRequestMessage request)
        {
            IEnumerable<string> traceIdHeaders = null;
            if (!request.Headers.TryGetValues(Constants.TraceIdHeaderKey, out traceIdHeaders))
            {
                var traceId = ShortGuid.NewGuid().Value;

                request.Properties[Constants.TraceIdHeaderKey] = traceId;
                request.Properties[Constants.SpanIdHeaderKey] = traceId;
                request.Properties[Constants.ParentSpanIdHeaderKey] =0.ToString();
            }
            else
            {
                request.Properties[Constants.TraceIdHeaderKey] = traceIdHeaders.First();

                IEnumerable<string> spanIdHeaders = null;
                if (!request.Headers.TryGetValues(Constants.SpanIdHeaderKey, out spanIdHeaders))
                {
                    var traceId = ShortGuid.NewGuid().Value;
                    request.Properties[Constants.SpanIdHeaderKey] = traceId;
                }
                else
                {
                    request.Properties[Constants.SpanIdHeaderKey] = spanIdHeaders.First();
                }

                IEnumerable<string> parentSpanIdHeaders = null;
                if (!request.Headers.TryGetValues(Constants.ParentSpanIdHeaderKey, out parentSpanIdHeaders))
                {
                    request.Properties[Constants.ParentSpanIdHeaderKey] = 0.ToString();
                }
                else
                {
                    request.Properties[Constants.ParentSpanIdHeaderKey] = parentSpanIdHeaders.First();
                }
            }

            IEnumerable<string> sampleHeaders = null;
            if (request.Headers.TryGetValues(Constants.SampledHeaderKey, out sampleHeaders))
            {
                request.Properties[Constants.SampledHeaderKey] = sampleHeaders.First();
            }

            IEnumerable<string> flagsHeaders = null;
            if (request.Headers.TryGetValues(Constants.FlagsHeaderKey, out flagsHeaders))
            {
                request.Properties[Constants.FlagsHeaderKey] = flagsHeaders.First();
            }
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
            var applicationName = string.Empty;
            object applicationNameObject;

            if (request.Properties.TryGetValue(Constants.ApplicationNamePropertyKey,
                out applicationNameObject))
            {
                applicationName = (string)applicationNameObject;
            }

            var eventName = string.Empty;
            object eventNameObject;

            if (request.Properties.TryGetValue(Constants.EventNamePropertyKey,
                out eventNameObject))
            {
                eventName = (string)eventNameObject;
            }

            var methodIdentifier = string.Empty;
            object methodIdentifierObject;

            if (request.Properties.TryGetValue(Constants.MethodIdentifierPropertyKey,
                out methodIdentifierObject))
            {
                methodIdentifier = (string)methodIdentifierObject;
            }

            var traceId = string.Empty;
            object traceIdObject;

            if (request.Properties.TryGetValue(Constants.TraceIdHeaderKey,
                out traceIdObject))
            {
                traceId = (string)traceIdObject;
            }

            var spanId = string.Empty;
            object spanIdObject;

            if (request.Properties.TryGetValue(Constants.SpanIdHeaderKey,
                out spanIdObject))
            {
                spanId = (string)spanIdObject;
            }

            var parentSpanId = string.Empty;
            object parentSpanIdObject;

            if (request.Properties.TryGetValue(Constants.ParentSpanIdHeaderKey,
                out parentSpanIdObject))
            {
                parentSpanId = (string)parentSpanIdObject;
            }

            string sampled = null;
            object sampledObject;

            if (request.Properties.TryGetValue(Constants.SampledHeaderKey,
                out sampledObject))
            {
                sampled = (string)sampledObject;
            }

            string flags = null;
            object flagsObject;

            if (request.Properties.TryGetValue(Constants.FlagsHeaderKey,
                out flagsObject))
            {
                flags = (string)flagsObject;
            }

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
            var applicationName = string.Empty;
            object applicationNameObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.ApplicationNamePropertyKey, out applicationNameObject))
            {
                applicationName = (string)applicationNameObject;
            }

            var eventName = string.Empty;
            object eventNameObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.EventNamePropertyKey, out eventNameObject))
            {
                eventName = (string)eventNameObject;
            }

            var methodIdentifier = string.Empty;
            object methodIdentifierObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.MethodIdentifierPropertyKey, out methodIdentifierObject))
            {
                methodIdentifier = (string)methodIdentifierObject;
            }

            var traceId = string.Empty;
            object traceIdObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.TraceIdHeaderKey, out traceIdObject))
            {
                traceId = (string)traceIdObject;
            }

            var spanId = string.Empty;
            object spanIdObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.SpanIdHeaderKey, out spanIdObject))
            {
                spanId = (string)spanIdObject;
            }

            var parentSpanId = string.Empty;
            object parentSpanIdObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.ParentSpanIdHeaderKey, out parentSpanIdObject))
            {
                parentSpanId = (string)parentSpanIdObject;
            }

            string sampled = null;
            object sampledObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.SampledHeaderKey, out sampledObject))
            {
                sampled = (string)sampledObject;
            }

            string flags = null;
            object flagsObject;

            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.FlagsHeaderKey, out flagsObject))
            {
                flags = (string)flagsObject;
            }

            object responseTimeObject;

            var responseTime = 0L;
            if (actionExecutedContext.Request.Properties.TryGetValue(Constants.ResponseTimePropertyKey, out responseTimeObject))
            {
                responseTime = ((Stopwatch)responseTimeObject).ElapsedMilliseconds;
            }

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
            AddApplicationName(actionContext.Request, _applicationName);
            AddEventName(actionContext, _pluralizationService);
            AddMethodIdentifier(actionContext);
            AddTracing(actionContext.Request);
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
            if (actionContext.Response == null)
            {
                return;
            }

            if (actionContext.Response != null && !actionContext.Response.Headers.Contains(Constants.TraceIdHeaderKey))
            {
                var traceId = string.Empty;
                object traceIdObject;

                if (actionContext.Request.Properties.TryGetValue(Constants.TraceIdHeaderKey, out traceIdObject))
                {
                    traceId = (string)traceIdObject;
                }

                actionContext.Response.Headers.Add(Constants.TraceIdHeaderKey, traceId);
            }

            if (actionContext.Response != null && !actionContext.Response.Headers.Contains(Constants.SpanIdHeaderKey))
            {
                var spanId = string.Empty;
                object spanIdObject;

                if (actionContext.Request.Properties.TryGetValue(Constants.SpanIdHeaderKey, out spanIdObject))
                {
                    spanId = (string)spanIdObject;
                }

                actionContext.Response.Headers.Add(Constants.SpanIdHeaderKey, spanId);
            }

            if (actionContext.Response != null && !actionContext.Response.Headers.Contains(Constants.ParentSpanIdHeaderKey))
            {
                var parentSpanId = string.Empty;
                object parentSpanIdObject;

                if (actionContext.Request.Properties.TryGetValue(Constants.ParentSpanIdHeaderKey, out parentSpanIdObject))
                {
                    parentSpanId = (string)parentSpanIdObject;
                }

                actionContext.Response.Headers.Add(Constants.ParentSpanIdHeaderKey, parentSpanId);
            }

            if (actionContext.Response != null && !actionContext.Response.Headers.Contains(Constants.SampledHeaderKey))
            {
                string sampled = null;
                object sampledObject;

                if (actionContext.Request.Properties.TryGetValue(Constants.SampledHeaderKey, out sampledObject))
                {
                    sampled = (string)sampledObject;
                }

                actionContext.Response.Headers.Add(Constants.SampledHeaderKey, sampled);
            }

            if (actionContext.Response != null && !actionContext.Response.Headers.Contains(Constants.SampledHeaderKey))
            {
                string flags = null;
                object flagsObject;

                if (actionContext.Request.Properties.TryGetValue(Constants.SampledHeaderKey, out flagsObject))
                {
                    flags = (string)flagsObject;
                }

                actionContext.Response.Headers.Add(Constants.SampledHeaderKey, flags);
            }
        }

        public static string GetMethodIdentifier(HttpMethod methodType, string controllerName, string actionName, string arguments)
        {
            return string.Format("{0}.{1}({2}) - {3}", controllerName, actionName, arguments, methodType);
        }

        public static string GetEventName(HttpMethod methodType, string actionName, string controllerName)
        {
            string eventName;

            if (methodType == HttpMethod.Delete)
            {
                eventName = actionName.ToLower() == "delete"
                    ? string.Format("Delete{0}", controllerName)
                    : string.Format("{0}{1}", actionName, controllerName);
            }
            else if (methodType == HttpMethod.Get)
            {
                eventName = actionName.ToLower() == "get"
                    ? string.Format("Get{0}", controllerName)
                    : string.Format("{0}{1}", actionName, controllerName);
            }
            else if (methodType == HttpMethod.Post)
            {
                eventName = actionName.ToLower() == "post"
                    ? string.Format("Create{0}", controllerName)
                    : string.Format("{0}{1}", actionName, controllerName);
            }
            else if (methodType == HttpMethod.Put)
            {
                eventName = actionName.ToLower() == "put"
                    ? string.Format("Update{0}", controllerName)
                    : string.Format("{0}{1}", actionName, controllerName);
            }
            else if (methodType == HttpMethod.Head)
            {
                eventName = actionName.ToLower() == "head"
                    ? string.Format("MetaDataFor{0}", controllerName)
                    : string.Format("{0}{1}", actionName, controllerName);
            }
            else if (methodType == HttpMethod.Options)
            {
                eventName = actionName.ToLower() == "options"
                    ? string.Format("OptionsFor{0}", controllerName)
                    : string.Format("{0}{1}", actionName, controllerName);
            }
            else if (methodType == HttpMethod.Trace)
            {
                eventName = actionName.ToLower() == "trace"
                    ? string.Format("TraceFor{0}", controllerName)
                    : string.Format("{0}{1}", actionName, controllerName);
            }
            else
            {
                eventName = string.Format("{0}{1}", actionName, controllerName);
            }
            return eventName;
        }
    }
}