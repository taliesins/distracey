using System;

namespace Distracey.Common.Message
{
    /// <summary>
    /// The definition of a message which is published with execution timing information.
    /// </summary>
    public interface ITimedMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the offset from the Http request start.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        TimeSpan Offset { get; set; }

        /// <summary>
        /// Gets or sets the duration of the executed method.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        DateTime StartTime { get; set; }
    }
}
