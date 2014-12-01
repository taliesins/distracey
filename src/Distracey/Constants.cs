namespace Distracey
{
    public class Constants
    {
        public const string ResponseTimePropertyKey = "ResponseTime";
        
        public const string ApplicationNamePropertyKey = "ApplicationName";
        public const string ClientNamePropertyKey = "ClientName";
        public const string ApmContextPropertyKey = "ApmContext";
        public const string EventNamePropertyKey = "eventName";
        public const string MethodIdentifierPropertyKey = "methodIdentifier";
        public const string TimeTakeMsPropertyKey = "timeTakenMs";
        public const string RequestMethodPropertyKey = "requestMethod";
        public const string RequestUriPropertyKey = "requestUri";
        public const string ResponseStatusCodePropertyKey = "responseStatusCode";
        public const string EventTypePropertyKey = "eventType";

        public const string IncomingTraceIdPropertyKey = "incomingTraceId";
        public const string IncomingSpanIdPropertyKey = "incomingSpanId";
        public const string IncomingParentSpanIdPropertyKey = "incomingParentSpanId";
        public const string IncomingSampledPropertyKey = "incomingSampled";
        public const string IncomingFlagsPropertyKey = "incomingFlags";

        public const string TraceIdHeaderKey = "X-B3-TraceId";
        public const string SpanIdHeaderKey = "X-B3-SpanId";
        public const string ParentSpanIdHeaderKey = "X-B3-ParentSpanId";
        public const string SampledHeaderKey = "X-B3-ParentSpanId";
        public const string FlagsHeaderKey = "X-B3-Flags";
    }
}