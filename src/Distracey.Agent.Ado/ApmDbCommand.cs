using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Distracey.Common;
using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class ApmDbCommand : DbCommand
    {
        public ApmDbCommand(DbCommand innerCommand)
        {
            InnerCommand = innerCommand; 
        }

        public ApmDbCommand(DbCommand innerCommand, ApmDbConnection connection) 
            : this(innerCommand)
        {
            InnerConnection = connection;
        }

        public DbCommand InnerCommand { get; set; }

        public ApmDbConnection InnerConnection { get; set; } 

        public override string CommandText
        {
            get { return InnerCommand.CommandText; }
            set { InnerCommand.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return InnerCommand.CommandTimeout; }
            set { InnerCommand.CommandTimeout = value; }
        }

        public override CommandType CommandType
        {
            get { return InnerCommand.CommandType; }
            set { InnerCommand.CommandType = value; }
        }

        public override bool DesignTimeVisible
        {
            get { return InnerCommand.DesignTimeVisible; }
            set { InnerCommand.DesignTimeVisible = value; }
        }

        public override ISite Site
        {
            get { return InnerCommand.Site; }
            set { InnerCommand.Site = value; }
        } 

        public override UpdateRowSource UpdatedRowSource
        {
            get { return InnerCommand.UpdatedRowSource; }
            set { InnerCommand.UpdatedRowSource = value; }
        }

        public bool BindByName
        {
            get
            {
                var property = InnerCommand.GetType().GetProperty("BindByName");
                if (property == null)
                {
                    return false;
                }

                return (bool)property.GetValue(InnerCommand, null);
            }

            set
            {
                var property = InnerCommand.GetType().GetProperty("BindByName");
                if (property != null)
                {
                    property.SetValue(InnerCommand, value, null);
                } 
            }
        }

        public DbCommand Inner
        {
            get { return InnerCommand; }
        } 

        protected override DbParameterCollection DbParameterCollection
        {
            get { return InnerCommand.Parameters; }
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return InnerConnection;
            }

            set
            {
                InnerConnection = value as ApmDbConnection;
                if (InnerConnection != null)
                {
                    InnerCommand.Connection = InnerConnection.InnerConnection;
                }
                else
                { 
                    InnerConnection = new ApmDbConnection(value);
                    InnerCommand.Connection = InnerConnection.InnerConnection; 
                }
            }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return InnerCommand.Transaction == null ? null : new ApmDbTransaction(InnerCommand.Transaction, InnerConnection);
            }

            set
            {
                var transaction = value as ApmDbTransaction;
                InnerCommand.Transaction = (transaction != null) ? transaction.InnerTransaction : value;
            }
        }

        public override void Cancel()
        {
            InnerCommand.Cancel();
        }

        public override void Prepare()
        {
            InnerCommand.Prepare();
        }

        public override int ExecuteNonQuery()
        {
            int recordsAffected;

            var commandHash = CommandText.GetHashCode();
            var apmContext = ApmContext.GetContext(string.Format("DbCommand.ExecuteNonQuery.{0}", commandHash));
            var activityId = ApmContext.StartActivityClientSend(apmContext);
            var commandId = activityId;
            try
            {
                LogStartOfExecuteNonQuery(apmContext, commandId, CommandText);

                try
                {
                    recordsAffected = InnerCommand.ExecuteNonQuery();
                    LogStopOfExecuteNonQuery(apmContext, commandId, CommandText, recordsAffected, null);
                }
                catch (Exception exception)
                {
                    LogStopOfExecuteNonQuery(apmContext, commandId, CommandText, 0, exception);
                    throw;
                }
            }
            finally
            {
                ApmContext.StopActivityClientReceived();
            }

            return recordsAffected;
        }

        private void LogStartOfExecuteNonQuery(IApmContext apmContext, ShortGuid commandId, string commandText)
        {
            var executeNonQueryStartedMessage = new ExecuteNonQueryStartedMessage
            {
                CommandId = commandId,
                CommandText = commandText
            }.AsMessage(apmContext);

            executeNonQueryStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfExecuteNonQuery(IApmContext apmContext, ShortGuid commandId, string commandText, int recordsEffected, Exception exception)
        {
            var executeNonQueryFinishedMessage = new ExecuteNonQueryFinishedMessage
            {
                CommandId = commandId,
                CommandText = commandText,
                RecordsEffected = recordsEffected,
                Exception = exception
            }.AsMessage(apmContext);

            executeNonQueryFinishedMessage.PublishMessage(apmContext, this);
        }

        public override object ExecuteScalar()
        {
            object result;

            var commandHash = CommandText.GetHashCode();
            var apmContext = ApmContext.GetContext(string.Format("DbCommand.ExecuteScalar.{0}", commandHash));
            var activityId = ApmContext.StartActivityClientSend(apmContext);
            var commandId = activityId;
            try
            {
                LogStartOfExecuteScalar(apmContext, commandId, CommandText);

                try
                {
                    result = InnerCommand.ExecuteScalar();
                    LogStopOfExecuteScalar(apmContext, commandId, CommandText, null);
                }
                catch (Exception exception)
                {
                    LogStopOfExecuteScalar(apmContext, commandId, CommandText, exception);
                    throw;
                }
            }
            finally
            {
                ApmContext.StopActivityClientReceived();
            }

            return result;
        }

        private void LogStartOfExecuteScalar(IApmContext apmContext, ShortGuid commandId, string commandText)
        {
            var executeScalarStartedMessage = new ExecuteScalarStartedMessage
            {
                CommandId = commandId,
                CommandText = commandText
            }.AsMessage(apmContext);

            executeScalarStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfExecuteScalar(IApmContext apmContext, ShortGuid commandId, string commandText, Exception exception)
        {
            var executeScalarFinishedMessage = new ExecuteScalarFinishedMessage
            {
                CommandId = commandId,
                CommandText = commandText,
                Exception = exception
            }.AsMessage(apmContext);

            executeScalarFinishedMessage.PublishMessage(apmContext, this);
        }

        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            object result;

            var commandHash = CommandText.GetHashCode();
            var apmContext = ApmContext.GetContext(string.Format("DbCommand.ExecuteScalarAsync.{0}", commandHash));
            var activityId = ApmContext.StartActivityClientSend(apmContext);
            var commandId = activityId;
            try
            {
                LogStartOfExecuteScalarAsync(apmContext, commandId, CommandText);

                try
                {
                    result = await InnerCommand.ExecuteScalarAsync(cancellationToken);
                    LogStopOfExecuteScalarAsync(apmContext, commandId, CommandText, null);
                }
                catch (Exception exception)
                {
                    LogStopOfExecuteScalarAsync(apmContext, commandId, CommandText, exception);
                    throw;
                }
            }
            finally
            {
                ApmContext.StopActivityClientReceived();
            }

            return result;
        }

        private void LogStartOfExecuteScalarAsync(IApmContext apmContext, ShortGuid commandId, string commandText)
        {
            var executeScalarAsyncStartedMessage = new ExecuteScalarAsyncStartedMessage
            {
                CommandId = commandId,
                CommandText = commandText
            }.AsMessage(apmContext);

            executeScalarAsyncStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfExecuteScalarAsync(IApmContext apmContext, ShortGuid commandId, string commandText, Exception exception)
        {
            var executeScalarAsyncFinishedMessage = new ExecuteScalarAsyncFinishedMessage
            {
                CommandId = commandId,
                CommandText = commandText,
                Exception = exception
            }.AsMessage(apmContext);

            executeScalarAsyncFinishedMessage.PublishMessage(apmContext, this);
        }

        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            int recordsEffected;

            var commandHash = CommandText.GetHashCode();
            var apmContext = ApmContext.GetContext(string.Format("DbCommand.ExecuteNonQueryAsync.{0}", commandHash));
            var activityId = ApmContext.StartActivityClientSend(apmContext);
            var commandId = activityId;

            try
            {
                LogStartOfExecuteNonQueryAsync(apmContext, commandId, CommandText);

                try
                {
                    recordsEffected = await InnerCommand.ExecuteNonQueryAsync(cancellationToken);
                    LogStopOfExecuteNonQueryAsync(apmContext, commandId, CommandText, recordsEffected, null);
                }
                catch (Exception exception)
                {
                    LogStopOfExecuteNonQueryAsync(apmContext, commandId, CommandText, 0, exception);
                    throw;
                }
            }
            finally
            {
                ApmContext.StopActivityClientReceived();
            }

            return recordsEffected;
        }

        private void LogStartOfExecuteNonQueryAsync(IApmContext apmContext, ShortGuid commandId, string commandText)
        {
            var executeNonQueryAsyncStartedMessage = new ExecuteNonQueryAsyncStartedMessage
            {
                CommandId = commandId,
                CommandText = commandText
            }.AsMessage(apmContext);

            executeNonQueryAsyncStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfExecuteNonQueryAsync(IApmContext apmContext, ShortGuid commandId, string commandText, int recordsEffected, Exception exception)
        {
            var executeNonQueryAsyncFinishedMessage = new ExecuteNonQueryAsyncFinishedMessage
            {
                CommandId = commandId,
                CommandText = commandText,
                RecordsEffected = recordsEffected,
                Exception = exception
            }.AsMessage(apmContext);

            executeNonQueryAsyncFinishedMessage.PublishMessage(apmContext, this);
        }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            DbDataReader reader;

            var commandHash = CommandText.GetHashCode();
            var apmContext = ApmContext.GetContext(string.Format("DbCommand.ExecuteDbDataReaderAsync.{0}", commandHash));
            var activityId = ApmContext.StartActivityClientSend(apmContext);
            var commandId = activityId;

            LogStartOfExecuteDbDataReaderAsync(apmContext, commandId, CommandText);

            try
            {
                reader = await InnerCommand.ExecuteReaderAsync(behavior, cancellationToken);
                LogStopOfExecuteDbDataReaderAsync(apmContext, commandId, CommandText, reader.RecordsAffected, null);
            }
            catch (Exception exception)
            {
                LogStopOfExecuteDbDataReader(apmContext, commandId, CommandText, 0, exception);
                throw;
            }

            return new ApmDbDataReader(reader, InnerCommand, InnerConnection.ConnectionId, commandId);
        }

        private void LogStartOfExecuteDbDataReaderAsync(IApmContext apmContext, ShortGuid commandId, string commandText)
        {
            var executeDbDataReaderAsyncStartedMessage = new ExecuteDbDataReaderAsyncStartedMessage
            {
                CommandId = commandId,
                CommandText = commandText
            }.AsMessage(apmContext);

            executeDbDataReaderAsyncStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfExecuteDbDataReaderAsync(IApmContext apmContext, ShortGuid commandId, string commandText, int recordsEffected, Exception exception)
        {
            var executeDbDataReaderAsyncFinishedMessage = new ExecuteDbDataReaderAsyncFinishedMessage
            {
                CommandId = commandId,
                CommandText = commandText,
                RecordsEffected = recordsEffected,
                Exception = exception
            }.AsMessage(apmContext);

            executeDbDataReaderAsyncFinishedMessage.PublishMessage(apmContext, this);
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            DbDataReader reader;

            var commandHash = CommandText.GetHashCode();
            var apmContext = ApmContext.GetContext(string.Format("DbCommand.ExecuteDbDataReader.{0}", commandHash));
            var activityId = ApmContext.StartActivityClientSend(apmContext);
            var commandId = activityId;
            try
            {
                LogStartOfExecuteDbDataReader(apmContext, commandId, CommandText);
                try
                {
                    reader = InnerCommand.ExecuteReader(behavior);
                    LogStopOfExecuteDbDataReader(apmContext, commandId, CommandText, reader.RecordsAffected, null);
                }
                catch (Exception exception)
                {
                    LogStopOfExecuteDbDataReader(apmContext, commandId, CommandText, 0, exception);
                    throw;
                }
            }
            finally
            {
                ApmContext.StopActivityClientReceived();
            }
            
            return new ApmDbDataReader(reader, InnerCommand, InnerConnection.ConnectionId, commandId);
        }

        private void LogStartOfExecuteDbDataReader(IApmContext apmContext, ShortGuid commandId, string commandText)
        {
            var executeDbDataReaderStartedMessage = new ExecuteDbDataReaderStartedMessage
            {
                CommandId = commandId,
                CommandText = commandText
            }.AsMessage(apmContext);

            executeDbDataReaderStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfExecuteDbDataReader(IApmContext apmContext, ShortGuid commandId, string commandText, int recordsEffected, Exception exception)
        {
            var executeDbDataReaderFinishedMessage = new ExecuteDbDataReaderFinishedMessage
            {
                CommandId = commandId,
                CommandText = commandText,
                RecordsEffected = recordsEffected,
                Exception = exception
            }.AsMessage(apmContext);

            executeDbDataReaderFinishedMessage.PublishMessage(apmContext, this);
        }

        protected override DbParameter CreateDbParameter()
        {
            return InnerCommand.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && InnerCommand != null)
            {
                InnerCommand.Dispose();
            }

            InnerCommand = null;
            InnerConnection = null;
            base.Dispose(disposing);
        }
    }
}
