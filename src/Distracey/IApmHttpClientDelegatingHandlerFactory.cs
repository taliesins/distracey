namespace Distracey
{
    public interface IApmHttpClientDelegatingHandlerFactory
    {
        ApmHttpClientDelegatingHandlerBase Create(IApmContext apmContext);
    }
}