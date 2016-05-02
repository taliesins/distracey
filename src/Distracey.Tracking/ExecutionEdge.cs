using QuickGraph;

namespace Distracey.Tracking
{
    public class ExecutionEdge : Edge<ExecutionVertex>
    {
        public ExecutionEdgeType EdgeType { get; private set; }
        public string TraceId { get; set; }
        public string ParentSpanId { get; set; }
        public string SpanId { get; set; }
        public string Sampled { get; set; }
        public string Flags { get; set; }

        public long Ticks { get; set; }

        public ExecutionEdge(ExecutionVertex source, ExecutionVertex target, ExecutionEdgeType edgeType)
            : base(source, target)
        {
            EdgeType = edgeType;
        }
    }
}
