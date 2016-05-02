using System.Web.Http.Filters;
using Distracey.Common;

namespace Distracey.Web.WebApi
{
    public class ApmOutgoingResponseDecorator
    {
        public void AddTraceId(HttpActionExecutedContext actionContext)
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
        }

        public void AddSpanId(HttpActionExecutedContext actionContext)
        {
            if (actionContext.Response == null)
            {
                return;
            }

            if (actionContext.Response != null && !actionContext.Response.Headers.Contains(Constants.SpanIdHeaderKey))
            {
                var spanId = string.Empty;
                object spanIdObject;

                if (actionContext.Request.Properties.TryGetValue(Constants.SpanIdHeaderKey, out spanIdObject))
                {
                    spanId = (string) spanIdObject;
                }

                actionContext.Response.Headers.Add(Constants.SpanIdHeaderKey, spanId);
            }
        }

        public void AddParentSpanId(HttpActionExecutedContext actionContext)
        {
            if (actionContext.Response == null)
            {
                return;
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
        }

        public void AddSampled(HttpActionExecutedContext actionContext)
        {
            if (actionContext.Response == null)
            {
                return;
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
        }

        public void AddFlags(HttpActionExecutedContext actionContext)
        {
            if (actionContext.Response == null)
            {
                return;
            }

            if (actionContext.Response != null && !actionContext.Response.Headers.Contains(Constants.FlagsHeaderKey))
            {
                string flags = null;
                object flagsObject;

                if (actionContext.Request.Properties.TryGetValue(Constants.FlagsHeaderKey, out flagsObject))
                {
                    flags = (string)flagsObject;
                }

                actionContext.Response.Headers.Add(Constants.FlagsHeaderKey, flags);
            }
        }

    }
}
