namespace Distracey.Common.Message
{
    public static class TracingMessageExtensions
    {
        public static T AsTracingMessage<T>(this T message, IApmContext apmContext)
            where T : ITracingMessage
        {
            var traceId = apmContext.GetTraceId();
            var spanId = apmContext.GetSpanId();
            var parentSpanId = apmContext.GetParentSpanId();
            var flags = apmContext.GetFlags();
            var sampled = apmContext.GetSampled();

            message.Flags = flags;
            message.ParentSpanId = parentSpanId;
            message.Sampled = sampled;
            message.SpanId = spanId;
            message.TraceId = traceId;

            return message;
        }
    }
}
