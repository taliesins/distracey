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
        public static readonly List<IApmContextExtractor> ApmContextExtractors = new List<IApmContextExtractor>();

        /// <summary>
        /// Create a new APM context, extracting information from containing contexts.
        /// </summary>
        /// <returns></returns>
        public static IApmContext GetContext(string eventName = "", string clientName = "") 
        {
            //A small performance hit, but it means we get eventName for free
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            var apmContext = new ApmContext();

            SetContext(apmContext, method, eventName, clientName);

            foreach (var apmContextExtractor in ApmContextExtractors)
            {
                apmContextExtractor.GetContext(apmContext, method);
            }

            SetTracing(apmContext);

            return apmContext;
        }

        public static void SetContext(IApmContext apmContext, MethodBase method, string eventName = "", string clientName = "")
        {
            if (string.IsNullOrEmpty(eventName))
            {
                eventName = GetEventName(method);
            }
            var methodIdentifier = GetMethodIdentifier(method);

            apmContext[Constants.EventNamePropertyKey] = eventName;
            apmContext[Constants.MethodIdentifierPropertyKey] = methodIdentifier;
            apmContext[Constants.ClientNamePropertyKey] = clientName;
        }

        public static void SetTracing(IApmContext apmContext)
        {
            var clientName = apmContext.ContainsKey(Constants.ClientNamePropertyKey) ? (string)apmContext[Constants.ClientNamePropertyKey] : null;
            var incomingTraceId = apmContext.ContainsKey(Constants.IncomingTraceIdPropertyKey) ? (string)apmContext[Constants.IncomingTraceIdPropertyKey] : null;
            var incomingSpanId = apmContext.ContainsKey(Constants.IncomingSpanIdPropertyKey) ? (string)apmContext[Constants.IncomingSpanIdPropertyKey] : null;
            var incomingFlags = apmContext.ContainsKey(Constants.IncomingFlagsPropertyKey) ? (string)apmContext[Constants.IncomingFlagsPropertyKey] : null;
            var incomingSampled = apmContext.ContainsKey(Constants.IncomingSampledPropertyKey) ? (string)apmContext[Constants.IncomingSampledPropertyKey] : null;

            SetTracing(apmContext, clientName, incomingTraceId, incomingSpanId, incomingFlags, incomingSampled);
        }

        public static void SetTracing(IApmContext apmContext, string clientName, string incomingTraceId, string incomingSpanId, string incomingFlags, string incomingSampled)
        {
            var traceId = incomingTraceId;
            var spanId = incomingSpanId;

            if (string.IsNullOrEmpty(traceId))
            {
                traceId = string.Format("{0}={1}", clientName, ShortGuid.NewGuid().Value);
                spanId = traceId;
            }
            else
            {
                if (string.IsNullOrEmpty(spanId))
                {
                    spanId = string.Format("{0};{1}={2}", traceId, clientName, ShortGuid.NewGuid().Value);
                }
                else
                {
                    spanId = string.Format("{0};{1}={2}", spanId, clientName, ShortGuid.NewGuid().Value);
                }
            }

            var parentSpanId = spanId;

            if (string.IsNullOrEmpty(parentSpanId))
            {
                parentSpanId = 0.ToString();
            }

            var sampled = incomingSampled;
            var flags = incomingFlags;

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