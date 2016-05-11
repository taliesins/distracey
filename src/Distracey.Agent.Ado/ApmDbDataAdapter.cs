using System;
using System.Data;
using System.Data.Common;
using Distracey.Common;
using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class ApmDbDataAdapter : DbDataAdapter
    {
        public ApmDbDataAdapter(DbDataAdapter innerDataAdapter)
        {
            InnerDataAdapter = innerDataAdapter;
        }

        public override bool ReturnProviderSpecificTypes
        {
            get { return InnerDataAdapter.ReturnProviderSpecificTypes; }
            set { InnerDataAdapter.ReturnProviderSpecificTypes = value; }
        }

        public override int UpdateBatchSize
        {
            get { return InnerDataAdapter.UpdateBatchSize; }
            set { InnerDataAdapter.UpdateBatchSize = value; }
        }

        private DbDataAdapter InnerDataAdapter { get; set; }

        public override int Fill(DataSet dataSet)
        {
            if (SelectCommand == null) return InnerDataAdapter.Fill(dataSet);
            var typedCommand = SelectCommand as ApmDbCommand;
            if (typedCommand != null)
            {
                InnerDataAdapter.SelectCommand = typedCommand.Inner;

                var recordsEffected = 0;
                var commandId = ShortGuid.NewGuid();
                var commandText = InnerDataAdapter.SelectCommand.CommandText;
                var commandHash = commandText.GetHashCode();
                var apmContext = ApmContext.GetContext(string.Format("DbDataAdapter.{0}", commandHash));
                    
                LogStartOfExecuteDbDataAdapter(apmContext, commandId, commandText);

                try
                {
                    recordsEffected = InnerDataAdapter.Fill(dataSet);
                    LogStopOfExecuteDbDataAdapter(apmContext, commandId, commandText, recordsEffected, null);
                }
                catch (Exception exception)
                {
                    LogStopOfExecuteDbDataAdapter(apmContext, commandId, commandText, 0, exception);
                    throw;
                }

                return recordsEffected;
            }

            InnerDataAdapter.SelectCommand = SelectCommand;

            return InnerDataAdapter.Fill(dataSet);
        }

        private void LogStartOfExecuteDbDataAdapter(IApmContext apmContext, ShortGuid commandId, string commandText)
        {
            var executeDbDataReaderStartedMessage = new DbDataAdapterStartedMessage
            {
                CommandId = commandId,
                CommandText = commandText
            }.AsMessage(apmContext);

            executeDbDataReaderStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfExecuteDbDataAdapter(IApmContext apmContext, ShortGuid commandId, string commandText, int recordsEffected, Exception exception)
        {
            var executeDbDataReaderFinishedMessage = new DbDataAdapterFinishedMessage
            {
                CommandId = commandId,
                CommandText = commandText,
                RecordsEffected = recordsEffected,
                Exception = exception
            }.AsMessage(apmContext);

            executeDbDataReaderFinishedMessage.PublishMessage(apmContext, this);
        }

        public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            if (SelectCommand != null)
            {
                InnerDataAdapter.SelectCommand = RetrieveBaseType(SelectCommand);
            }

            return InnerDataAdapter.FillSchema(dataSet, schemaType);
        }

        public override IDataParameter[] GetFillParameters()
        {
            return InnerDataAdapter.GetFillParameters();
        }

        public override bool ShouldSerializeAcceptChangesDuringFill()
        {
            return InnerDataAdapter.ShouldSerializeAcceptChangesDuringFill();
        }

        public override bool ShouldSerializeFillLoadOption()
        {
            return InnerDataAdapter.ShouldSerializeFillLoadOption();
        }

        public override string ToString()
        {
            return InnerDataAdapter.ToString();
        }

        public override int Update(DataSet dataSet)
        {
            if (UpdateCommand != null)
            {
                InnerDataAdapter.UpdateCommand = RetrieveBaseType(UpdateCommand);
            }

            if (InsertCommand != null)
            {
                InnerDataAdapter.InsertCommand = RetrieveBaseType(InsertCommand);
            }

            if (DeleteCommand != null)
            {
                InnerDataAdapter.DeleteCommand = RetrieveBaseType(DeleteCommand);
            }

            return InnerDataAdapter.Update(dataSet);
        }

        protected override void Dispose(bool disposing)
        {
            InnerDataAdapter.Dispose();
        }

        private DbCommand RetrieveBaseType(DbCommand command)
        {
            var typedCommand = command as ApmDbCommand;
            return typedCommand.Inner ?? command;
        }
    }
}
