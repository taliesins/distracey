using System;
using System.Web;
using Distracey.Common.Session;

namespace Distracey.Agent.SystemWeb.Session
{
    public class HttpContextSessionContainerHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PostAcquireRequestState += ContextOnPostAcquireRequestState;
            context.PostReleaseRequestState += ContextOnPostReleaseRequestState;
        }

        private static void ContextOnPostAcquireRequestState(object sender, EventArgs eventArgs)
        {
            SessionContext.StartSession();
        }

        private void ContextOnPostReleaseRequestState(object sender, EventArgs eventArgs)
        {
            SessionContext.StopSession();
        }

        public void Dispose()
        {
        } 
    }
}
