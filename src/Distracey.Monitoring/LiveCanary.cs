namespace Distracey.Monitoring
{
    public class LiveCanary : ICanary
    {
        public string Message { get; set; }
        public dynamic Content { get; set; }
    }
}