using System;
using Distracey.Session.Storage;

namespace Distracey.Session
{
    /// <summary>
    /// Represents a SessionContext.
    /// </summary>
    public sealed class SessionContext : MarshalByRefObject, IDisposable
    {
        private static ISessionContainer _sessionContainer;
        private static ISessionStorage _sessionStorage;

        public SessionContext()
        {
            Start();
        }

        /// <summary>
        /// Gets the current SessionContext.
        /// </summary>
        public static ISession Current
        {
            get { return _sessionContainer.Current; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ISessionContainer"/>.
        /// </summary>
        public static ISessionContainer SessionContainer
        {
            get { return _sessionContainer; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _sessionContainer = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ISessionStorage"/>.
        /// </summary>
        public static ISessionStorage SessionStorage
        {
            get { return _sessionStorage; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _sessionStorage = value;
            }
        }

        static SessionContext()
        {
            // by default, use CallContextSessionContainer
            _sessionContainer = new CallContextSessionContainer();

            // by default, use JsonProfilingStorage
            _sessionStorage = new NullSessionStorage();
        }

        /// <summary>
        /// Starts the SessionContext.
        /// </summary>
        public static void Start()
        {
            if (_sessionContainer.Current == null)
            {
                _sessionContainer.Current = new Session();
            }
        }

        /// <summary>
        /// Stops the current SessionContext.
        /// </summary>
        /// When true, discards the entire SessionContext.
        /// </param>
        public static void Stop()
        {
            // Clear the current SessionContext
            _sessionContainer.Current = null;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
