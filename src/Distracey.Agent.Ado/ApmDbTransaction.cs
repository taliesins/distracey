using System;
using System.Data;
using System.Data.Common;
using Distracey.Common;
using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class ApmDbTransaction : DbTransaction
    {
        public ApmDbTransaction(DbTransaction transaction, ApmDbConnection connection)
        {
            InnerTransaction = transaction; 
            InnerConnection = connection;
            TransactionId = ShortGuid.NewGuid();
            ApmContext = Common.ApmContext.GetContext(string.Format("DbTransaction.{0}", connection.ConnectionString.GetHashCode()));

            LogStartOfDbTransaction(ApmContext, TransactionId);
        }

        private void LogStartOfDbTransaction(IApmContext apmContext, ShortGuid transactionId)
        {
            var executeNonQueryStartedMessage = new DbTransactionStartedMessage
            {
                TransactionId = transactionId
            }.AsMessage(apmContext);

            executeNonQueryStartedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfDbTransaction(IApmContext apmContext, ShortGuid transactionId, bool rollback, Exception exception)
        {
            var executeNonQueryFinishedMessage = new DbTransactionFinishedMessage
            {
                TransactionId = transactionId,
                Rollback = rollback,
                Exception = exception
            }.AsMessage(apmContext);

            executeNonQueryFinishedMessage.PublishMessage(apmContext, this);
        }

        public ApmDbConnection InnerConnection { get; set; } 
         
        public DbTransaction InnerTransaction { get; set; }

        public ShortGuid TransactionId { get; set; }

        public IApmContext ApmContext { get; set; }

        public override IsolationLevel IsolationLevel
        {
            get { return InnerTransaction.IsolationLevel; }
        }

        protected override DbConnection DbConnection
        {
            get { return InnerConnection; }
        }

        public override void Commit()
        {
            try
            {
                InnerTransaction.Commit();
                LogStopOfDbTransaction(ApmContext, TransactionId, false, null);
            }
            catch (Exception exception)
            {
                LogStopOfDbTransaction(ApmContext, TransactionId, false, exception);
                throw;
            } 
        }

        public override void Rollback()
        {
            try
            {
                InnerTransaction.Rollback();
                LogStopOfDbTransaction(ApmContext, TransactionId, true, null);
            }
            catch (Exception exception)
            {
                LogStopOfDbTransaction(ApmContext, TransactionId, true, exception);
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                InnerTransaction.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
