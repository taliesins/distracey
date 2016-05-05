using System.Threading.Tasks;
using Distracey.Common;

namespace Distracey.Agent.SystemWeb.HttpClient
{
    public interface IApmHttpClientLogger : IEventLogger
    {
        Task OnApmHttpClientStartInformation(Task<ApmEvent<ApmHttpClientStartedMessage>> task);
        Task OnApmHttpClientFinishInformation(Task<ApmEvent<ApmHttpClientFinishedMessage>> task);
    }
}
