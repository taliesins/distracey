using System;
using System.Web;
using Distracey.Agent.SystemWeb.Session.OperationCorrelation;
using Distracey.Common.Session;
using Distracey.Common.Session.OperationCorrelation;

namespace Distracey.Agent.SystemWeb.Session
{
    public static class IHttpContextSessionContextExtensions
    {
        public static void UseHttpContextSessionContext()
        {
            SessionContext.SessionContainer = new Lazy<ISessionContainer>(SessionContainerFactory);
            SessionContext.OperationCorrelationManager = new Lazy<OperationCorrelationManager>(OperationCorrelationManagerFactory);

            HttpApplication.RegisterModule(typeof(HttpContextSessionContainerHttpModule));
        }

        private static ISessionContainer SessionContainerFactory()
        {
            var httpContextSessionContainer = new HttpContextSessionContainer();
            return httpContextSessionContainer;
        }

        private static OperationCorrelationManager OperationCorrelationManagerFactory()
        {
            var httpContextOperationCorrelationManager = new OperationCorrelationManager(new OperationStack(new HttpContextOperationStackStorage()));
            return httpContextOperationCorrelationManager;
        }
    }
}