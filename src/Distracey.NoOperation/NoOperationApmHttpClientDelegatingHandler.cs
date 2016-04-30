using Distracey.Web.HttpClient;

namespace Distracey.NoOperation
{
    public class NoOperationApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        public static string ApplicationName { get; set; }

        public NoOperationApmHttpClientDelegatingHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {
        }

        public static void Start(IApmContext apmContext, ApmHttpClientStartInformation apmWebApiStartInformation)
        {
        }

        public static void Finish(IApmContext apmContext, ApmHttpClientFinishInformation apmWebApiFinishInformation)
        {
        }
    }
}
