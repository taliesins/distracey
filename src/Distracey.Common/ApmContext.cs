using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Distracey.Common.Helpers;

namespace Distracey.Common
{
    [Serializable]
    public class ApmContext : Dictionary<string, object>, IApmContext
    {
        public const string NoParent = "0";

        /// <summary>
        /// Create a new APM context, extracting information from containing contexts.
        /// </summary>
        /// <returns></returns>
        public static IApmContext GetContext(string eventName = "", string clientName = "", string methodIdentifier = "",
            string traceId = "", string spanId = "", string parentSpanId = "", string sampled = "", string flags = "")
        {
            if (string.IsNullOrEmpty(eventName) || string.IsNullOrEmpty(methodIdentifier))
            {
                //A small performance hit, but it means we get eventName for free
                var frame = new StackFrame(1);
                var method = frame.GetMethod();

                if (string.IsNullOrEmpty(eventName))
                {
                    eventName = GetEventName(method);
                }

                if (string.IsNullOrEmpty(methodIdentifier))
                {
                    methodIdentifier = GetMethodIdentifier(method);
                }
            }

            var apmContext = new ApmContext();

            SetContext(apmContext, methodIdentifier, eventName, clientName);
            SetTracing(apmContext, traceId, spanId, parentSpanId, sampled, flags);

            return apmContext;
        }

        private static void SetContext(IApmContext apmContext, string methodIdentifier, string eventName, string clientName)
        {
            apmContext[Constants.EventNamePropertyKey] = eventName;
            apmContext[Constants.MethodIdentifierPropertyKey] = methodIdentifier;
            apmContext[Constants.ClientNamePropertyKey] = clientName;
        }

        private static void SetTracing(IApmContext apmContext, string traceId, string spanId, string parentSpanId, string sampled, string flags)
        {
            if (string.IsNullOrEmpty(traceId))
            {
                traceId = ShortGuid.NewGuid().Value;
                spanId = traceId;
                parentSpanId = NoParent;
            }
            else
            {
                if (string.IsNullOrEmpty(spanId))
                {
                    spanId = ShortGuid.NewGuid().Value;
                }

                if (string.IsNullOrEmpty(parentSpanId))
                {
                    parentSpanId = NoParent;
                }
            }

            apmContext[Constants.TraceIdHeaderKey] = traceId;
            apmContext[Constants.SpanIdHeaderKey] = spanId;
            apmContext[Constants.ParentSpanIdHeaderKey] = parentSpanId;
            apmContext[Constants.SampledHeaderKey] = sampled;
            apmContext[Constants.FlagsHeaderKey] = flags;
        }

        public static string GetMethodIdentifier(MethodBase methodInfo)
        {
            var param = methodInfo.GetParameters()
                             .Select(parameter => string.Format("{0} {1}", parameter.ParameterType.Name, parameter.Name))
                             .ToArray();

            var arguments = string.Join(", ", param);

            return string.Format("{0}.{1}({2})", methodInfo.DeclaringType.FullName, methodInfo.Name, arguments);
        }

        public static string GetEventName(MethodBase methodInfo)
        {
            return string.Format("{0}.{1}", methodInfo.DeclaringType.Name, methodInfo.Name);
        }
    }
}