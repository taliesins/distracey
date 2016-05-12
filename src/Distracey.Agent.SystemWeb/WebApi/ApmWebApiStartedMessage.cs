using System;
using System.Net.Http;
using Distracey.Common.Message;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public class ApmWebApiStartedMessage : ITracingMessage, ISourceMessage, ITimedMessage
    {
        public string EventName { get; set; }
        public string MethodIdentifier { get; set; }
        public HttpRequestMessage Request { get; set; }

        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
        public string Sampled { get; set; }
        public string Flags { get; set; }

        public TimeSpan Offset { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartTime { get; set; }
    }
}