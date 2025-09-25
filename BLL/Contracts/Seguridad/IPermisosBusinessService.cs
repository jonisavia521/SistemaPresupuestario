using BLL.DTOs.Seguridad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Contracts.Seguridad
{
    /// <summary>
    /// Interfaz para servicios de negocio de cálculo de permisos efectivos
    /// </summary>
    public interface IPermisosBusinessService
    {
        /// <summary>
        /// Calcula todos los permisos efectivos de un usuario
        /// (patentes directas + patentes heredadas de familias)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de permisos efectivos con información de origen</returns>
        Task<IEnumerable<PermisoEfectivoDto>> CalcularPermisosEfectivosAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene las patentes asignadas directamente a un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de patentes directas</returns>
        Task<IEnumerable<PatenteDto>> GetPatentesDirectasUsuarioAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene todas las patentes heredadas de las familias de un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de patentes heredadas con información de origen</returns>
        Task<IEnumerable<PatenteDto>> GetPatentesHeredadasUsuarioAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene todas las patentes disponibles con estado de asignación para un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de patentes con flags de asignación directa/heredada</returns>
        Task<IEnumerable<PatenteDto>> GetAllPatentesWithAssignmentStatusAsync(Guid usuarioId);

        /// <summary>
        /// Asigna familias a un usuario (reemplaza asignaciones existentes)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="familiasIds">IDs de las familias a asignar</param>
        /// <param name="versionConcurrencia">Versión para control de concurrencia</param>
        Task AsignarFamiliasAsync(Guid usuarioId, IEnumerable<Guid> familiasIds, string versionConcurrencia);

        /// <summary>
        /// Asigna patentes a un usuario (reemplaza asignaciones existentes)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="patentesIds">IDs de las patentes a asignar</param>
        /// <param name="versionConcurrencia">Versión para control de concurrencia</param>
        Task AsignarPatentesAsync(Guid usuarioId, IEnumerable<Guid> patentesIds, string versionConcurrencia);

        /// <summary>
        /// Valida que un usuario tenga al menos un permiso efectivo
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="familiasIds">IDs de familias que tendría asignadas</param>
        /// <param name="patentesIds">IDs de patentes que tendría asignadas</param>
        /// <returns>True si tendrá al menos un permiso efectivo</returns>
        Task<bool> ValidarUsuarioTienePermisosAsync(Guid usuarioId, IEnumerable<Guid> familiasIds, IEnumerable<Guid> patentesIds);

        /// <summary>
        /// Cuenta la cantidad de permisos efectivos de un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Información de conteos de permisos</returns>
        Task<PermissionCountInfo> GetPermissionCountsAsync(Guid usuarioId);
    }

    /// <summary>
    /// Información de conteos de permisos de un usuario
    /// </summary>
    public class PermissionCountInfo
    {
        public int PatentesDirectas { get; set; }
        public int FamiliasDirectas { get; set; }
        public int PatentesHeredadas { get; set; }
        public int PermisosEfectivosUnicos { get; set; }
    }
}