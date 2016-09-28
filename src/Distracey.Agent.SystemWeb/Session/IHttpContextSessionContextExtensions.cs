using System;
using System.Web;
using Distracey.Agent.SystemWeb.Session.OperationCorrelation;
using Distracey.Agent.SystemWeb.Session.SessionIdentifier;
using Distracey.Common.Session;
using Distracey.Common.Session.OperationCorrelation;
using Distracey.Common.Session.SessionAudit;
using Distracey.Common.Session.SessionIdentifier;

namespace Distracey.Agent.SystemWeb.Session
{
    public static class IHttpContextSessionContextExtensions
    {
        public static void UseHttpContextSessionContext()
        {
            //SessionContext.OperationCorrelationManager = new Lazy<OperationCorrelationManager>(OperationCorrelationManagerFactory);
            //SessionContext.SessionContainer = new Lazy<ISessionContainer>(SessionContainerFactory);
            HttpApplication.RegisterModule(typeof(HttpContextSessionContainerHttpModule));
        }

        private static ISessionContainer SessionContainerFactory()
        {
            return new InMemorySessionContainer(new HttpContextSessionIdentifierStorage(new CallContextSessionIdentifierStorage()), new SessionAuditStorageLogger(), TimeSpan.FromSeconds(10));
        }

        private static OperationCorrelationManager OperationCorrelationManagerFactory()
        {
            var httpContextOperationCorrelationManager = new OperationCorrelationManager(new OperationStack(new HttpContextOperationStackStorage()));
            return httpContextOperationCorrelationManager;
        }
    }
}