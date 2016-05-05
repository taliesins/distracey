using System;
using System.Data.Common;
using System.Reflection;
using Distracey.Common;

namespace Distracey.Agent.Ado
{
    public abstract class ApmDbProviderFactory : DbProviderFactory
    {
    }

    public class ApmDbProviderFactory<TProviderFactory> : ApmDbProviderFactory, IServiceProvider
        where TProviderFactory : DbProviderFactory
    {
        public static readonly ApmDbProviderFactory<TProviderFactory> Instance = new ApmDbProviderFactory<TProviderFactory>();
        
        public ApmDbProviderFactory()
        {            
            var field = typeof(TProviderFactory).GetField("Instance", BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                throw new NotSupportedException("Provider doesn't have Instance property.");
            }

            InnerFactory = (TProviderFactory)field.GetValue(null);           
        }

        private IApmContext GetApmContext()
        {
            return ApmContextHelper.GetApmContext();
        }

        public override bool CanCreateDataSourceEnumerator
        {
            get { return InnerFactory.CanCreateDataSourceEnumerator; }
        }

        private TProviderFactory InnerFactory { get; set; }

        public override DbCommand CreateCommand()
        {
            var command = InnerFactory.CreateCommand();
            if (IsAdoMonitoringNeeded()) 
            {
                return new ApmDbCommand(command);
            }
            return command;
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return InnerFactory.CreateCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            var connection = InnerFactory.CreateConnection();
            if (IsAdoMonitoringNeeded()) 
            {
                return new ApmDbConnection(connection, this);
            }
            return connection;
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return InnerFactory.CreateConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            var adapter = InnerFactory.CreateDataAdapter();
            if (IsAdoMonitoringNeeded()) 
            {
                return new ApmDbDataAdapter(adapter);
            }
            return adapter;
        }

        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return InnerFactory.CreateDataSourceEnumerator();
        }

        public override DbParameter CreateParameter()
        {
            return InnerFactory.CreateParameter();
        }
         
        public object GetService(Type serviceType)
        {
            if (serviceType == GetType())
            {
                return InnerFactory;
            }

            var service = ((IServiceProvider)InnerFactory).GetService(serviceType);

            // HACK: To make things easier on ourselves we are going to try and see
            // what we can do for people using EF. If they are using EF but don't have
            // Glimpse.EF then we throw because the exception that will be caused down 
            // the track by EF isn't obvious as to whats going on. When it gets to 
            // requesting DbProviderServices, if we don't return the profiled version, 
            // when GetDbProviderManifestToken is called, it passes in a ApmDbConnection rather than the inner connection. This is a problem because the GetDbProviderManifestToken trys to cast the connection to its concreat type
            if (serviceType.FullName == "System.Data.Common.DbProviderServices")
            {
                var type = Type.GetType("Glimpse.EF.AlternateType.GlimpseDbProviderServices, Glimpse.EF43", false);
                if (type == null)
                {
                    type = Type.GetType("Glimpse.EF.AlternateType.GlimpseDbProviderServices, Glimpse.EF5", false);
                }

                if (type == null)
                {
                    type = Type.GetType("Glimpse.EF.AlternateType.GlimpseDbProviderServices, Glimpse.EF6", false);
                }

                if (type != null)
                {
                    return Activator.CreateInstance(type, service);
                } 
                
                throw new NotSupportedException(Resources.GlimpseEFNotPresent);  
            }

            return service;
        }

        private static bool IsAdoMonitoringNeeded()
        {
            return true;
        }
    }
}