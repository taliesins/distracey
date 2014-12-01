using System;

namespace Distracey
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApmAttribute : Attribute
    {
        /// <summary>
        /// Gets the event name.
        /// </summary>
        public virtual string EventName { get; private set; }

        public ApmAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
}