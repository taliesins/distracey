using System;
using System.Threading.Tasks;
using Distracey.Agent.Common.MethodHandler;
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
            this.Subscribe<ApmEvent<ApmMethodHandlerStartedMessage>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishedMessage>>(OnApmMethodHandlerFinishInformation);
            this.Subscribe<ApmEvent<ApmHttpClientStartedMessage>>(OnApmHttpClientStartInformation);
            this.Subscribe<ApmEvent<ApmHttpClientFinishedMessage>>(OnApmHttpClientFinishInformation);
            this.Subscribe<ApmEvent<ApmWebApiStartedMessage>>(OnApmWebApiStartInformation);
            this.Subscribe<ApmEvent<ApmWebApiFinishedMessage>>(OnApmWebApiFinishInformation);
        }

        public Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmHttpClientStartInformation(Task<ApmEvent<ApmHttpClientStartedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmWebApiStartInformation(Task<ApmEvent<ApmWebApiStartedMessage>> task)
        {
            return Task.FromResult(false);
        }

        public Task OnApmWebApiFinishInformation(Task<ApmEvent<ApmWebApiFinishedMessage>> task)
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
