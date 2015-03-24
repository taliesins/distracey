namespace Distracey.Monitoring
{
    public class DeadCanary : ICanary
    {
        public string Message { get; set; }
        public dynamic Content { get; set; }
        public string ExceptionDetails { get; set; }
    }
}