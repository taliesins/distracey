using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Distracey.Common.Session;

namespace Distracey.Common
{
    [Serializable]
    public class ApmContext : Dictionary<string, object>, IApmContext
    {
        public const string NoParent = "0";

        /// <summary>
        /// SR
        /// </summary>
        /// <param name="spanId"></param>
        /// <param name="traceId"></param>
        /// <param name="sampled"></param>
        /// <param name="flags"></param>
        public static void StartActivityServerReceived(string spanId, string traceId, string sampled, string flags)
        {
            var activity = SessionContext.StartActivity();
            activity.Items[Constants.SpanIdHeaderKey] = spanId;
            activity.Items[Constants.TraceIdHeaderKey] = traceId;
            activity.Items[Constants.SampledHeaderKey] = sampled;
            activity.Items[Constants.FlagsHeaderKey] = flags;
        }

        /// <summary>
        /// CS
        /// </summary>
        /// <returns></returns>
        public static Guid StartActivityClientSend()
        {
            var activity = SessionContext.StartActivity();
            return activity.ActivityId;
        }


        public static Guid StartActivityClientSend(IApmContext apmContext)
        {
            var parentSpanId = string.Empty;
            var traceId = string.Empty;
            var sampled = string.Empty;
            var flags = string.Empty;

            if (SessionContext.CurrentActivity != null)
            {
                parentSpanId = (string)SessionContext.CurrentActivity.Items[Constants.SpanIdHeaderKey];
                traceId = (string)SessionContext.CurrentActivity.Items[Constants.TraceIdHeaderKey];
                sampled = (string)SessionContext.CurrentActivity.Items[Constants.SampledHeaderKey];
                flags = (string)SessionContext.CurrentActivity.Items[Constants.FlagsHeaderKey];
            }

            var activityId = StartActivityClientSend();
            apmContext[Constants.SpanIdHeaderKey] = activityId.ToString();
            apmContext[Constants.ParentSpanIdHeaderKey] = parentSpanId;
            apmContext[Constants.TraceIdHeaderKey] = traceId;
            apmContext[Constants.SampledHeaderKey] = sampled;
            apmContext[Constants.FlagsHeaderKey] = flags;

            return activityId;
        }

        /// <summary>
        /// CR
        /// </summary>
        public static void StopActivityClientReceived()
        {
            SessionContext.StopActivity();
        }

        /// <summary>
        /// SS
        /// </summary>
        public static void StopActivityServerSend()
        {
            SessionContext.StopActivity();
        }

        /// <summary>
        /// Create a new APM context, extracting information from containing contexts.
        /// </summary>
        /// <returns></returns>
        public static IApmContext GetContext(string eventName = "", string clientName = "", string methodIdentifier = "", string methodArgs = "",
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

            SetContext(apmContext, methodIdentifier, methodArgs, eventName, clientName);
            SetTracing(apmContext, traceId, spanId, parentSpanId, sampled, flags);

            return apmContext;
        }

        private static void SetContext(IApmContext apmContext, string methodIdentifier, string methodArgs, string eventName, string clientName)
        {
            apmContext[Constants.EventNamePropertyKey] = eventName;
            apmContext[Constants.MethodIdentifierPropertyKey] = methodArgs;
            apmContext[Constants.MethodArgsPropertyKey] = methodIdentifier;
            apmContext[Constants.ClientNamePropertyKey] = clientName;
        }

        private static void SetTracing(IApmContext apmContext, string traceId, string spanId, string parentSpanId, string sampled, string flags)
        {
            if (string.IsNullOrEmpty(traceId))
            {
                traceId = Guid.NewGuid().ToString();
                spanId = traceId;
                parentSpanId = NoParent;
            }
            else
            {
                if (string.IsNullOrEmpty(spanId))
                {
                    spanId = Guid.NewGuid().ToString();
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