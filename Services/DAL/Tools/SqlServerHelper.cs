using Services.DAL.Tools.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Tools
{
    public class SqlServerHelper
    {
        private string conString;

        public SqlServerHelper(NameValueCollection app)
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["Huamani_SistemaPresupuestario"];
            
            //if (connectionString == null)
            //{
            //    connectionString = ConfigurationManager.ConnectionStrings["Huamani_Seguridad"];
            //}
            
            //if (connectionString == null)
            //{
            //    throw new InvalidOperationException("No se encontró ninguna cadena de conexión válida en App.config");
            //}
            
            //conString = connectionString.ConnectionString;
        }
        // 1. EL PARÁMETRO AHORA ES DEL TIPO enumDataBase, NO string
        public void setDataBase(enumDataBase dataBase)
        {
            string stringConnectionParameter;

            // 2. EL SWITCH AHORA ES LIMPIO Y SEGURO
            switch (dataBase)
            {
                // 3. LOS CASE SON DIRECTAMENTE SOBRE LOS MIEMBROS DEL ENUM
                case enumDataBase.Huamani_SistemaPresupuestario:
                    stringConnectionParameter = "Huamani_SistemaPresupuestario";
                    break;

                case enumDataBase.Huamani_Seguridad:
                    stringConnectionParameter = "Huamani_Seguridad";
                    break;

                default:
                    // 4. ESTA EXCEPCIÓN AHORA ES PARA CASOS DE PROGRAMACIÓN NO PREVISTOS
                    // (Ej: si añades un nuevo miembro al enum y olvidas actualizar este switch)
                    throw new ArgumentOutOfRangeException(nameof(dataBase), dataBase, "Valor de enum no soportado.");
            }

            // 5. OBTÉN LA CADENA DE CONEXIÓN AL FINAL
            conString = ConfigurationManager.ConnectionStrings[stringConnectionParameter].ConnectionString;
        }
        public Object ExecuteScalar(String commandText,
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
        /// establece conexion y ejecutar el comandText
        /// </summary>
        public DataTable ExecuteReader(String commandText,
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

        public int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
