using System.Data;
using MySql.Data.MySqlClient;
using Xb2.Config;

namespace Xb2.Utils.Database
{
    public static class DbHelper
    {
        private static readonly string _connectionString = Xb2Config.GetConnStr();

        public static DataTable GetDataTable(string commandText)
        {
            return MySqlHelper.ExecuteDataset(_connectionString, commandText).Tables[0];
        }

        public static DataTable GetDataTable(string commandText, MySqlParameter[] parameters)
        {
            return MySqlHelper.ExecuteDataset(_connectionString, commandText, parameters).Tables[0];
        }

        public static object GetScalar(string commandText)
        {
            return MySqlHelper.ExecuteScalar(_connectionString, commandText);
        }


    }
}
