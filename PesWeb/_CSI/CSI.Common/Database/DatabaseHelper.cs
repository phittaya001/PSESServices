using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Data.EntityClient;
using System.IO;

namespace CSI.Common.Database
{
    public class DatabaseHelper
    {
        //static DatabaseHelper()
        //{
        //    CurrentContainerName = string.Empty;
        //    AllConnections = new Dictionary<string, string>();
        //    Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        //    if (config != null)
        //        foreach (ConnectionStringSettings setting in config.ConnectionStrings.ConnectionStrings)
        //            AllConnections.Add(setting.Name, setting.ConnectionString);
        //}
        //public static string CurrentContainerName { get; set; }
        //public static Dictionary<string, string> AllConnections { get; private set; }

        //public static List<String> GetAllConnectionNames()
        //{
        //    return AllConnections.Keys.ToList();
        //}

        //public static void SetCurrentContainerName(string containerName)
        //{
        //    CurrentContainerName = containerName;
        //}

        //public static string GetCurrentContainerName()
        //{
        //    return CurrentContainerName;
        //}

        public static string ExtractProviderConnectionString(string contextConnectionString)
        {
            string connectionString = contextConnectionString;
            if (connectionString.Replace(" ", "").Trim().ToLower().StartsWith("name="))
            {
                string settingName = connectionString.Replace("name=", "");
                var con = ConfigurationManager.ConnectionStrings[settingName];
                if (null != con)
                    connectionString = con.ConnectionString;
            }

            string providerConnectionString;
            if (connectionString.ToLower().StartsWith("metadata="))
            {
                using (EntityConnection ec = new EntityConnection(connectionString))
                    providerConnectionString = ec.StoreConnection.ConnectionString;
            }
            else
                providerConnectionString = connectionString;

            return providerConnectionString;
        }
    }
}
