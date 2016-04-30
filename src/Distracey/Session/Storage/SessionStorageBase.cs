using System.Collections.Concurrent;
using System.Threading;

namespace Distracey.Session.Storage
{
    /// <summary>
    /// Asynchronous saving profiling timing sessions with a single-thread-queue worker. 
    /// The worker thread is automatically started when the first item is added.
    /// Override the Save() method for custom saving logic.
    /// 
    /// All methods and properties are thread safe.
    /// </summary>
    /// <remarks></remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public abstract class SessionStorageBase : ISessionStorage
    {
        //TODO: what do we do for a logger?
        //private static readonly slf4net.ILogger Logger = slf4net.LoggerFactory.GetLogger(typeof(SessionStorageBase));

        private readonly ConcurrentQueue<ISession> _sessionQueue = new ConcurrentQueue<ISession>();
        private Thread _workerThread;
        private readonly AutoResetEvent _processWait = new AutoResetEvent(false);
        private readonly ManualResetEvent _entryWait = new ManualResetEvent(true);
        private const string OnQueueOverflowEventMessage = "SessionStorageBase worker queue overflowed";

        /// <summary>
        /// The infinite queue length.
        /// </summary>
        public const int Infinite = -1;

        /// <summary>
        /// Disables the queue, which means, each call to SaveResult saves the result immediately.
        /// </summary>
        public const int Inline = 0;

        /// <summary>
        /// The max length of the internal queue.
        /// Max queue length must be -1 (infinite), 0 (process inline) or a positive number.
        /// </summary>
        public int MaxQueueLength { get; set; }

        /// <summary>
        /// The time the worker thread sleeps.
        /// A long sleep period or infinite can cause the process to live longer than necessary.
        /// </summary>
        public int ThreadSleepMilliseconds { get; set; }

        /// <summary>
        /// Constructs a new <see cref="SessionStorageBase"/>.
        /// </summary>
        protected SessionStorageBase()
        {
            MaxQueueLength = 10000;
            ThreadSleepMilliseconds = 100;
        }

        void ISessionStorage.SaveSession(ISession session)
        {
            SaveSession(session);
        }

        /// <summary>
        /// Saves a profiling timing session.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/> to be saved.</param>
        public void SaveSession(ISession session)
        {
            if (MaxQueueLength == Inline)
            {
                Save(session);
            }
            else if (Count < MaxQueueLength || MaxQueueLength == Infinite)
            {
                Enqueue(session);
                InvokeThreadStart();
            }
            else
            {
                OnQueueOverflow(session);
            }
        }

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        protected int Count
        {
            get
            {
                return _sessionQueue.Count;
            }
        }

        /// <summary>
        /// Saves an <see cref="ISession"/>.
        /// </summary>
        /// <param name="session"></param>
        protected abstract void Save(ISession session);

        /// <summary>
        /// Enqueues a session to internal queue.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/> to be enqueued.</param>
        protected void Enqueue(ISession session)
        {
            _sessionQueue.Enqueue(session);
        }

        /// <summary>
        /// Tries to dequeue a session from internal queue for processing.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/> to be dequeued.</param>
        /// <returns>Returns the dequeued <see cref="ISession"/>.</returns>
        protected bool TryDequeue(out ISession session)
        {
            return _sessionQueue.TryDequeue(out session);
        }

        /// <summary>
        /// What to do on internal queue overflow.
        /// 
        /// By default, it will delay the enqueue of session for at most 5000ms and log exception.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/> being enqueued when internal queue overflow.</param>
        protected virtual void OnQueueOverflow(ISession session)
        {
            // On overflow, never block the main thread running,
            // simply throw away the item at the top of the queue, enqueue the new item and log the event
            // so basically, the queue works like a ring buffer
            ISession temp;
            TryDequeue(out temp);
            Enqueue(session);

            //TODO: log exception once we passing in logger
            //Logger.Error(OnQueueOverflowEventMessage);
        }

        private void InvokeThreadStart()
        {
            lock (_sessionQueue)
            {
                // Kick off thread if not there
                if (_workerThread == null)
                {
                    _workerThread = new Thread(SaveQueuedSessions) { Name = GetType().Name };
                    _workerThread.Start();
                }

                // Signal process to continue
                _processWait.Set();
            }
        }

        private void SaveQueuedSessions()
        {
            do
            {
                // Suspend for a while
                _processWait.WaitOne(ThreadSleepMilliseconds, exitContext: false);

                // Upgrade to foreground thread
                Thread.CurrentThread.IsBackground = false;

                // set null the current session bound to the running thread to release the memory
                SessionContext.Stop();

                // Save all the queued sessions
                ISession session;
                while (TryDequeue(out session))
                {
                    Save(session);

                    // Signal waiting threads to continue
                    _entryWait.Set();
                }

                // Downgrade to background thread while waiting
                Thread.CurrentThread.IsBackground = true;

            } while (true);
        }
    }
}
