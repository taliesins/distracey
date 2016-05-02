using System.Threading.Tasks;
using Distracey.Common.EventAggregator;
using Distracey.MethodHandler;

namespace Distracey.NoOperation
{
    public class NoOperationEventLogger : IEventLogger
    {
        public NoOperationEventLogger()
        {
            this.Subscribe<ApmEvent<ApmMethodHandlerStartInformation>>(OnApmMethodHandlerStartInformation);
            this.Subscribe<ApmEvent<ApmMethodHandlerFinishInformation>>(OnApmMethodHandlerFinishInformation);
        }

        private Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartInformation>> task)
        {
            return Task.FromResult(false);
        }

        private Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishInformation>> task)
        {
            return Task.FromResult(false);
        }
    }
}
