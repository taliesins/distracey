using System;
using System.Threading.Tasks;
using Distracey.Agent.Common.MethodHandler;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using Distracey.Web.HttpClient;
using Distracey.Web.WebApi;

namespace Distracey.NoOperation
{
    public class NoOperationApmEventLogger : IEventLogger, IDisposable
    {
        public NoOperationApmEventLogger()
        {
            this.Subscribe<ApmEvent<ApmMethodHandlerStartInformation>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishInformation>>(OnApmMethodHandlerFinishInformation);
            this.Subscribe<ApmEvent<ApmHttpClientStartInformation>>(OnApmHttpClientStartInformation);
            this.Subscribe<ApmEvent<ApmHttpClientFinishInformation>>(OnApmHttpClientFinishInformation);
            this.Subscribe<ApmEvent<ApmWebApiStartInformation>>(OnApmWebApiStartInformation);
            this.Subscribe<ApmEvent<ApmWebApiFinishInformation>>(OnApmWebApiFinishInformation);
        }

        private Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartInformation>> task)
        {
            return Task.FromResult(false);
        }

        private Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishInformation>> task)
        {
            return Task.FromResult(false);
        }

        private Task OnApmHttpClientStartInformation(Task<ApmEvent<ApmHttpClientStartInformation>> task)
        {
            return Task.FromResult(false);
        }

        private Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishInformation>> task)
        {
            return Task.FromResult(false);
        }

        private Task OnApmWebApiStartInformation(Task<ApmEvent<ApmWebApiStartInformation>> task)
        {
            return Task.FromResult(false);
        }

        private Task OnApmWebApiFinishInformation(Task<ApmEvent<ApmWebApiFinishInformation>> task)
        {
            return Task.FromResult(false);
        }

        public void Dispose()
        {
            this.Unsubscribe<ApmEvent<ApmMethodHandlerStartInformation>>();
            this.Unsubscribe<ApmEvent<ApmMethodHandlerFinishInformation>>();
            this.Unsubscribe<ApmEvent<ApmHttpClientStartInformation>>();
            this.Unsubscribe<ApmEvent<ApmHttpClientFinishInformation>>();
            this.Unsubscribe<ApmEvent<ApmWebApiStartInformation>>();
            this.Unsubscribe<ApmEvent<ApmWebApiFinishInformation>>();
        }
    }
}
