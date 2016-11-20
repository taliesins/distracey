using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Distracey.Log4Net;
using Distracey.NoOperation;
using log4net;

namespace Distracey.Examples.ServiceDepthOne
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configure(ApplicationStart);
        }

        public static void ApplicationStart(HttpConfiguration httpConfiguration)
        {
            GlobalContext.Properties["assemblyVersion"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var applicationName = typeof(WebApiApplication).Assembly.GetName().Name;
            var logger = log4net.LogManager.GetLogger("WebApiApmLogger");

            httpConfiguration.AddNoOperationApm(applicationName);
            httpConfiguration.AddLog4NetApm(applicationName, logger);
            //httpConfiguration.AddPerformanceCountersApm(applicationName, addResponseHeaders);
        }
    }
}