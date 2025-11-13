using System.Data;

namespace Services.BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de Backup y Restore de SQL Server
    /// </summary>
    public interface IBackupRestoreService
    {
        /// <summary>
        /// Crea un backup de la base de datos
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        /// <param name="usuarioApp">Usuario de la aplicación que ejecuta el backup</param>
        void CrearBackup(string rutaArchivo, string usuarioApp);

        /// <summary>
        /// Restaura un backup de la base de datos
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        void RestaurarBackup(string rutaArchivo);

        /// <summary>
        /// Obtiene el historial de backups realizados
        /// </summary>
        /// <returns>DataTable con el historial</returns>
        DataTable ObtenerHistorial();
    }
}
