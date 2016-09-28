namespace Distracey.Common.Session.SessionAudit
{
    /// <summary>
    /// Represents a generic profiling storage.
    /// </summary>
    public interface ISessionAuditStorage
    {
        /// <summary>
        /// Saves a profiling timing session.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/> to be saved.</param>
        void SaveSession(ISession session);
    }
}
