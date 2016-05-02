namespace Distracey.Common.Session.Storage
{
    /// <summary>
    /// A <see cref="ISessionStorage"/> implementation which performs no operation.
    /// </summary>
    public class NullSessionStorage : SessionStorageBase
    {
        /// <summary>
        /// Saves an <see cref="IApmContext"/>.
        /// </summary>
        /// <param name="session"></param>
        protected override void Save(ISession session)
        {
            // no operation
        }
    }
}
