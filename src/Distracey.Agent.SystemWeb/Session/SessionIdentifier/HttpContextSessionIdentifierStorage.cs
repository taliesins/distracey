using System;
using System.Web;
using Distracey.Common.Session.SessionIdentifier;

namespace Distracey.Agent.SystemWeb.Session.SessionIdentifier
{
    public class HttpContextSessionIdentifierStorage : ISessionIdentifierStorage
    {
        private readonly ISessionIdentifierStorage _innerSessionIdentifierStorage;
        private const string CurrentSessionIdCacheKey = "distracey::current_session_id";

        public HttpContextSessionIdentifierStorage(ISessionIdentifierStorage innerSessionIdentifierStorage)
        {
            _innerSessionIdentifierStorage = innerSessionIdentifierStorage;
        }

        // ASP.Net has thread agilitiy so when SessionContext.StartSession() executes in 
        // begin request event handler, it may be in a different thread so
        // the callcontext might not contain the current profiling SessionContext correctly
        // so on reading of the current SessionContext from HttpContextSessionContainer
        // double check to ensure current SessionContext stored in callcontext
        public Guid? Current
        {
            get
            {
                Guid? sessionId = null;

                if (HttpContext.Current != null)
                {
                    sessionId = HttpContext.Current.Items[CurrentSessionIdCacheKey] as Guid?;
                }

                if (_innerSessionIdentifierStorage != null)
                {
                    _innerSessionIdentifierStorage.Current = sessionId;
                }

                return sessionId;
            }
            set
            {
                if (_innerSessionIdentifierStorage != null)
                {
                    _innerSessionIdentifierStorage.Current = value;
                }

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[CurrentSessionIdCacheKey] = value;
                }
            }
        }

        public void Clear()
        {
            if (_innerSessionIdentifierStorage != null)
            {
                _innerSessionIdentifierStorage.Clear();
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[CurrentSessionIdCacheKey] = null;
            }
        }
    }
}
