using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Services.DAL
{
    /// <summary>
    /// Repositorio de acceso a datos para operaciones de Backup y Restore de SQL Server
    /// Esta clase solo ejecuta comandos SQL, sin lógica de negocio
    /// </summary>
    public class BackupRepository
    {
        private readonly string _connectionString;
        private readonly string _masterConnectionString;

        public BackupRepository()
        {
            // Obtener connection strings desde App.config
            var csSetting = ConfigurationManager.ConnectionStrings["SistemaPresupuestario"];
            if (csSetting == null)
                throw new InvalidOperationException("No se encontró la connection string 'SistemaPresupuestario' en App.config");

            _connectionString = csSetting.ConnectionString;

            // Construir connection string para master (reemplazar el catalog)
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };
            _masterConnectionString = builder.ConnectionString;
        }

        /// <summary>
        /// Ejecuta un BACKUP de la base de datos
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        public void ExecuteBackup(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta del archivo de backup no puede estar vacía", nameof(rutaArchivo));

            // Obtener el nombre de la base de datos desde la connection string
            var builder = new SqlConnectionStringBuilder(_connectionString);
            string databaseName = builder.InitialCatalog;

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("No se pudo determinar el nombre de la base de datos desde la connection string");

            string sql = $@"
                BACKUP DATABASE [{databaseName}] 
                TO DISK = @RutaArchivo 
                WITH FORMAT, 
                     INIT, 
                     NAME = N'{databaseName} - Full Database Backup', 
                     SKIP, 
                     NOREWIND, 
                     NOUNLOAD,
                     COMPRESSION,
                     STATS = 10";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = 3600; // 1 hora
                    cmd.Parameters.AddWithValue("@RutaArchivo", rutaArchivo);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Ejecuta un RESTORE de la base de datos
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        public void ExecuteRestore(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta del archivo de restore no puede estar vacía", nameof(rutaArchivo));

            // Obtener el nombre de la base de datos desde la connection string
            var builder = new SqlConnectionStringBuilder(_connectionString);
            string databaseName = builder.InitialCatalog;

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("No se pudo determinar el nombre de la base de datos desde la connection string");

            // Conectarse a master para poder hacer el RESTORE
            using (SqlConnection conn = new SqlConnection(_masterConnectionString))
            {
                conn.Open();

                // Paso 1: Poner la base de datos en modo SINGLE_USER
                string sqlSingleUser = $@"
                    IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
                    BEGIN
                        ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    END";

                using (SqlCommand cmdSingleUser = new SqlCommand(sqlSingleUser, conn))
                {
                    cmdSingleUser.CommandTimeout = 3600;
                    cmdSingleUser.ExecuteNonQuery();
                }

                // Paso 2: Ejecutar RESTORE
                string sqlRestore = $@"
                    RESTORE DATABASE [{databaseName}] 
                    FROM DISK = @RutaArchivo 
                    WITH REPLACE,
                         STATS = 10";

                using (SqlCommand cmdRestore = new SqlCommand(sqlRestore, conn))
                {
                    cmdRestore.CommandTimeout = 3600;
                    cmdRestore.Parameters.AddWithValue("@RutaArchivo", rutaArchivo);
                    cmdRestore.ExecuteNonQuery();
                }

                // Paso 3: Volver a modo MULTI_USER
                string sqlMultiUser = $@"ALTER DATABASE [{databaseName}] SET MULTI_USER;";

                using (SqlCommand cmdMultiUser = new SqlCommand(sqlMultiUser, conn))
                {
                    cmdMultiUser.CommandTimeout = 3600;
                    cmdMultiUser.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserta un registro en la tabla de historial de backups
        /// </summary>
        /// <param name="ruta">Ruta del archivo de backup</param>
        /// <param name="usuario">Usuario que ejecutó la operación</param>
        /// <param name="estado">Estado de la operación (Exitoso/Fallido)</param>
        /// <param name="error">Mensaje de error (si hubo)</param>
        public void InsertHistorial(string ruta, string usuario, string estado, string error)
        {
            if (string.IsNullOrWhiteSpace(ruta))
                throw new ArgumentException("La ruta del archivo no puede estar vacía", nameof(ruta));

            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("El usuario no puede estar vacío", nameof(usuario));

            if (string.IsNullOrWhiteSpace(estado))
                throw new ArgumentException("El estado no puede estar vacío", nameof(estado));

            string sql = @"
                INSERT INTO dbo.HistorialBackups 
                    (FechaHora, RutaArchivo, Estado, MensajeError, UsuarioApp)
                VALUES 
                    (GETDATE(), @RutaArchivo, @Estado, @MensajeError, @UsuarioApp)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@RutaArchivo", ruta);
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    cmd.Parameters.AddWithValue("@MensajeError", (object)error ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UsuarioApp", usuario);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Obtiene el historial de backups ordenado por fecha descendente
        /// </summary>
        /// <returns>DataTable con el historial</returns>
        public DataTable GetHistorial()
        {
            string sql = @"
                SELECT 
                    ID,
                    FechaHora,
                    RutaArchivo,
                    Estado,
                    MensajeError,
                    UsuarioApp
                FROM dbo.HistorialBackups
                ORDER BY ID DESC";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }
    }
}
