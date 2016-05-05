using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Distracey.Agent.Ado
{
    public static class Support
    {
        public static DbProviderFactory TryGetProviderFactory(this DbConnection connection)
        {
            // If we can pull it out quickly and easily
            var profiledConnection = connection as ApmDbConnection;
            if (profiledConnection != null)
            {
                return profiledConnection.InnerProviderFactory;
            }

#if (NET45)
            return DbProviderFactories.GetFactory(connection);
#else
            return connection.GetType().GetProperty("ProviderFactory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(connection, null) as DbProviderFactory;
#endif
        }

        public static DbProviderFactory TryGetProfiledProviderFactory(this DbConnection connection)
        {
            var factory = connection.TryGetProviderFactory();
            if (factory != null)
            { 
                if (!(factory is ApmDbProviderFactory))
                {
                    factory = factory.WrapProviderFactory(); 
                }
            }
            else
            {
                throw new NotSupportedException(string.Format(Resources.DbFactoryNotFoundInDbConnection, connection.GetType().FullName));
            }

            return factory;
        }

        public static DbProviderFactory WrapProviderFactory(this DbProviderFactory factory)
        {
            if (!(factory is ApmDbProviderFactory))
            { 
                var factoryType = typeof(ApmDbProviderFactory<>).MakeGenericType(factory.GetType());
                return (DbProviderFactory)factoryType.GetField("Instance").GetValue(null);    
            }

            return factory;
        }

        public static DataTable FindDbProviderFactoryTable()
        {
            var providerFactories = typeof(DbProviderFactories);
            var providerField = providerFactories.GetField("_configTable", BindingFlags.NonPublic | BindingFlags.Static) ?? providerFactories.GetField("_providerTable", BindingFlags.NonPublic | BindingFlags.Static);
            var registrations = providerField.GetValue(null);
            return registrations is DataSet ? ((DataSet)registrations).Tables["DbProviderFactories"] : (DataTable)registrations;
        }
    }
}
