using System;
using System.Runtime.Remoting.Messaging;

namespace Distracey.Common.Session.SessionIdentifier
{
    public class CallContextSessionIdentifierStorage : ISessionIdentifierStorage
    {
        private const string CurrentSessionIdCacheKey = "distracey::current_session_id";

        public Guid? Current
        {
            get
            {
                return (Guid?)CallContext.LogicalGetData(CurrentSessionIdCacheKey);
            }
            set
            {
                CallContext.LogicalSetData(CurrentSessionIdCacheKey, value);
            }
        }

        public void Clear()
        {
            CallContext.FreeNamedDataSlot(CurrentSessionIdCacheKey);
        }
    }
}
