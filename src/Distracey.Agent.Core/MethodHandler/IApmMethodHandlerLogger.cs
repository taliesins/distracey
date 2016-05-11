using System.Threading.Tasks;
using Distracey.Common;

namespace Distracey.Agent.Core.MethodHandler
{
    public interface IApmMethodHandlerLogger : IEventLogger
    {
        Task OnApmMethodHandlerStartedMessage(Task<ApmEvent<ApmMethodHandlerStartedMessage>> task);
        Task OnApmMethodHandlerFinishedMessage(Task<ApmEvent<ApmMethodHandlerFinishedMessage>> task);
    }
}
