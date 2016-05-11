namespace Distracey.Common.Message
{
    public interface ISourceMessage : IMessage
    {
        string EventName { get; set; }
        string MethodIdentifier { get; set; }
    }
}
