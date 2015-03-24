namespace Distracey.Null
{
    public class NullApmMethodHandler : ApmMethodHandlerBase
    {
        public static string ApplicationName { get; set; }

        public NullApmMethodHandler(IApmContext apmContext)
            : base(ApplicationName, Start, Finish)
        {
        }

        public static void Start(ApmMethodHandlerStartInformation apmMethodHandlerStartInformation)
        {
        }

        public static void Finish(ApmMethodHandlerFinishInformation apmMethodHandlerFinishInformation)
        {
        }
    }
}
