using System.Web;
using Distracey.Common.Session;

namespace Distracey.Agent.SystemWeb.Session
{
    public static class ISessionContainerExtensions
    {
        public static void UseHttpContextSessionContainer(this ISessionContainer sessionContainer)
        {
            if (sessionContainer is HttpContextSessionContainer)
            {
                return;
            }

            SessionContext.SessionContainer = new HttpContextSessionContainer(null);

            HttpApplication.RegisterModule(typeof(HttpContextSessionContainerHttpModule));
        }
    }
}