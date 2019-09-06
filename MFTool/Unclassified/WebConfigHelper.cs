using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MFTool
{
    public class WebConfigHelper
    {
        public static List<string> GetDBNames()
        {
            List<string> strList = new List<string>();
            var connList = System.Configuration.ConfigurationManager.ConnectionStrings;
            foreach (ConnectionStringSettings con in connList)
            {
                System.Data.Common.DbConnectionStringBuilder ss = new System.Data.Common.DbConnectionStringBuilder(false);
                ss.ConnectionString = con.ToString();
                object aa = "";
                if (ss.TryGetValue("Initial Catalog", out aa))
                {
                    strList.Add(aa.ToString());
                }
            }
            return strList;
        }

        public static string GetConnSettingNameByDBName(string dbName)
        {
            List<string> strList = new List<string>();
            var connList = System.Configuration.ConfigurationManager.ConnectionStrings;
            foreach (ConnectionStringSettings con in connList)
            {
                System.Data.Common.DbConnectionStringBuilder ss = new System.Data.Common.DbConnectionStringBuilder(false);
                ss.ConnectionString = con.ToString();
                object aa = "";
                if (ss.TryGetValue("Initial Catalog", out aa))
                {
                    if (dbName == aa.ToString())
                    {
                        return con.Name;
                    }
                }
            }
            return "";
        }
    }
}
