using System;
using System.Collections.Generic;

namespace Distracey.Common.Session.OperationCorrelation
{
    public class OperationStack
    {
        private readonly IOperationStackStorage _operationStackStorage;

        public OperationStack(IOperationStackStorage operationStackStorage)
        {
            _operationStackStorage = operationStackStorage;
        }

        public IDisposable Push(string operation)
        {
            var parent = _operationStackStorage.Current;
            var op = new OperationStackItem(parent, operation, () => { Pop(); });
            _operationStackStorage.Current = op;
            return op;
        }

        public object Pop()
        {
            var current = _operationStackStorage.Current;

            if (current != null)
            {
                _operationStackStorage.Current = current.Parent;
                return current.Operation;
            }
            else
            {
                Clear();
            }
            return null;
        }

        public object Peek()
        {
            var top = Top();
            return top != null ? top.Operation : null;
        }

        public OperationStackItem Top()
        {
            var top = _operationStackStorage.Current;
            return top;
        }

        public IEnumerable<object> Operations()
        {
            var current = Top();
            while (current != null)
            {
                yield return current.Operation;
                current = current.Parent;
            }
        }

        public int Count
        {
            get
            {
                var top = Top();
                return top == null ? 0 : top.Depth;
            }
        }

        public IEnumerable<string> OperationStrings()
        {
            foreach (object o in Operations())
            {
                yield return o.ToString();
            }
        }

        public void Clear()
        {
            _operationStackStorage.Clear();
        }
    }
}
