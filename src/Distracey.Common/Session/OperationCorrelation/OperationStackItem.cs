using System;

namespace Distracey.Common.Session.OperationCorrelation
{
    public class OperationStackItem : IDisposable
    {
        private readonly OperationStackItem _parent;
        private readonly object _operation;
        private readonly Action _dispose;
        private readonly int _depth;
        private bool _disposed;

        internal OperationStackItem(OperationStackItem parentOperation, object operation, Action dispose)
        {
            _parent = parentOperation;
            _operation = operation;
            _dispose = dispose;
            _depth = _parent == null ? 1 : _parent.Depth + 1;
        }

        internal object Operation { get { return _operation; } }
        internal int Depth { get { return _depth; } }

        internal OperationStackItem Parent { get { return _parent; } }

        public override string ToString()
        {
            return _operation != null ? _operation.ToString() : "";
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_disposed) return;

            _dispose();

            _disposed = true;
        }

        #endregion
    }
}
