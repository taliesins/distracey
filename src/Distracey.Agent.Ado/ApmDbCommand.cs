using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using Distracey.Common.Helpers;

#if NET45

#endif

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

        private IApmContext GetApmContext()
        {
            return ApmContextHelper.GetApmContext();
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
            var commandId = ShortGuid.NewGuid();

            LogStartOfExecuteNonQuery(commandId);

            try
            {
                recordsAffected = InnerCommand.ExecuteNonQuery();
                LogStopOfExecuteNonQuery(commandId, recordsAffected, null);
            }
            catch (Exception exception)
            {
                LogStopOfExecuteNonQuery(commandId, 0, exception);
                throw;
            }

            return recordsAffected;
        }

        private void LogStartOfExecuteNonQuery(ShortGuid commandId)
        {
            var executeNonQueryStartedMessage = new ExecuteNonQueryStartedMessage
            {
                CommandId = commandId
            };

            var eventContext = new ApmEvent<ExecuteNonQueryStartedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeNonQueryStartedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfExecuteNonQuery(ShortGuid commandId, int recordsEffected, Exception exception)
        {
            var executeNonQueryFinishedMessage = new ExecuteNonQueryFinishedMessage
            {
                CommandId = commandId,
                RecordsEffected = recordsEffected,
                Exception = exception
            };

            var eventContext = new ApmEvent<ExecuteNonQueryFinishedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeNonQueryFinishedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public override object ExecuteScalar()
        {
            object result;
            var commandId = ShortGuid.NewGuid();

            LogStartOfExecuteScalar(commandId);
 
            try
            {
                result = InnerCommand.ExecuteScalar();
                LogStopOfExecuteScalar(commandId, null);
            }
            catch (Exception exception)
            {
                LogStopOfExecuteScalar(commandId, exception);
                throw;
            }

            return result;
        }

        private void LogStartOfExecuteScalar(ShortGuid commandId)
        {
            var executeScalarStartedMessage = new ExecuteScalarStartedMessage
            {
                CommandId = commandId
            };

            var eventContext = new ApmEvent<ExecuteScalarStartedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeScalarStartedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfExecuteScalar(ShortGuid commandId, Exception exception)
        {
            var executeScalarFinishedMessage = new ExecuteScalarFinishedMessage
            {
                CommandId = commandId,
                Exception = exception
            };

            var eventContext = new ApmEvent<ExecuteScalarFinishedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeScalarFinishedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

#if NET45
        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            object result;
            var commandId = ShortGuid.NewGuid();

            LogStartOfExecuteScalarAsync(commandId);

            try
            {
                result = await InnerCommand.ExecuteScalarAsync(cancellationToken);
                LogStopOfExecuteScalarAsync(commandId, null);
            }
            catch (Exception exception)
            {
                LogStopOfExecuteScalarAsync(commandId, exception);
                throw;
            }

            return result;
        }

        private void LogStartOfExecuteScalarAsync(ShortGuid commandId)
        {
            var executeScalarAsyncStartedMessage = new ExecuteScalarAsyncStartedMessage
            {
                CommandId = commandId
            };

            var eventContext = new ApmEvent<ExecuteScalarAsyncStartedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeScalarAsyncStartedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfExecuteScalarAsync(ShortGuid commandId, Exception exception)
        {
            var executeScalarAsyncFinishedMessage = new ExecuteScalarAsyncFinishedMessage
            {
                CommandId = commandId,
                Exception = exception
            };

            var eventContext = new ApmEvent<ExecuteScalarAsyncFinishedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeScalarAsyncFinishedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            int recordsEffected;
            var commandId = ShortGuid.NewGuid();

            LogStartOfExecuteNonQueryAsync(commandId);

            try
            {
                recordsEffected = await InnerCommand.ExecuteNonQueryAsync(cancellationToken);
                LogStopOfExecuteNonQueryAsync(commandId, recordsEffected, null);
            }
            catch (Exception exception)
            {
                LogStopOfExecuteNonQueryAsync(commandId, 0, exception);
                throw;
            }

            return recordsEffected;
        }

        private void LogStartOfExecuteNonQueryAsync(ShortGuid commandId)
        {
            var executeNonQueryAsyncStartedMessage = new ExecuteNonQueryAsyncStartedMessage
            {
                CommandId = commandId
            };

            var eventContext = new ApmEvent<ExecuteNonQueryAsyncStartedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeNonQueryAsyncStartedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfExecuteNonQueryAsync(ShortGuid commandId, int recordsEffected, Exception exception)
        {
            var executeNonQueryAsyncFinishedMessage = new ExecuteNonQueryAsyncFinishedMessage
            {
                CommandId = commandId,
                RecordsEffected = recordsEffected,
                Exception = exception
            };

            var eventContext = new ApmEvent<ExecuteNonQueryAsyncFinishedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeNonQueryAsyncFinishedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            DbDataReader reader;
            var commandId = ShortGuid.NewGuid();

            LogStartOfExecuteDbDataReaderAsync(commandId);

            try
            {
                reader = await InnerCommand.ExecuteReaderAsync(behavior, cancellationToken);
                LogStopOfExecuteDbDataReaderAsync(commandId, reader.RecordsAffected, null);
            }
            catch (Exception exception)
            {
                LogStopOfExecuteDbDataReader(commandId, 0, exception);
                throw;
            }

            return new ApmDbDataReader(reader, InnerCommand, InnerConnection.ConnectionId, commandId);
        }

        private void LogStartOfExecuteDbDataReaderAsync(ShortGuid commandId)
        {
            var executeDbDataReaderAsyncStartedMessage = new ExecuteDbDataReaderAsyncStartedMessage
            {
                CommandId = commandId
            };

            var eventContext = new ApmEvent<ExecuteDbDataReaderAsyncStartedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeDbDataReaderAsyncStartedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfExecuteDbDataReaderAsync(ShortGuid commandId, int recordsEffected, Exception exception)
        {
            var executeDbDataReaderAsyncFinishedMessage = new ExecuteDbDataReaderAsyncFinishedMessage
            {
                CommandId = commandId,
                RecordsEffected = recordsEffected,
                Exception = exception
            };

            var eventContext = new ApmEvent<ExecuteDbDataReaderAsyncFinishedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeDbDataReaderAsyncFinishedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }
#endif

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            DbDataReader reader;
            var commandId = ShortGuid.NewGuid();

            LogStartOfExecuteDbDataReader(commandId);
            try
            {
                reader = InnerCommand.ExecuteReader(behavior);
                LogStopOfExecuteDbDataReader(commandId, reader.RecordsAffected, null);
            }
            catch (Exception exception)
            {
                LogStopOfExecuteDbDataReader(commandId, 0, exception);
                throw;
            }

            return new ApmDbDataReader(reader, InnerCommand, InnerConnection.ConnectionId, commandId);
        }

        private void LogStartOfExecuteDbDataReader(ShortGuid commandId)
        {
            var executeDbDataReaderStartedMessage = new ExecuteDbDataReaderStartedMessage
            {
                CommandId = commandId
            };

            var eventContext = new ApmEvent<ExecuteDbDataReaderStartedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeDbDataReaderStartedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfExecuteDbDataReader(ShortGuid commandId, int recordsEffected, Exception exception)
        {
            var executeDbDataReaderFinishedMessage = new ExecuteDbDataReaderFinishedMessage
            {
                CommandId = commandId,
                RecordsEffected = recordsEffected,
                Exception = exception
            };

            var eventContext = new ApmEvent<ExecuteDbDataReaderFinishedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeDbDataReaderFinishedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
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
