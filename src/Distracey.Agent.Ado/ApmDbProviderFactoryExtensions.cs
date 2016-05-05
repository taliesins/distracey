using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace Distracey.Agent.Ado
{
    public static class ApmDbProviderFactoryExtensions
    {
        public readonly static Dictionary<string, string> Factories = new Dictionary<string, string>();

        public static void AddApmDbProvider()
        {
            Trace.Write("AdoInspector: Starting to replace DbProviderFactory");

            // This forces the creation 
            try
            {
                DbProviderFactories.GetFactory("Anything");
            }
            catch (ArgumentException)
            {
            }

            // Find the registered providers
            var table = Support.FindDbProviderFactoryTable();

            // Run through and replace providers
            foreach (var row in table.Rows.Cast<DataRow>().ToList())
            {
                DbProviderFactory factory;
                try
                {
                    factory = DbProviderFactories.GetFactory(row);

                    Trace.Write(string.Format("AdoInspector: Successfully retrieved factory - {0}", row["Name"]));
                }
                catch (Exception)
                {
                    Trace.Write(string.Format("AdoInspector: Failed to retrieve factory - {0}", row["Name"]));
                    continue;
                }

                // Check that we haven't already wrapped things up 
                if (factory is ApmDbProviderFactory)
                {
                    Trace.Write(string.Format("AdoInspector: Factory is already wrapped - {0}", row["Name"]));
                    continue;
                }

                var proxyType = typeof(ApmDbProviderFactory<>).MakeGenericType(factory.GetType());

                Factories.Add(row["InvariantName"].ToString(), row["AssemblyQualifiedName"].ToString());

                var newRow = table.NewRow();
                newRow["Name"] = row["Name"];
                newRow["Description"] = row["Description"];
                newRow["InvariantName"] = row["InvariantName"];
                newRow["AssemblyQualifiedName"] = proxyType.AssemblyQualifiedName;

                table.Rows.Remove(row);
                table.Rows.Add(newRow);

                Trace.Write(string.Format("AdoInspector: Successfully replaced - {0}", newRow["Name"]));
            }

            Trace.Write("AdoInspector: Finished replacing DbProviderFactory");
        }

    }
}
