namespace Distracey.Common.Message
{
    public static class SourceMessageExtensions
    {
        public static T AsSourceMessage<T>(this T message, IApmContext apmContext)
            where T : ISourceMessage
        {
            var eventName = apmContext.GetEventName();
            var methodIdentifier = apmContext.GetMethodIdentifier();

            message.EventName = eventName;
            message.MethodIdentifier = methodIdentifier;

            return message;
        }
    }
}
