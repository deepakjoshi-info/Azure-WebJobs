using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJobs
{
    public class AppConfig
    {
        static AppConfig()
        {
            var nameValue = ConfigurationManager.ConnectionStrings;
            if (nameValue["MyDbConnection"] != null)
            {
                MyDbConnection = Convert.ToString(nameValue["MyDbConnection"]);
            }
            else
            {
                throw new Exception("Key MyDbConnection does not exists in App config file");
            }
        }
        public static string MyDbConnection
        {
            get;
            set;
        }
    }
}
