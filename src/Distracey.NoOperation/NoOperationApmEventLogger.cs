using System;
using System.Threading.Tasks;
using Distracey.Agent.Core.MethodHandler;
using Distracey.Agent.SystemWeb.HttpClient;
using Distracey.Agent.SystemWeb.WebApi;
using Distracey.Common;
using Distracey.Common.EventAggregator;

namespace Distracey.NoOperation
{
    public class NoOperationApmEventLogger : IApmMethodHandlerLogger, IApmHttpClientLogger, IApmWebApiFilterLogger, IDisposable
    {
        public NoOperationApmEventLogger()
        {
            this.Subscribe<ApmEvent<ApmMethodHandlerStartedMessage>>(OnApmMethodHandlerStartedMessage);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>(OnApmMethodHandlerFinishedMessage);
            this.Subscribe<ApmEvent<ApmHttpClientStartedMessage>>(OnApmHttpClientStartedMessage);
            this.Subscribe<ApmEvent<ApmHttpClientFinishedMessage>>(OnApmHttpClientFinishedMessage);
            this.Subscribe<ApmEvent<ApmWebApiStartedMessage>>(OnApmWebApiStartedMessage);
            this.Subscribe<ApmEvent<ApmWebApiFinishedMessage>>(OnApmWebApiFinishedMessage);
        }

        public Task OnApmMethodHandlerStartedMessage(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmMethodHandlerFinishedMessage(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmHttpClientStartedMessage(Task<ApmEvent<ApmHttpClientStartedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmHttpClientFinishedMessage(Task<ApmEvent<ApmHttpClientFinishedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmWebApiStartedMessage(Task<ApmEvent<ApmWebApiStartedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmWebApiFinishedMessage(Task<ApmEvent<ApmWebApiFinishedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public void Dispose()
        {
            this.Unsubscribe<ApmEvent<ApmMethodHandlerStartedMessage>>();
            this.Unsubscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>();
            this.Unsubscribe<ApmEvent<ApmHttpClientStartedMessage>>();
            this.Unsubscribe<ApmEvent<ApmHttpClientFinishedMessage>>();
            this.Unsubscribe<ApmEvent<ApmWebApiStartedMessage>>();
            this.Unsubscribe<ApmEvent<ApmWebApiFinishedMessage>>();
        }
    }
}
