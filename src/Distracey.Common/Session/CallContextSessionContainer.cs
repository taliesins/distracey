using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Distracey.Common.Helpers;

namespace Distracey.Common.Session
{
    /// <summary>
    /// The default CallContext based <see cref="ISessionContainer"/> implementation.
    /// </summary>
    public class CallContextSessionContainer : ISessionContainer
    {
        private static readonly ConcurrentDictionary<ShortGuid, WeakReference> SessionStore = new ConcurrentDictionary<ShortGuid, WeakReference>();
        private const string CurrentSessionIdCacheKey = "distracey::current_session_id";
        private static readonly Timer CleanUpSessionStoreTimer = new Timer(CleanUpSessionStoreTimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

        /// <summary>
        /// Sets the periodically clean-up profiling SessionContext store period in milliseconds.
        /// </summary>
        /// <param name="milliseconds"></param>
        public static void SetCleanUpSessionStorePeriod(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                throw new ArgumentException("milliseconds should > 0");
            }

            CleanUpSessionStoreTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Gets or sets the current SessionContext.
        /// </summary>
        public ISession Current
        {
            get
            {
                var obj = CallContext.GetData(CurrentSessionIdCacheKey);
                if (obj == null)
                {
                    return null;
                }

                var sessionId = (ShortGuid?)obj;
                WeakReference wrapper;
                if (!SessionStore.TryGetValue(sessionId.Value, out wrapper) || wrapper == null || !wrapper.IsAlive)
                {
                    return null;
                }

                return wrapper.Target as ISession;
            }
            set
            {
                if (value == null)
                {
                    var obj = CallContext.GetData(CurrentSessionIdCacheKey);
                    if (obj != null)
                    {
                        var sessionId = (ShortGuid?)obj;
                        WeakReference temp;
                        SessionStore.TryRemove(sessionId.Value, out temp);
                    }

                    CallContext.LogicalSetData(CurrentSessionIdCacheKey, null);
                    return;
                }

                SessionStore.TryAdd(value.SessionId, new WeakReference(value));
                CallContext.LogicalSetData(CurrentSessionIdCacheKey, (ShortGuid?)value.SessionId);
            }
        }

        ISession ISessionContainer.Current
        {
            get { return Current; }
            set { Current = value; }
        }

        private static void CleanUpSessionStoreTimerCallback(object state)
        {
            WeakReference wrapper;

            // search for keys to remove
            var keysToRemove = SessionStore
                .Select(item => item.Key)
                .ToList()
                .Where(key => SessionStore.TryGetValue(key, out wrapper) && !wrapper.IsAlive)
                .ToList();

            // remove
            foreach (var key in keysToRemove)
            {
                SessionStore.TryRemove(key, out wrapper);
            }
        }
    }
}
