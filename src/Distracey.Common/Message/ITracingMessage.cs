namespace Distracey.Common.Message
{
    public interface ITracingMessage : IMessage
    {
        string TraceId { get; set; }
        string SpanId { get; set; }
        string ParentSpanId { get; set; }
        string Sampled { get; set; }
        string Flags { get; set; }
    }
}
