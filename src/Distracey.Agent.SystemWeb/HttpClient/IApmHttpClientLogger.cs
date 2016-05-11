using System.Threading.Tasks;
using Distracey.Common;

namespace Distracey.Agent.SystemWeb.HttpClient
{
    public interface IApmHttpClientLogger : IEventLogger
    {
        Task OnApmHttpClientStartedMessage(Task<ApmEvent<ApmHttpClientStartedMessage>> task);
        Task OnApmHttpClientFinishedMessage(Task<ApmEvent<ApmHttpClientFinishedMessage>> task);
    }
}
