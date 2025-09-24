using BLL.DTOs;
using BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Seguridad
{
    /// <summary>
    /// Servicio de lógica de negocio para cálculo y gestión de permisos
    /// DECISIÓN: Servicio separado para encapsular la complejidad del patrón Composite
    /// </summary>
    public interface IPermisosBusinessService
    {
        /// <summary>
        /// Calcula permisos efectivos de un usuario (directos + heredados)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de permisos efectivos con información de origen</returns>
        Task<List<PermisoEfectivoDto>> GetPermisosEfectivosAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene jerarquía completa de familias para visualización en TreeView
        /// </summary>
        /// <param name="usuarioId">ID del usuario (opcional, para marcar asignaciones)</param>
        /// <returns>Lista jerárquica de familias</returns>
        Task<List<FamiliaDto>> GetJerarquiaFamiliasAsync(Guid? usuarioId = null);

        /// <summary>
        /// Obtiene todas las patentes disponibles
        /// </summary>
        /// <param name="usuarioId">ID del usuario (opcional, para marcar asignaciones)</param>
        /// <returns>Lista de patentes con información de asignación</returns>
        Task<List<PatenteDto>> GetPatentesDisponiblesAsync(Guid? usuarioId = null);

        /// <summary>
        /// Obtiene patentes asignadas directamente a un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de patentes directas</returns>
        Task<List<PatenteDto>> GetPatentesDirectasAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene familias asignadas directamente a un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de familias asignadas</returns>
        Task<List<FamiliaDto>> GetFamiliasAsignadasAsync(Guid usuarioId);

        /// <summary>
        /// Valida que una asignación de permisos sea válida
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="familiasIds">IDs de familias a asignar</param>
        /// <param name="patentesIds">IDs de patentes a asignar</param>
        /// <returns>True si la asignación es válida</returns>
        /// <exception cref="PermissionIntegrityException">Si hay violaciones de integridad</exception>
        Task ValidatePermissionAssignmentAsync(Guid usuarioId, 
            IEnumerable<Guid> familiasIds, IEnumerable<Guid> patentesIds);

        /// <summary>
        /// Detecta si una relación padre-hijo en familias generaría un ciclo
        /// </summary>
        /// <param name="padreId">ID de la familia padre</param>
        /// <param name="hijoId">ID de la familia hijo</param>
        /// <returns>True si generaría un ciclo</returns>
        Task<bool> WouldCreateCycleAsync(Guid padreId, Guid hijoId);

        /// <summary>
        /// Calcula el número de permisos efectivos de un usuario
        /// (para mostrar en listados sin calcular todos los detalles)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Tupla con (permisos directos, permisos efectivos totales)</returns>
        Task<(int directos, int efectivos)> CountPermisosEfectivosAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene estadísticas de uso de una patente
        /// </summary>
        /// <param name="patenteId">ID de la patente</param>
        /// <returns>Información de uso de la patente</returns>
        Task<dynamic> GetPatenteUsageStatsAsync(Guid patenteId);

        /// <summary>
        /// Verifica si un usuario tiene una patente específica (directa o heredada)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="patenteId">ID de la patente</param>
        /// <returns>True si el usuario tiene la patente</returns>
        Task<bool> UserHasPatenteAsync(Guid usuarioId, Guid patenteId);

        /// <summary>
        /// Obtiene todas las patentes efectivas de una familia (directas + heredadas)
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <returns>Lista de patentes efectivas</returns>
        Task<List<PatenteDto>> GetPatentesEfectivasFamiliaAsync(Guid familiaId);

        /// <summary>
        /// Construye la ruta completa de una familia en la jerarquía
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <returns>Ruta jerárquica (ej: "Administración > Usuarios > Consulta")</returns>
        Task<string> GetFamiliaPathAsync(Guid familiaId);
    }
}