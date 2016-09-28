using System;
using System.Collections.Concurrent;
using System.Linq;
using Distracey.Common.Session.SessionAudit;
using Distracey.Common.Session.SessionIdentifier;

namespace Distracey.Common.Session
{
    /// <summary>
    /// The default CallContext based <see cref="ISessionContainer"/> implementation.
    /// </summary>
    public class InMemorySessionContainer : ISessionContainer
    {
        private readonly ISessionIdentifierStorage _sessionIdentifierStorage;
        private readonly ISessionAuditStorage _sessionAuditStorage;
        private readonly ConcurrentDictionary<Guid, WeakReference> _sessionStore;
        private readonly System.Threading.Timer _cleanUpSessionStoreTimer;

        public InMemorySessionContainer(ISessionIdentifierStorage sessionIdentifierStorage, ISessionAuditStorage sessionAuditStorage, TimeSpan cleanupPeriod)
        {
            _sessionIdentifierStorage = sessionIdentifierStorage;
            _sessionAuditStorage = sessionAuditStorage;
            _sessionStore = new ConcurrentDictionary<Guid, WeakReference>();
            _cleanUpSessionStoreTimer = new System.Threading.Timer(CleanUpSessionStoreTimerCallback, null, TimeSpan.Zero, cleanupPeriod);
        }

        /// <summary>
        /// Gets or sets the current SessionContext.
        /// </summary>
        public ISession Current
        {
            get
            {
                var obj = _sessionIdentifierStorage.Current;
                if (obj == null)
                {
                    return null;
                }

                var sessionId = (Guid?)obj;
                WeakReference wrapper;
                if (!_sessionStore.TryGetValue(sessionId.Value, out wrapper) || wrapper == null || !wrapper.IsAlive)
                {
                    return null;
                }

                return wrapper.Target as ISession;
            }
            set
            {
                if (value == null)
                {
                    var sessionId = _sessionIdentifierStorage.Current;
                    if (sessionId != null)
                    {
                        WeakReference wrapper;
                        _sessionStore.TryRemove(sessionId.Value, out wrapper);

                        SaveSession(wrapper);
                        _sessionIdentifierStorage.Clear();
                    }

                    return;
                }

                _sessionStore.TryAdd(value.SessionId, new WeakReference(value));
                _sessionIdentifierStorage.Current = value.SessionId;
            }
        }

        private void CleanUpSessionStoreTimerCallback(object state)
        {
            WeakReference wrapper;

            // search for keys to remove
            var keysToRemove = _sessionStore
                .Select(item => item.Key)
                .ToList()
                .Where(key => _sessionStore.TryGetValue(key, out wrapper) && !wrapper.IsAlive)
                .ToList();

            // remove
            foreach (var key in keysToRemove)
            {
                _sessionStore.TryRemove(key, out wrapper);
                SaveSession(wrapper);
            }
        }

        private void SaveSession(WeakReference wrapper)
        {
            if (wrapper != null)
            {
                var session = wrapper.Target as ISession;
                if (session != null)
                {
                    _sessionAuditStorage.SaveSession(session);
                }
            }
        }
    }
}