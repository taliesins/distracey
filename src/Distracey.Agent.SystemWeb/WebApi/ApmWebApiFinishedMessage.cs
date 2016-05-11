using System;
using System.Net.Http;
using Distracey.Common.Message;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public class ApmWebApiFinishedMessage : ITracingMessage, ISourceMessage
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