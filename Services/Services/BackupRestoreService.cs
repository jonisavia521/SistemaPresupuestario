using Services.BLL.Contracts;
using Services.DAL;
using Services.DAL.Implementations;
using Services.Services.Contracts;
using System;
using System.Data;

namespace Services.Services
{
    /// <summary>
    /// Servicio de lógica de negocio para Backup y Restore de SQL Server
    /// Esta es la clase "inteligente" que orquesta toda la funcionalidad
    /// Es la única clase que la UI debe conocer e instanciar
    /// REFACTORIZADO: Ahora usa inyección de dependencias
    /// </summary>
    public class BackupRestoreService : IBackupRestoreService
    {
        private readonly BackupRepository _repository;

        public BackupRestoreService(BackupRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Crea un backup de la base de datos
        /// Registra el resultado en la tabla de historial
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        /// <param name="usuarioApp">Usuario de la aplicación que ejecuta el backup</param>
        public void CrearBackup(string rutaArchivo, string usuarioApp)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta del archivo de backup no puede estar vacía", nameof(rutaArchivo));

            if (string.IsNullOrWhiteSpace(usuarioApp))
                throw new ArgumentException("El usuario no puede estar vacío", nameof(usuarioApp));

            string estado = "Fallido";
            string mensajeError = null;

            try
            {
                // Ejecutar el backup
                _repository.ExecuteBackup(rutaArchivo);

                estado = "Exitoso";
            }
            catch (Exception ex)
            {
                mensajeError = ex.Message;
                throw; // Relanzar la excepción después de registrarla
            }
            finally
            {
                // Siempre registrar en el historial, éxito o fallo
                try
                {
                    _repository.InsertHistorial(rutaArchivo, usuarioApp, estado, mensajeError);
                }
                catch
                {
                    // Si falla el registro del historial, no hacer nada
                    // (el backup ya se ejecutó o falló, ese es el resultado importante)
                }
            }
        }

        /// <summary>
        /// Restaura un backup de la base de datos
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo de backup (.bak)</param>
        public void RestaurarBackup(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("La ruta del archivo de restore no puede estar vacía", nameof(rutaArchivo));

            try
            {
                // Ejecutar el restore
                _repository.ExecuteRestore(rutaArchivo);
            }
            catch (Exception ex)
            {
                // Relanzar la excepción con un mensaje más descriptivo
                throw new InvalidOperationException($"Error al restaurar el backup: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene el historial de backups realizados
        /// </summary>
        /// <returns>DataTable con el historial</returns>
        public DataTable ObtenerHistorial()
        {
            try
            {
                return _repository.GetHistorial();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener el historial: {ex.Message}", ex);
            }
        }
    }
}
