using System.Web;

namespace Distracey.Agent.SystemWeb.Session
{
    public static class IHttpContextSessionContextExtensions
    {
        public static void UseHttpContextSessionContext()
        {
            HttpApplication.RegisterModule(typeof(HttpContextSessionContainerHttpModule));
        }
    }
}