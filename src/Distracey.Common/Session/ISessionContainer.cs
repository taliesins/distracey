namespace Distracey.Common.Session
{
    /// <summary>
    /// Represents a container of the current Session.
    /// </summary>
    public interface ISessionContainer
    {
        /// <summary>
        /// Gets or sets the current Session.
        /// </summary>
        ISession Current { get; set; }
    }
}
