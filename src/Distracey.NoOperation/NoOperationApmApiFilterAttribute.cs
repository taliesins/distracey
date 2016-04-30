using Distracey.Web.WebApi;

namespace Distracey.NoOperation
{
    public class NoOperationApmApiFilterAttribute : ApmWebApiFilterAttributeBase
    {
        public static string ApplicationName { get; set; }
        public static bool AddResponseHeaders { get; set; }

        public NoOperationApmApiFilterAttribute()
            : base(ApplicationName, AddResponseHeaders, Start, Finish)
        {
        }

        public static void Start(IApmContext apmContext, ApmWebApiStartInformation apmWebApiStartInformation)
        {

        }

        public static void Finish(IApmContext apmContext, ApmWebApiFinishInformation apmWebApiFinishInformation)
        {
        }
    }
}
