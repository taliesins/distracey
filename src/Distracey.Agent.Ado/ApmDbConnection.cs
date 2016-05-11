using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Transactions;
using Distracey.Common;
using Distracey.Common.Helpers;
using Distracey.Common.Message;

namespace Distracey.Agent.Ado
{
    public class ApmDbConnection : DbConnection
    {
        private bool _wasPreviouslyUsed;

        public ApmDbConnection(DbConnection connection)
            : this(connection, connection.TryGetProfiledProviderFactory())
        { 
        }

        public ApmDbConnection(DbConnection connection, DbProviderFactory providerFactory)
            : this(connection, providerFactory, ShortGuid.NewGuid())
        { 
        }

        public ApmDbConnection(DbConnection connection, DbProviderFactory providerFactory, ShortGuid connectionId)
        {
            InnerConnection = connection;
            InnerProviderFactory = providerFactory;
            ConnectionId = connectionId;

            if (connection.State == ConnectionState.Open)
            {
                OpenConnection();
            }

            connection.StateChange += StateChangeHaneler;
        }

        private IApmContext GetApmContext()
        {
            return ApmContextHelper.GetApmContext();
        }

        public override event StateChangeEventHandler StateChange
        {
            add
            {
                if (InnerConnection != null)
                {
                    InnerConnection.StateChange += value;
                }
            }
            remove
            {
                if (InnerConnection != null)
                {
                    InnerConnection.StateChange -= value;
                }
            }
        }

        public DbProviderFactory InnerProviderFactory { get; set; }

        public DbConnection InnerConnection { get; set; }

        public ShortGuid ConnectionId { get; set; }

        public override string ConnectionString
        {
            get { return InnerConnection.ConnectionString; }
            set { InnerConnection.ConnectionString = value; }
        }

        public override int ConnectionTimeout
        {
            get { return InnerConnection.ConnectionTimeout; }
        }

        public override string Database
        {
            get { return InnerConnection.Database; }
        }

        public override string DataSource
        {
            get { return InnerConnection.DataSource; }
        }

        public override ConnectionState State
        {
            get { return InnerConnection.State; }
        }

        public override string ServerVersion
        {
            get { return InnerConnection.ServerVersion; }
        }

        public override ISite Site
        {
            get { return InnerConnection.Site; }
            set { InnerConnection.Site = value; }
        }

        protected override DbProviderFactory DbProviderFactory
        {
            get { return InnerProviderFactory; }
        }

        public override void ChangeDatabase(string databaseName)
        {
            InnerConnection.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            InnerConnection.Close(); 
        }

        public override void Open()
        {
            InnerConnection.Open(); 
        }

        public override void EnlistTransaction(Transaction transaction)
        {
            InnerConnection.EnlistTransaction(transaction);
            if (transaction != null)
            {
                LogStartOfDtcTransaction(ConnectionId, transaction.TransactionInformation, transaction.IsolationLevel);
                transaction.TransactionCompleted += OnDtcTransactionCompleted;
            }
        }
         
        public override DataTable GetSchema()
        {
            return InnerConnection.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return InnerConnection.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return InnerConnection.GetSchema(collectionName, restrictionValues);
        }
         
        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return new ApmDbTransaction(InnerConnection.BeginTransaction(isolationLevel), this);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new ApmDbCommand(InnerConnection.CreateCommand(), this);
        }

        protected override object GetService(Type service)
        {
            return ((IServiceProvider)InnerConnection).GetService(service);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && InnerConnection != null)
            {
                InnerConnection.Dispose();
                InnerConnection.StateChange -= StateChangeHaneler;
            }

            InnerConnection = null;
            InnerProviderFactory = null;
            base.Dispose(disposing);
        }
        
        private void OnDtcTransactionCompleted(object sender, TransactionEventArgs args)
        {
            TransactionStatus aborted;
            try
            {
                aborted = args.Transaction.TransactionInformation.Status;
            }
            catch (ObjectDisposedException)
            {
                aborted = TransactionStatus.Aborted;
            }

            LogStopOfDtcTransaction(ConnectionId, args.Transaction.TransactionInformation, args.Transaction.IsolationLevel, aborted);
        }

        private void StateChangeHaneler(object sender, StateChangeEventArgs args)
        { 
            if (args.CurrentState == ConnectionState.Open)
            {
                OpenConnection();
            }
            else if (args.CurrentState == ConnectionState.Closed)
            {
                ClosedConnection();
            }
        }

        private void OpenConnection()
        {
            if (_wasPreviouslyUsed)
            {
                ConnectionId = ShortGuid.NewGuid();
            }

            LogStartOfDbConnection(ConnectionId);
        }

        private void ClosedConnection()
        {
            _wasPreviouslyUsed = true;

            LogStopOfDbConnection(ConnectionId);
        }

        private void LogStartOfDbConnection(ShortGuid connectionId)
        {
            var apmContext = GetApmContext();
            var dbConnectionOpenedMessage = new DbConnectionOpenedMessage
            {
                ConectionId = connectionId
            }.AsMessage(apmContext);

            dbConnectionOpenedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfDbConnection(ShortGuid connectionId)
        {
            var apmContext = GetApmContext();
            var dbConnectionClosedMessage = new DbConnectionClosedMessage
            {
                ConectionId = connectionId
            }.AsMessage(apmContext);

            dbConnectionClosedMessage.PublishMessage(apmContext, this);
        }

        private void LogStartOfDtcTransaction(ShortGuid connectionId, TransactionInformation transactionInformation, System.Transactions.IsolationLevel isolationLevel)
        {
            var apmContext = GetApmContext();
            var dbConnectionOpenedMessage = new DbConnectionOpenedMessage
            {
                ConectionId = connectionId,
                TransactionInformation = transactionInformation,
                IsolationLevel = isolationLevel
            }.AsMessage(apmContext);

            dbConnectionOpenedMessage.PublishMessage(apmContext, this);
        }

        private void LogStopOfDtcTransaction(ShortGuid connectionId, TransactionInformation transactionInformation, System.Transactions.IsolationLevel isolationLevel, TransactionStatus aborted)
        {
            var apmContext = GetApmContext();
            var dbConnectionClosedMessage = new DbConnectionClosedMessage
            {
                ConectionId = connectionId,
                TransactionInformation = transactionInformation,
                IsolationLevel = isolationLevel,
                Aborted = aborted
            }.AsMessage(apmContext);

            dbConnectionClosedMessage.PublishMessage(apmContext, this);
        }
    }
}
