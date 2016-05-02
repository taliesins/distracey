using System.Collections.Generic;

namespace Distracey.Common.Helpers
{
    /// <summary>
    /// Represents a generic circular buffer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICircularBuffer<T> : IEnumerable<T>
    {
        /// <summary>
        /// Adds an item to the buffer.
        /// </summary>
        /// <param name="item"></param>
        void Add(T item);
    }
}
