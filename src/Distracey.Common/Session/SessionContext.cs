using System;
using Distracey.Common.Session.OperationCorrelation;
using Distracey.Common.Session.SessionIdentifier;

namespace Distracey.Common.Session
{
    /// <summary>
    /// Represents a session context.
    /// </summary>
    public sealed class SessionContext : MarshalByRefObject, IDisposable
    {
        private static Lazy<ISessionContainer> _sessionContainer;
        private static Lazy<OperationCorrelationManager> _operationCorrelationManager;

        static SessionContext()
        {
            _sessionContainer = new Lazy<ISessionContainer>(SessionContainerFactory); 
            _operationCorrelationManager = new Lazy<OperationCorrelationManager>(OperationCorrelationManagerFactory);
        }

        private static ISessionContainer SessionContainerFactory()
        {
            return new InMemorySessionContainer(new CallContextSessionIdentifierStorage(), TimeSpan.FromSeconds(10));
        }

        private static OperationCorrelationManager OperationCorrelationManagerFactory()
        {
            return new OperationCorrelationManager(new OperationStack(new CallContextOperationStackStorage()));
        }

        private SessionContext()
        {
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public static ISession CurrentSession
        {
            get
            {
                if (!_sessionContainer.IsValueCreated)
                {
                    return null;
                }
                return _sessionContainer.Value.Current;
            }
        }

        /// <summary>
        /// Gets the current activity identifier
        /// </summary>
        public static IActivity CurrentActivity
        {
            get
            {
                if (!_operationCorrelationManager.IsValueCreated)
                {
                    return null;
                }
                var activityId = _operationCorrelationManager.Value.ActivityId;
                if (activityId == Guid.Empty)
                {
                    return null;
                }

                return CurrentSession.Activities[activityId];
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ISessionContainer"/>.
        /// </summary>
        public static Lazy<ISessionContainer> SessionContainer
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
        /// Gets or sets the <see cref="ISessionContainer"/>.
        /// </summary>
        public static Lazy<OperationCorrelationManager> OperationCorrelationManager
        {
            get { return _operationCorrelationManager; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _operationCorrelationManager = value;
            }
        }

        /// <summary>
        /// Starts the session context.
        /// </summary>
        public static void StartSession()
        {
            if (_sessionContainer.IsValueCreated && _sessionContainer.Value.Current != null)
            {
                StopSession();
            }

            _sessionContainer.Value.Current = new Session();
        }

        /// <summary>
        /// Stops the current session context.
        /// </summary>
        public static void StopSession()
        {
            if (_operationCorrelationManager.IsValueCreated)
            {
                _operationCorrelationManager.Value.Clear();
            }

            if (_sessionContainer.IsValueCreated)
            {
                // Clear the current session context
                _sessionContainer.Value.Current = null;
            }
        }

        public static IActivity StartActivity()
        {
            var activityId = _operationCorrelationManager.Value.StartLogicalOperation();
            var activity = new Activity(activityId);

            CurrentSession.Activities[activityId] = activity;

            return activity;
        }

        public static IActivity StopActivity()
        {
            if (!_operationCorrelationManager.IsValueCreated)
            {
                return null;
            }
            var activityId = _operationCorrelationManager.Value.StopLogicalOperation();
            return CurrentSession.Activities[activityId];
        }

        public void Dispose()
        {
            StopSession();
        }
    }
}
