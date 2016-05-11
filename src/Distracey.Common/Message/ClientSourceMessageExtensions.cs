namespace Distracey.Common.Message
{
    public static class ClientSourceMessageExtensions
    {
        public static T AsClientSourceMessage<T>(this T message, IApmContext apmContext)
            where T : IClientSourceMessage
        {
            var clientName = apmContext.GetClientName();

            message.ClientName = clientName;

            return message;
        }
    }
}
