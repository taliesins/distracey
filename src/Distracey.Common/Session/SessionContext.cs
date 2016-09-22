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
        public static ISession Current
        {
            get
            {
                return _sessionContainer.Value.Current;
            }
        }

        /// <summary>
        /// Gets the current activity identifier
        /// </summary>
        public static Guid CurrentActivityId
        {
            get
            {
                return _operationCorrelationManager.Value.ActivityId;
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
            if (_sessionContainer.Value.Current != null)
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
            _operationCorrelationManager.Value.Clear();

            // Clear the current session context
            _sessionContainer.Value.Current = null; 
        }

        /// <summary>
        /// CS
        /// </summary>
        /// <returns></returns>
        public static Guid StartActivityClientSend()
        {
            return _operationCorrelationManager.Value.StartLogicalOperation();
        }

        /// <summary>
        /// SR
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="traceId"></param>
        /// <param name="sampled"></param>
        /// <param name="flags"></param>
        public static void StartActivityServerReceived(string activityId, string traceId, string sampled, string flags)
        {
            _operationCorrelationManager.Value.StartLogicalOperation(activityId);
            var session = Current;
            session.TraceId = traceId;
            session.Sampled = sampled;
            session.Flags = flags;
        }

        /// <summary>
        /// SS or CR
        /// </summary>
        public static void StopActivity()
        {
            _operationCorrelationManager.Value.StopLogicalOperation();
        }

        public void Dispose()
        {
            StopSession();
        }
    }
}
