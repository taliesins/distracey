using QuickGraph;

namespace Distracey.Tracking
{
    public class ExecutionGraph : AdjacencyGraph<ExecutionVertex, ExecutionEdge>
    {
        public ExecutionGraph() : base(true)
        {   
        }
    }
}
