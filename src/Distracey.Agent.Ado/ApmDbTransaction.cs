using System;
using System.Data;
using System.Data.Common;
using Distracey.Common;
using Distracey.Common.EventAggregator;
using Distracey.Common.Helpers;

namespace Distracey.Agent.Ado
{
    public class ApmDbTransaction : DbTransaction
    {
        public ApmDbTransaction(DbTransaction transaction, ApmDbConnection connection)
        {
            InnerTransaction = transaction; 
            InnerConnection = connection;
            TransactionId = ShortGuid.NewGuid();

            LogStartOfDbTransaction(TransactionId);
        }

        private IApmContext GetApmContext()
        {
            return ApmContextHelper.GetApmContext();
        }

        private void LogStartOfDbTransaction(ShortGuid transactionId)
        {
            var executeNonQueryStartedMessage = new DbTransactionStartedMessage
            {
                TransactionId = transactionId
            };

            var eventContext = new ApmEvent<DbTransactionStartedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeNonQueryStartedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void LogStopOfDbTransaction(ShortGuid transactionId, bool rollback, Exception exception)
        {
            var executeNonQueryFinishedMessage = new DbTransactionFinishedMessage
            {
                TransactionId = transactionId,
                Rollback = rollback,
                Exception = exception
            };

            var eventContext = new ApmEvent<DbTransactionFinishedMessage>
            {
                ApmContext = GetApmContext(),
                Event = executeNonQueryFinishedMessage
            };

            this.Publish(eventContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public ApmDbConnection InnerConnection { get; set; } 
         
        public DbTransaction InnerTransaction { get; set; }

        public ShortGuid TransactionId { get; set; }

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
                LogStopOfDbTransaction(TransactionId, false, null);
            }
            catch (Exception exception)
            {
                LogStopOfDbTransaction(TransactionId, false, exception);
                throw;
            } 
        }

        public override void Rollback()
        {
            try
            {
                InnerTransaction.Rollback();
                LogStopOfDbTransaction(TransactionId, true, null);
            }
            catch (Exception exception)
            {
                LogStopOfDbTransaction(TransactionId, true, exception);
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
