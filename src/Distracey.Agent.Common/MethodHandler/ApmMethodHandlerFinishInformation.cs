namespace Distracey.Agent.Common.MethodHandler
{
    public class ApmMethodHandlerFinishInformation
    {
        public string EventName { get; set; }
        public string MethodIdentifier { get; set; }
        public long ResponseTime { get; set; }

        public string ClientName { get; set; }

        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
        public string Sampled { get; set; }
        public string Flags { get; set; }

        public string IncomingTraceId { get; set; }
        public string IncomingSpanId { get; set; }
        public string IncomingParentSpanId { get; set; }
        public string IncomingSampled { get; set; }
        public string IncomingFlags { get; set; }
    }
}