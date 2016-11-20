using System;

namespace Distracey.Common.Session.OperationCorrelation
{
    public class OperationCorrelationManager
    {
        private readonly OperationStack _operationStack;

        public OperationCorrelationManager(OperationStack operationStack)
        {
            _operationStack = operationStack;
        }

        /// <summary>
        /// Gets or sets the identity for a global activity.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Guid"/> structure that identifies the global activity.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public Guid ActivityId
        {
            get
            {
                var data = _operationStack.Peek();
                return data != null ? Guid.Parse((string)data) : Guid.Empty;
            }
        }

        /// <summary>
        /// Starts a logical operation on a thread.
        /// </summary>
        public Guid StartLogicalOperation()
        {
            var activityId = Guid.NewGuid();
            _operationStack.Push(activityId.ToString());

            return activityId;
        }

        /// <summary>
        /// Stops the current logical operation.
        /// </summary>
        public Guid StopLogicalOperation()
        {
            var activityId = (string)_operationStack.Pop();
            return Guid.Parse(activityId);
        }

        public void Clear()
        {
            _operationStack.Clear();
        }
    }
}