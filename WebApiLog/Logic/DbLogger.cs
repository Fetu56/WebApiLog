using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WebApiLog.Logic
{
    public static class DbLogger 
    {
        private static SqlConnection _conn;
        static DbLogger()
        {
            _conn = new SqlConnection("Data Source=SQL8001.site4now.net;Initial Catalog=db_a84e85_egordb;User Id=db_a84e85_egordb_admin;Password=EgorPrivet123");
            _conn.Open();
        }
        public static void Log(string data, string ip, ActionResult result = null)
        {
            if(result == null)
            {
                result = new ObjectResult(null);
            }
            string methodCall = new StackTrace().GetFrame(1)?.GetMethod()?.ReflectedType.FullName;
            SqlCommand command = new SqlCommand($"INSERT INTO [Log] VALUES('{ip}', '{methodCall}', '{result}', '{data}');", _conn);
            command.ExecuteNonQuery();
        }
    }
}
