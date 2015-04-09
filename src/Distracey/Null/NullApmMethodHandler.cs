namespace Distracey.Null
{
    public class NullApmMethodHandler : ApmMethodHandlerBase
    {
        public static string ApplicationName { get; set; }

        public NullApmMethodHandler()
            : base(ApplicationName, Start, Finish)
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
