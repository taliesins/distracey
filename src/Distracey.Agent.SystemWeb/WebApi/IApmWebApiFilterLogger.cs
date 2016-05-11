using System.Threading.Tasks;
using Distracey.Common;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public interface IApmWebApiFilterLogger : IEventLogger
    {
        Task OnApmWebApiStartedMessage(Task<ApmEvent<ApmWebApiStartedMessage>> task);
        Task OnApmWebApiFinishedMessage(Task<ApmEvent<ApmWebApiFinishedMessage>> task);
    }
}
