namespace Distracey.Null
{
    public class NullApmHttpClientDelegatingHandler : ApmHttpClientDelegatingHandlerBase
    {
        public static string ApplicationName { get; set; }

        public NullApmHttpClientDelegatingHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {
        }

        public static void Start(ApmHttpClientStartInformation apmWebApiStartInformation)
        {
        }

        public static void Finish(ApmHttpClientFinishInformation apmWebApiFinishInformation)
        {
        }
    }
}
