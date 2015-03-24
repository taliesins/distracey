namespace Distracey
{
    public interface IApmMethodHandlerFactory
    {
        ApmMethodHandlerBase Create(IApmContext apmContext);
    }
}