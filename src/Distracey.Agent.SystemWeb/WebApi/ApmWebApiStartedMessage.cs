using System.Net.Http;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public class ApmWebApiStartedMessage
    {
        public string EventName { get; set; }
        public string MethodIdentifier { get; set; }
        public HttpRequestMessage Request { get; set; }

        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
        public string Sampled { get; set; }
        public string Flags { get; set; }
    }
}