using System;
using System.Net.Http;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public class ApmWebApiFinishedMessage
    {
        public string EventName { get; set; }
        public string MethodIdentifier { get; set; }
        public HttpRequestMessage Request { get; set; }
        public HttpResponseMessage Response { get; set; }
        public long ResponseTime { get; set; }

        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
        public string Sampled { get; set; }
        public string Flags { get; set; }

        public Exception Exception { get; set; }
    }
}