using System.Runtime.Remoting.Messaging;

namespace Distracey.Common.Session.OperationCorrelation
{
    public class CallContextOperationStackStorage : IOperationStackStorage
    {
        private const string OperationStackSlot = "Distracey.OperationStack";

        public OperationStackItem Current
        {
            get
            {
                return (OperationStackItem)CallContext.LogicalGetData(OperationStackSlot);
            }
            set
            {
                CallContext.LogicalSetData(OperationStackSlot, value);
            }
        }
        
        public void Clear()
        {
            CallContext.FreeNamedDataSlot(OperationStackSlot);
        }
    }
}
