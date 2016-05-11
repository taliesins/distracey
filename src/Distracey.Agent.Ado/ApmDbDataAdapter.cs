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

        private IApmContext GetApmContext()
        {
            return ApmContextHelper.GetApmContext();
        }

        private DbDataAdapter InnerDataAdapter { get; set; }

        public override int Fill(DataSet dataSet)
        {
            if (SelectCommand != null)
            {
                var typedCommand = SelectCommand as ApmDbCommand;
                if (typedCommand != null)
                {
                    InnerDataAdapter.SelectCommand = typedCommand.Inner;

                    var recordsEffected = 0;
                    var commandId = ShortGuid.NewGuid();

                    LogStartOfExecuteDbDataAdapter(commandId);

                    try
                    {
                        recordsEffected = InnerDataAdapter.Fill(dataSet);
                        LogStopOfExecuteDbDataAdapter(commandId, recordsEffected, null);
                    }
                    catch (Exception exception)
                    {
                        LogStopOfExecuteDbDataAdapter(commandId, 0, exception);
                        throw;
                    }

                    return recordsEffected;
                }

                InnerDataAdapter.SelectCommand = SelectCommand;
            }

            return InnerDataAdapter.Fill(dataSet);
        }

        private void LogStartOfExecuteDbDataAdapter(ShortGuid commandId)
        {
            var apmContext = GetApmContext();
            var executeDbDataReaderStartedMessage = new DbDataAdapterStartedMessage
            {
                CommandId = commandId
            }.AsMessage(apmContext);

            executeDbDataReaderStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfExecuteDbDataAdapter(ShortGuid commandId, int recordsEffected, Exception exception)
        {
            var apmContext = GetApmContext();
            var executeDbDataReaderFinishedMessage = new DbDataAdapterFinishedMessage
            {
                CommandId = commandId,
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
