﻿namespace Distracey.Common
{
    public class Constants
    {
        public const string ResponseTimePropertyKey = "ResponseTime";
        
        public const string ClientNamePropertyKey = "ClientName";
        public const string ApmContextPropertyKey = "ApmContext";
        public const string EventNamePropertyKey = "eventName";
        public const string MethodArgsPropertyKey = "args";
        public const string MethodIdentifierPropertyKey = "methodIdentifier";
        public const string RequestMethodPropertyKey = "requestMethod";
        public const string RequestUriPropertyKey = "requestUri";
        public const string ResponseStatusCodePropertyKey = "responseStatusCode";

        public const string TraceIdHeaderKey = "X-B3-TraceId";
        public const string SpanIdHeaderKey = "X-B3-SpanId";
        public const string ParentSpanIdHeaderKey = "X-B3-ParentSpanId";
        public const string SampledHeaderKey = "X-B3-Sampled";
        public const string FlagsHeaderKey = "X-B3-Flags";
    }
}