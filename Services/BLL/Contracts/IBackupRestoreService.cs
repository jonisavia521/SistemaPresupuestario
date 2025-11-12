using System.Data;
using System.Threading.Tasks;

namespace Services.BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de Backup y Restore de SQL Server
    /// </summary>
    public interface IBackupRestoreService
    {
        /// <summary>
        /// Crea un backup de la base de datos de forma asíncrona
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        /// <param name="usuarioApp">Usuario de la aplicación que ejecuta el backup</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        Task CrearBackupAsync(string rutaArchivo, string usuarioApp);

        /// <summary>
        /// Restaura un backup de la base de datos de forma asíncrona
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        Task RestaurarBackupAsync(string rutaArchivo);

        /// <summary>
        /// Obtiene el historial de backups realizados
        /// </summary>
        /// <returns>DataTable con el historial</returns>
        DataTable ObtenerHistorial();
    }
}
