namespace Distracey.Common.Message
{
    public interface IClientSourceMessage : IMessage
    {
        string ClientName { get; set; }
    }
}
