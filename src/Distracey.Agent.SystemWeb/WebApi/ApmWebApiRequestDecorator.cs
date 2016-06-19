using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using Distracey.Common;
using Distracey.Common.Helpers;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public class ApmWebApiRequestDecorator
    {
        public const string NoParent = "0";
        private static readonly Type EnumerableType = typeof(IEnumerable);

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

        public string GetEventName(HttpMethod methodType, string actionName, string controllerName)
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

                var methodInfo = actionContext.ActionDescriptor is ReflectedHttpActionDescriptor ? 
                    ((ReflectedHttpActionDescriptor)(actionContext.ActionDescriptor)).MethodInfo :
                    null;

                var param = methodInfo == null ? 
                    new string[]{}: 
                    methodInfo.GetParameters()
                       .Select(parameter => string.Format("{0} {1}", parameter.ParameterType.Name, parameter.Name))
                       .ToArray();

                var arguments = string.Join(", ", param);

                methodIdentifier = GetMethodIdentifier(methodType, controllerName, actionName, arguments);

                actionContext.Request.Properties[Constants.MethodIdentifierPropertyKey] = methodIdentifier;
            }
        }

        public string GetMethodIdentifier(HttpMethod methodType, string controllerName, string actionName, string arguments)
        {
            return string.Format("{0}.{1}({2}) - {3}", controllerName, actionName, arguments, methodType);
        }

        public void AddTracing(HttpRequestMessage request)
        {
            IEnumerable<string> traceIdHeaders = null;
            if (!request.Headers.TryGetValues(Constants.TraceIdHeaderKey, out traceIdHeaders))
            {
                var traceId = ShortGuid.NewGuid().Value;

                request.Properties[Constants.TraceIdHeaderKey] = traceId;
                request.Properties[Constants.SpanIdHeaderKey] = traceId;
                request.Properties[Constants.ParentSpanIdHeaderKey] = NoParent;
            }
            else
            {
                var traceId = traceIdHeaders.First();
                request.Properties[Constants.TraceIdHeaderKey] = traceId;

                IEnumerable<string> spanIdHeaders = null;
                if (!request.Headers.TryGetValues(Constants.SpanIdHeaderKey, out spanIdHeaders))
                {
                    var spanId = ShortGuid.NewGuid().Value;
                    request.Properties[Constants.SpanIdHeaderKey] = spanId;
                }
                else
                {
                    request.Properties[Constants.SpanIdHeaderKey] = spanIdHeaders.First();
                }

                IEnumerable<string> parentSpanIdHeaders = null;
                if (!request.Headers.TryGetValues(Constants.ParentSpanIdHeaderKey, out parentSpanIdHeaders))
                {
                    request.Properties[Constants.ParentSpanIdHeaderKey] = NoParent;
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
    }
}
