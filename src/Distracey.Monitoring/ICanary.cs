namespace Distracey.Monitoring
{
    public interface ICanary
    {
        string Message { get; set; }
        dynamic Content { get; set; }
    }
}