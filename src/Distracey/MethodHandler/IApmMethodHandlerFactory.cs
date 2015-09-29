namespace Distracey.MethodHandler
{
    public interface IApmMethodHandlerFactory
    {
        ApmMethodHandlerBase Create(IApmContext apmContext);
    }
}