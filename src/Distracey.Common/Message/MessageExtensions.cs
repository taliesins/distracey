using Distracey.Common.EventAggregator;

namespace Distracey.Common.Message
{
    public static class MessageExtensions
    {
        public static T AsMessage<T>(this T message, IApmContext apmContext)
            where T : IMessage
        {
            var sourceMessage = message as ISourceMessage;
            if (sourceMessage != null)
            {
                sourceMessage.AsSourceMessage(apmContext);
            }

            var clientSourceMessage = message as IClientSourceMessage;
            if (clientSourceMessage != null)
            {
                clientSourceMessage.AsClientSourceMessage(apmContext);
            }

            var tracingMessage = message as ITracingMessage;
            if (tracingMessage != null)
            {
                tracingMessage.AsTracingMessage(apmContext);
            }

            return message;
        }

        public static void PublishMessage<T>(this T message, IApmContext apmContext, object sender)
            where T : IMessage
        {
            var eventContext = new ApmEvent<T>
            {
                ApmContext = apmContext,
                Event = message
            };

            sender.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
