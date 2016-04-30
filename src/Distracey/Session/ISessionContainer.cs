namespace Distracey.Session
{
    /// <summary>
    /// Represents a container of the current SessionContext.
    /// </summary>
    public interface ISessionContainer
    {
        /// <summary>
        /// Gets or sets the current SessionContext.
        /// </summary>
        ISession Current { get; set; }
    }
}
