using System;

namespace Distracey.Common.Session.OperationCorrelation
{
    internal static class OperationCorrelationManager
    {
        /// <summary>
        /// Gets or sets the identity for a global activity.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Guid"/> structure that identifies the global activity.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public static Guid ActivityId
        {
            get
            {
                var data = OperationStack.Peek();
                return data != null ? (Guid) data : Guid.Empty;
            }
        }

        /// <summary>
        /// Starts a logical operation on a thread.
        /// </summary>
        public static Guid StartLogicalOperation()
        {
            var activityId = Guid.NewGuid();
            OperationStack.Push(activityId.ToString());

            return activityId;
        }

        /// <summary>
        /// Stops the current logical operation.
        /// </summary>
        public static void StopLogicalOperation()
        {
            OperationStack.Pop();
        }

        public static void Clear()
        {
            OperationStack.Clear();
        }
    }
}