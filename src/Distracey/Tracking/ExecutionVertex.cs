namespace Distracey.Tracking
{
    public class ExecutionVertex
    {
        public ExecutionVertextType VertexType { get; private set; }
        public string ApplicationName { get; set; }
        public string EventName { get; set; }
        /* ExecutionVertextType to popuate EventName with this logic:
         * Entry = Initiator
         * Event = Event Name
         * Exit = Client Name
         * */
    }
}
