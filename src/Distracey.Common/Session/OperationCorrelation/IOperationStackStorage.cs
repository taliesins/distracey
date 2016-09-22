namespace Distracey.Common.Session.OperationCorrelation
{
    public interface IOperationStackStorage
    {
        void Clear();
        OperationStackItem Current { get; set; }
    }
}