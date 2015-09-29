namespace Distracey.Web.HttpClient
{
    public interface IApmHttpClientDelegatingHandlerFactory
    {
        ApmHttpClientDelegatingHandlerBase Create(IApmContext apmContext);
    }
}