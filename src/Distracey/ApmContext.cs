using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using Distracey.Null;

namespace Distracey
{
    public class ApmContext : Dictionary<string, string>, IApmContext
	{
        public static List<IApmHttpClientDelegatingHandlerFactory> ApmHttpClientDelegatingHandlerFactories = new List<IApmHttpClientDelegatingHandlerFactory>();
        public static List<IApmMethodHandlerFactory> ApmMethodHttpFactories = new List<IApmMethodHandlerFactory>();

        public static ApmMethodHandlerBase GetInvoker(IApmContext apmContext)
        {
            if (!ApmMethodHttpFactories.Any())
            {
                return new NullApmMethodHandlerFactory().Create(apmContext);
            }

            ApmMethodHandlerBase apmMethodHandler = null;

            foreach (var apmMethodHttpFactory in ApmMethodHttpFactories)
            {
                var currentApmMethod = apmMethodHttpFactory.Create(apmContext);
                if (apmMethodHandler != null)
                {
                    currentApmMethod.InnerHandler = apmMethodHandler;
                }

                apmMethodHandler = currentApmMethod;
            }

            return apmMethodHandler;
        }

        public static ApmHttpClientDelegatingHandlerBase GetDelegatingHandler(IApmContext apmContext)
        {
            if (!ApmHttpClientDelegatingHandlerFactories.Any())
            {
                return new NullApmHttpClientDelegatingHandlerFactory().Create(apmContext);
            }

            ApmHttpClientDelegatingHandlerBase apmHttpClientDelegatingHandler = null;

            foreach (var apmHttpClientDelegatingHandlerFactory in ApmHttpClientDelegatingHandlerFactories)
            {
                var currentApmHttpClientDelegatingHandler = apmHttpClientDelegatingHandlerFactory.Create(apmContext);
                if (apmHttpClientDelegatingHandler != null)
                {
                    currentApmHttpClientDelegatingHandler.InnerHandler = apmHttpClientDelegatingHandler;
                }
                else
                {
                    currentApmHttpClientDelegatingHandler.InnerHandler = new HttpClientHandler();
                }
                apmHttpClientDelegatingHandler = currentApmHttpClientDelegatingHandler;
            }

            return apmHttpClientDelegatingHandler;
        }

        public static IApmContext GetContext() 
        {
            //A small performance hit, but it means we get eventName for free
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            var apmContext = new ApmContext();

            SetContext(apmContext, method);

            HttpRequestMessage request = null;
            if (HttpContext.Current != null)
            {
                request = HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
            }

            SetIncomingTracing(apmContext, request);

            SetTracing(apmContext);

            return apmContext;
        }

        public static void SetContext(IApmContext apmContext, MethodBase method)
        {
            var eventName = ApmHttpClientDelegatingHandlerBase.GetEventName(method);
            var methodIdentifier = ApmHttpClientDelegatingHandlerBase.GetMethodIdentifier(method);
            var clientName = ApmHttpClientDelegatingHandlerBase.GetClientName();

            apmContext[Constants.EventNamePropertyKey] = eventName;
            apmContext[Constants.MethodIdentifierPropertyKey] = methodIdentifier;
            apmContext[Constants.ClientNamePropertyKey] = clientName;
        }

        public static void SetIncomingTracing(IApmContext apmContext, HttpRequestMessage request)
        {
            var incomingTraceId = string.Empty;
            var incomingSpanId = string.Empty;
            var incomingParentSpanId = string.Empty;
            var incomingSampled = string.Empty;
            var incomingFlags = string.Empty;

            if (request != null)
            {
                incomingTraceId = ApmHttpClientDelegatingHandlerBase.GetTraceId(request);
                incomingSpanId = ApmHttpClientDelegatingHandlerBase.GetSpanId(request);
                incomingParentSpanId = ApmHttpClientDelegatingHandlerBase.GetParentSpanId(request);
                incomingSampled = ApmHttpClientDelegatingHandlerBase.GetSampled(request);
                incomingFlags = ApmHttpClientDelegatingHandlerBase.GetFlags(request);
            }

            apmContext[Constants.IncomingTraceIdPropertyKey] = incomingTraceId;
            apmContext[Constants.IncomingSpanIdPropertyKey] = incomingSpanId;
            apmContext[Constants.IncomingParentSpanIdPropertyKey] = incomingParentSpanId;
            apmContext[Constants.IncomingSampledPropertyKey] = incomingSampled;
            apmContext[Constants.IncomingFlagsPropertyKey] = incomingFlags;
        }

        public static void SetTracing(IApmContext apmContext)
        {
            var clientName = apmContext.ContainsKey(Constants.ClientNamePropertyKey) ? apmContext[Constants.ClientNamePropertyKey] : null;
            var incomingTraceId = apmContext.ContainsKey(Constants.IncomingTraceIdPropertyKey) ?  apmContext[Constants.IncomingTraceIdPropertyKey] : null;
            var incomingSpanId = apmContext.ContainsKey(Constants.IncomingSpanIdPropertyKey) ?  apmContext[Constants.IncomingSpanIdPropertyKey] : null;
            var incomingFlags = apmContext.ContainsKey(Constants.IncomingFlagsPropertyKey) ?  apmContext[Constants.IncomingFlagsPropertyKey] : null;
            var incomingSampled = apmContext.ContainsKey(Constants.IncomingSampledPropertyKey) ? apmContext[Constants.IncomingSampledPropertyKey] : null;

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
	}
}