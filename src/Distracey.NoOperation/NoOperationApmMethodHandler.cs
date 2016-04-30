using Distracey.MethodHandler;

namespace Distracey.NoOperation
{
    public class NoOperationApmMethodHandler : ApmMethodHandlerBase
    {
        public static string ApplicationName { get; set; }

        public NoOperationApmMethodHandler(IApmContext apmContext)
            : base(apmContext, ApplicationName, Start, Finish)
        {
        }

        public static void Start(IApmContext apmContext, ApmMethodHandlerStartInformation apmMethodHandlerStartInformation)
        {
        }

        public static void Finish(IApmContext apmContext, ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation)
        {
        }
    }
}
