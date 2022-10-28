using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Tools
{
    internal static class SqlServerHelper
    {
        readonly static string conString;

        static SqlServerHelper()
        {
            conString = ConfigurationManager.ConnectionStrings["ServicesConString"].ConnectionString;
        }
        public static Object ExecuteScalar(String commandText,
           CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Set the connection, command, and then execute the command with query and return the reader.
        /// </summary>
        public static DataTable ExecuteReader(String commandText,
            CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable(); ;
            using (SqlConnection conn = new SqlConnection(conString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }
    }
}
