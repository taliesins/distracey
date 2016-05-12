using System;
using Distracey.Common.Timer;

namespace Distracey.Common.Message
{
    /// <summary>
    /// Extension methods for populating <see cref="ITimedMessage"/> instances.
    /// </summary>
    public static class TimedMessageExtension
    {
        /// <summary>
        /// Populates relevant properties on the source message.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="timerResult">The timer result.</param>
        /// <returns>The <paramref name="message"/> with populated <see cref="ITimedMessage"/> properties.</returns>
        public static T AsTimedMessage<T>(this T message, TimerResult timerResult)
            where T : ITimedMessage
        {
            message.Offset = timerResult.Offset;
            message.Duration = timerResult.Duration;
            message.StartTime = timerResult.StartTime;

            return message;
        }

        /// <summary>
        /// Populates relevant properties on the source message.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message.</param> 
        /// <param name="offset">The offset.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="startTime">The start time.</param>
        /// <returns>The <paramref name="message"/> with populated <see cref="ITimedMessage"/> properties.</returns>
        public static T AsTimedMessage<T>(this T message, TimeSpan offset, TimeSpan duration, DateTime startTime)
            where T : ITimedMessage
        {
            message.Offset = offset;
            message.Duration = duration;
            message.StartTime = startTime;

            return message;
        }

        /// <summary>
        /// Populates relevant properties on the source message. Duration is defaulted to Zero and StartTime is not set.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message.</param> 
        /// <param name="offset">The offset.</param> 
        /// <returns>The <paramref name="message"/> with populated <see cref="ITimedMessage"/> properties.</returns>
        public static T AsTimedMessage<T>(this T message, TimeSpan offset)
            where T : ITimedMessage
        {
            message.Offset = offset;
            message.Duration = TimeSpan.Zero;

            return message;
        }

        /// <summary>
        /// Populates relevant properties on the source message. Duration is defaulted to Zero.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message.</param> 
        /// <param name="offset">The offset.</param> 
        /// <param name="startTime">The start time.</param>
        /// <returns>The <paramref name="message"/> with populated <see cref="ITimedMessage"/> properties.</returns>
        public static T AsTimedMessage<T>(this T message, TimeSpan offset, DateTime startTime)
            where T : ITimedMessage
        {
            message.Offset = offset;
            message.Duration = TimeSpan.Zero;
            message.StartTime = startTime;

            return message;
        }

        /// <summary>
        /// Populates relevant properties on the source message. StartTime is not set.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="message">The message.</param> 
        /// <param name="offset">The offset.</param>
        /// <param name="duration">The duration.</param> 
        /// <returns>The <paramref name="message"/> with populated <see cref="ITimedMessage"/> properties.</returns>
        public static T AsTimedMessage<T>(this T message, TimeSpan offset, TimeSpan duration)
            where T : ITimedMessage
        {
            message.Offset = offset;
            message.Duration = duration;

            return message;
        }
    }
}
