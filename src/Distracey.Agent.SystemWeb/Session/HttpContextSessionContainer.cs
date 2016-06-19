﻿using System.Web;
using Distracey.Common.Session;

namespace Distracey.Agent.SystemWeb.Session
{
    /// <summary>
    /// A IProfilingSessionContainer implementation
    /// which stores current profiling SessionContext in both HttpContext.Items and CallContext,
    /// So that SessionContext.Current could work consistently in web application.
    /// </summary>
    public class HttpContextSessionContainer : ISessionContainer
    {
        private const string CurrentSessionIdCacheKey = "distracey::current_session";
        private readonly ISessionContainer _innerSessionContainer;

        public HttpContextSessionContainer(ISessionContainer innerSessionContainer)
        {
            _innerSessionContainer = innerSessionContainer;
        }

        /// <summary>
        /// Gets or sets the current SessionContext.
        /// </summary>
        public ISession Current 
        {
            get
            {
                // Try to get current profiling SessionContext from HttpContext.Items first
                var sessionContext = HttpContext.Current == null ? null : HttpContext.Current.Items[CurrentSessionIdCacheKey] as ISession;

                // when SessionContext.StartSession() executes in begin request event handler in a different thread
                // the callcontext might not contain the current profiling SessionContext correctly
                // so on reading of the current SessionContext from HttpContextSessionContainer
                // double check to ensure current SessionContext stored in callcontext
                if (sessionContext != null && _innerSessionContainer != null && _innerSessionContainer.Current == null)
                {
                    _innerSessionContainer.Current = sessionContext;
                }

                //We dont have a session so try to get it out of other session containers
                if (sessionContext == null && _innerSessionContainer != null && _innerSessionContainer.Current != null)
                {
                    return _innerSessionContainer.Current;
                }

                return sessionContext;
            }
            set
            {
                if (_innerSessionContainer != null)
                {
                    _innerSessionContainer.Current = value;
                }

                // Cache current profiler SessionContext in HttpContext.Items if HttpContext accessible

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[CurrentSessionIdCacheKey] = value;
                }
            } 
        }
    }
}
