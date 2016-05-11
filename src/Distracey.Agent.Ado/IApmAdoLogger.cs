using System.Threading.Tasks;
using Distracey.Common;

namespace Distracey.Agent.Ado
{
    public interface IApmAdoLogger : IEventLogger
    {
        Task OnDbConnectionOpenedMessage(Task<ApmEvent<DbConnectionOpenedMessage>> task);
        Task OnDbConnectionClosedMessage(Task<ApmEvent<DbConnectionClosedMessage>> task);

        Task OnDbDataAdapterFinishedMessage(Task<ApmEvent<DbDataAdapterFinishedMessage>> task);
        Task OnDbDataAdapterStartedMessage(Task<ApmEvent<DbDataAdapterStartedMessage>> task);

        Task OnDbTransactionFinishedMessage(Task<ApmEvent<DbTransactionFinishedMessage>> task);
        Task OnDbTransactionStartedMessage(Task<ApmEvent<DbTransactionStartedMessage>> task);

        Task OnExecuteDbDataReaderAsyncFinishedMessage(Task<ApmEvent<ExecuteDbDataReaderAsyncFinishedMessage>> task);
        Task OnExecuteDbDataReaderAsyncStartedMessage(Task<ApmEvent<ExecuteDbDataReaderAsyncStartedMessage>> task);

        Task OnExecuteDbDataReaderFinishedMessage(Task<ApmEvent<ExecuteDbDataReaderFinishedMessage>> task);
        Task OnExecuteDbDataReaderStartedMessage(Task<ApmEvent<ExecuteDbDataReaderStartedMessage>> task);

        Task OnExecuteNonQueryAsyncFinishedMessage(Task<ApmEvent<ExecuteNonQueryAsyncFinishedMessage>> task);
        Task OnExecuteNonQueryAsyncStartedMessage(Task<ApmEvent<ExecuteNonQueryAsyncStartedMessage>> task);

        Task OnExecuteNonQueryFinishedMessage(Task<ApmEvent<ExecuteNonQueryFinishedMessage>> task);
        Task OnExecuteNonQueryStartedMessage(Task<ApmEvent<ExecuteNonQueryStartedMessage>> task);

        Task OnExecuteScalarAsyncFinishedMessage(Task<ApmEvent<ExecuteScalarAsyncFinishedMessage>> task);
        Task OnExecuteScalarAsyncStartedMessage(Task<ApmEvent<ExecuteScalarAsyncStartedMessage>> task);

        Task OnExecuteScalarFinishedMessage(Task<ApmEvent<ExecuteScalarFinishedMessage>> task);
        Task OnExecuteScalarStartedMessage(Task<ApmEvent<ExecuteScalarStartedMessage>> task);
    }
}
