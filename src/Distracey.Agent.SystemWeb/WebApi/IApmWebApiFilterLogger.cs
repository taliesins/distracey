using System.Threading.Tasks;
using Distracey.Common;

namespace Distracey.Agent.SystemWeb.WebApi
{
    public interface IApmWebApiFilterLogger : IEventLogger
    {
        Task OnApmWebApiStartInformation(Task<ApmEvent<ApmWebApiStartedMessage>> task);
        Task OnApmWebApiFinishInformation(Task<ApmEvent<ApmWebApiFinishedMessage>> task);
    }
}
