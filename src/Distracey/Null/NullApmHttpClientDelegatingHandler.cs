using Distracey.Web.HttpClient;

namespace Distracey.Null
{
    public class NullApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        public static string ApplicationName { get; set; }

        public NullApmHttpClientDelegatingHandler(IApmContext apmContext)
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
