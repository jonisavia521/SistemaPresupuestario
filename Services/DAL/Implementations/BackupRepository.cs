using Services.DAL.Tools;
using Services.DAL.Tools.Enums;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Services.DAL.Implementations
{
    /// <summary>
    /// Repositorio de acceso a datos para operaciones de Backup y Restore de SQL Server
    /// Esta clase solo ejecuta comandos SQL, sin lógica de negocio
    /// REFACTORIZADO: Ahora usa SqlServerHelper como todos los demás repositorios
    /// Las excepciones se propagan naturalmente para ser manejadas en capas superiores
    /// </summary>
    public sealed class BackupRepository
    {
        private readonly SqlServerHelper _sqlHelper;
        private readonly string _databaseName;

        public BackupRepository(SqlServerHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
            
            // SqlHelper principal para la base de datos SistemaPresupuestario
            
            
            
            // Extraer nombre de BD desde connection string
            var connectionString = System.Configuration.ConfigurationManager
                .ConnectionStrings["Huamani_SistemaPresupuestario"]?.ConnectionString;
            
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                _databaseName = builder.InitialCatalog;
            }
            
            if (string.IsNullOrWhiteSpace(_databaseName))
                throw new InvalidOperationException("No se pudo determinar el nombre de la base de datos");
        }

        /// <summary>
        /// Ejecuta un BACKUP de la base de datos
        /// NOTA: La opción COMPRESSION ha sido removida para compatibilidad con SQL Server Express Edition
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        public void ExecuteBackup(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta del archivo de backup no puede estar vacía", nameof(rutaArchivo));

            // CAMBIO: Se removió COMPRESSION para compatibilidad con SQL Server Express
            // Express Edition no soporta compresión de backups
            string sql = $@"
                BACKUP DATABASE [{_databaseName}] 
                TO DISK = @RutaArchivo 
                WITH FORMAT, 
                     INIT, 
                     NAME = N'{_databaseName} - Full Database Backup', 
                     SKIP, 
                     NOREWIND, 
                     NOUNLOAD,
                     STATS = 10";

            var parametrosSQL = new SqlParameter[]
            {
                new SqlParameter("@RutaArchivo", rutaArchivo)
            };
            _sqlHelper.setDataBase(enumDataBase.Master);
            _sqlHelper.ExecuteNonQuery(sql, CommandType.Text, parametrosSQL);
        }

        /// <summary>
        /// Ejecuta un RESTORE de la base de datos
        /// NOTA: Esta operación requiere conexión a master y permisos elevados
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        public void ExecuteRestore(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta del archivo de restore no puede estar vacía", nameof(rutaArchivo));

            try
            {
                // Paso 1: Poner la base de datos en modo SINGLE_USER
                string sqlSingleUser = $@"
                    IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{_databaseName}')
                    BEGIN
                        ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    END";
                _sqlHelper.setDataBase(enumDataBase.Master);
                _sqlHelper.ExecuteNonQuery(sqlSingleUser, CommandType.Text);

                // Paso 2: Ejecutar RESTORE
                string sqlRestore = $@"
                    RESTORE DATABASE [{_databaseName}] 
                    FROM DISK = @RutaArchivo 
                    WITH REPLACE,
                         STATS = 10";

                var parametrosSQL = new SqlParameter[]
                {
                    new SqlParameter("@RutaArchivo", rutaArchivo)
                };

                _sqlHelper.ExecuteNonQuery(sqlRestore, CommandType.Text, parametrosSQL);

                // Paso 3: Volver a modo MULTI_USER
                string sqlMultiUser = $@"ALTER DATABASE [{_databaseName}] SET MULTI_USER;";
                _sqlHelper.ExecuteNonQuery(sqlMultiUser, CommandType.Text);
            }
            catch
            {
                // Intentar restaurar a MULTI_USER en caso de error
                try
                {
                    string sqlMultiUser = $@"ALTER DATABASE [{_databaseName}] SET MULTI_USER;";
                    _sqlHelper.ExecuteNonQuery(sqlMultiUser, CommandType.Text);
                }
                catch { /* Ignorar errores al intentar restaurar estado */ }

                throw; // Re-lanzar la excepción original
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

            var parametrosSQL = new SqlParameter[]
            {
                new SqlParameter("@RutaArchivo", ruta),
                new SqlParameter("@Estado", estado),
                new SqlParameter("@MensajeError", (object)error ?? DBNull.Value),
                new SqlParameter("@UsuarioApp", usuario)
            };
            _sqlHelper.setDataBase(enumDataBase.Huamani_SistemaPresupuestario);
            _sqlHelper.ExecuteNonQuery(sql, CommandType.Text, parametrosSQL);
        }

        /// <summary>
        /// Obtiene el historial de backups ordenado por fecha descendente
        /// </summary>
        /// <returns>DataTable con el historial</returns>
        public DataTable GetHistorial()
        {
            string sql = $@"
                SELECT 
                    ID,
                    FechaHora,
                    RutaArchivo,
                    Estado,
                    MensajeError,
                    UsuarioApp
                FROM {_databaseName}.dbo.HistorialBackups
                ORDER BY ID DESC";
            _sqlHelper.setDataBase(enumDataBase.Huamani_SistemaPresupuestario);
            return _sqlHelper.ExecuteReader(sql, CommandType.Text);
        }
    }
}
