using System;
using System.Web;
using Distracey.Common.Session;
using Distracey.Common.Session.SessionIdentifier;

namespace Distracey.Agent.SystemWeb.Session
{
    /// <summary>
    /// A IProfilingSessionContainer implementation
    /// which stores current profiling SessionContext in both HttpContext.Items and CallContext,
    /// So that SessionContext.Current could work consistently in web application.
    /// </summary>
    public class HttpContextSessionContainer : ISessionContainer
    {
        private readonly ISessionIdentifierStorage _sessionIdentifierStorage;
        private readonly ISessionContainer _innerSessionContainer;

        public HttpContextSessionContainer(ISessionIdentifierStorage sessionIdentifierStorage, ISessionContainer innerSessionContainer)
        {
            _sessionIdentifierStorage = sessionIdentifierStorage;
            _innerSessionContainer = innerSessionContainer;
        }

        /// <summary>
        /// Gets or sets the current SessionContext.
        /// </summary>
        public ISession Current 
        {
            get
            {
                // ASP.Net has thread agilitiy so when SessionContext.StartSession() executes in 
                // begin request event handler, it may be in a different thread so
                // the callcontext might not contain the current profiling SessionContext correctly
                // so on reading of the current SessionContext from HttpContextSessionContainer
                // double check to ensure current SessionContext stored in callcontext

                var obj = _sessionIdentifierStorage.Current;
                if (obj == null)
                {
                    if (_innerSessionContainer != null)
                    {
                        _innerSessionContainer.Current = null;
                    }
                    return null;
                }

                var sessionId = (Guid?)obj;

                var sessionContext = HttpContext.Current.Items[sessionId.Value] as ISession;

                if (_innerSessionContainer != null)
                {
                    if (sessionContext == null)
                    {
                        _innerSessionContainer.Current = null;
                    }
                    else if (_innerSessionContainer.Current == null || _innerSessionContainer.Current != sessionContext)
                    {
                        _innerSessionContainer.Current = sessionContext;
                    }
                }

                return sessionContext;
            }
            set
            {
                if (_innerSessionContainer != null && _innerSessionContainer.Current != value)
                {
                    _innerSessionContainer.Current = value;
                }

                // Cache current profiler SessionContext in HttpContext.Items if HttpContext accessible

                if (value == null)
                {
                    var obj = _sessionIdentifierStorage.Current;
                    if (obj != null)
                    {
                        var sessionId = (Guid?)obj;
                        HttpContext.Current.Items.Remove(sessionId.Value);
                    }

                    _sessionIdentifierStorage.Clear();
                    return;
                }

                HttpContext.Current.Items[value.SessionId] = value;
                _sessionIdentifierStorage.Current = value.SessionId;
            } 
        }
    }
}
