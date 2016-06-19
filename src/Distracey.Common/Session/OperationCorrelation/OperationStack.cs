using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Distracey.Common.Session.OperationCorrelation
{
    public static class OperationStack
    {
        private const string OperationStackSlot = "Distracey.OperationStack";

        public static IDisposable Push(string operation)
        {
            var parent = CallContext.LogicalGetData(OperationStackSlot) as OperationStackItem;
            var op = new OperationStackItem(parent, operation);
            CallContext.LogicalSetData(OperationStackSlot, op);
            return op;
        }

        public static object Pop()
        {
            var current = CallContext.LogicalGetData(OperationStackSlot) as OperationStackItem;

            if (current != null)
            {
                CallContext.LogicalSetData(OperationStackSlot, current.Parent);
                return current.Operation;
            }
            else
            {
                CallContext.FreeNamedDataSlot(OperationStackSlot);
            }
            return null;
        }

        public static object Peek()
        {
            var top = Top();
            return top != null ? top.Operation : null;
        }

        internal static OperationStackItem Top()
        {
            var top = CallContext.LogicalGetData(OperationStackSlot) as OperationStackItem;
            return top;
        }

        public static IEnumerable<object> Operations()
        {
            var current = Top();
            while (current != null)
            {
                yield return current.Operation;
                current = current.Parent;
            }
        }

        public static int Count
        {
            get
            {
                var top = Top();
                return top == null ? 0 : top.Depth;
            }
        }

        public static IEnumerable<string> OperationStrings()
        {
            foreach (object o in Operations())
            {
                yield return o.ToString();
            }
        }

        public static void Clear()
        {
            CallContext.FreeNamedDataSlot(OperationStackSlot);
        }
    }
}
