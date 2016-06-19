using System;
using Distracey.Common.Session.OperationCorrelation;

namespace Distracey.Common.Session
{
    /// <summary>
    /// Represents a session context.
    /// </summary>
    public sealed class SessionContext : MarshalByRefObject, IDisposable
    {
        private static ISessionContainer _sessionContainer;

        private SessionContext()
        {
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public static ISession Current
        {
            get
            {
                return _sessionContainer.Current;
            }
        }

        /// <summary>
        /// Gets the current activity identifier
        /// </summary>
        public static Guid CurrentActivityId
        {
            get
            {
                return OperationCorrelationManager.ActivityId;
            }
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

        static SessionContext()
        {
            _sessionContainer = new CallContextSessionContainer();
        }

        /// <summary>
        /// Starts the session context.
        /// </summary>
        public static void StartSession()
        {
            if (_sessionContainer.Current != null)
            {
                StopSession();
            }

            _sessionContainer.Current = new Session();
        }

        /// <summary>
        /// Stops the current session context.
        /// </summary>
        public static void StopSession()
        {
            OperationCorrelationManager.Clear();

            // Clear the current session context
            _sessionContainer.Current = null; 
        }

        public static Guid StartActivity()
        {
            return OperationCorrelationManager.StartLogicalOperation();
        }

        public static void StopActivity()
        {
            OperationCorrelationManager.StopLogicalOperation();
        }

        public void Dispose()
        {
            StopSession();
        }
    }
}
