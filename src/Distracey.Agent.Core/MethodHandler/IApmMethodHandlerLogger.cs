using System.Threading.Tasks;
using Distracey.Agent.Common.MethodHandler;
using Distracey.Common;

namespace Distracey.Agent.Core.MethodHandler
{
    public interface IApmMethodHandlerLogger : IEventLogger
    {
        Task OnApmMethodHandlerStartInformation(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task);
        Task OnApmMethodHandlerFinishInformation(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task);
    }
}
