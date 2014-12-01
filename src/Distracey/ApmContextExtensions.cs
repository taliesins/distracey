namespace Distracey
{
    public static class ApmContextExtensions
    {
        public static ApmHttpClientDelegatingHandlerBase GetDelegatingHandler(this IApmContext apmContext)
        {
            return ApmContext.GetDelegatingHandler(apmContext);
        }
    }
}