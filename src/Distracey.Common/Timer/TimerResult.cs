using System;

namespace Distracey.Common.Timer
{
    /// <summary>
    /// The result of a <c>Time</c> method call on a <see cref="IExecutionTimer"/>.
    /// </summary>
    public class TimerResult
    {
        /// <summary>
        /// Gets or sets the offset from the beginning of the Http request.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public TimeSpan Offset { get; set; }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime { get; set; }
    }

    /// <summary>
    /// A subtype of <see cref="TimerResult"/> which includes the result of non-void method calls.
    /// </summary>
    /// <typeparam name="T">The type returned by the executed method.</typeparam>
    public class TimerResult<T> : TimerResult
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public T Result { get; set; }
    }
}
