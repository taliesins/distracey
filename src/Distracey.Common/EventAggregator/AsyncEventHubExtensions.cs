using System;
using System.Threading.Tasks;

namespace Distracey.Common.EventAggregator
{
    public static class AsyncEventHubExtensions
    {
        private static readonly AsyncEventHub AsyncEventHub = new AsyncEventHub();

        public static Task<Task[]> Publish<TEvent>(this object sender, Task<TEvent> eventData)
        {
            return AsyncEventHub.Publish(sender, eventData);
        }

        public static Task<Task[]> Publish<TEvent>(this object sender, TEvent eventData)
        {
            return AsyncEventHub.Publish(sender, eventData.AsTask());
        }

        public static Task Subscribe<TEvent>(this object sender, Func<Task<TEvent>, Task> eventHandlerTaskFactory)
        {
            return AsyncEventHub.Subscribe(sender, eventHandlerTaskFactory);
        }

        public static Task Unsubscribe<TEvent>(this object sender)
        {
            return AsyncEventHub.Unsubscribe<TEvent>(sender);
        }
    }
}
