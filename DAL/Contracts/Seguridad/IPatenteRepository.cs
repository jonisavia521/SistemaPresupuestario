using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Contracts.Seguridad
{
    /// <summary>
    /// Repositorio para patentes (permisos individuales)
    /// </summary>
    public interface IPatenteRepository : IRepository<Patente>
    {
        /// <summary>
        /// Obtiene una patente con todas sus relaciones
        /// </summary>
        /// <param name="id">ID de la patente</param>
        /// <returns>Patente con relaciones</returns>
        Task<Patente> GetWithRelationsAsync(Guid id);

        /// <summary>
        /// Obtiene patentes por vista/formulario
        /// </summary>
        /// <param name="vista">Nombre de la vista</param>
        /// <returns>Lista de patentes</returns>
        Task<IEnumerable<Patente>> GetByVistaAsync(string vista);

        /// <summary>
        /// Obtiene todas las patentes disponibles con información de asignación para un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario (opcional)</param>
        /// <returns>Lista de patentes con información de asignación</returns>
        Task<IEnumerable<Patente>> GetAllWithAssignmentInfoAsync(Guid? usuarioId = null);

        /// <summary>
        /// Obtiene todas las patentes heredadas por un usuario a través de familias
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de patentes heredadas con información de origen</returns>
        Task<IEnumerable<(Patente patente, Familia familiaOrigen)>> GetPatentesHeredadasAsync(Guid usuarioId);

        /// <summary>
        /// Cuenta patentes por diferentes criterios
        /// </summary>
        /// <returns>Información de conteos</returns>
        Task<(int total, int conVista, int sinVista)> GetCountsAsync();

        /// <summary>
        /// Verifica si una patente está en uso (asignada a usuarios o familias)
        /// </summary>
        /// <param name="patenteId">ID de la patente</param>
        /// <returns>True si está en uso</returns>
        Task<bool> IsInUseAsync(Guid patenteId);

        /// <summary>
        /// Obtiene las familias que tienen asignada una patente específica
        /// </summary>
        /// <param name="patenteId">ID de la patente</param>
        /// <returns>Lista de familias</returns>
        Task<IEnumerable<Familia>> GetFamiliasConPatenteAsync(Guid patenteId);

        /// <summary>
        /// Obtiene los usuarios que tienen asignada una patente específica (directamente)
        /// </summary>
        /// <param name="patenteId">ID de la patente</param>
        /// <returns>Lista de usuarios</returns>
        Task<IEnumerable<Usuario>> GetUsuariosConPatenteDirectaAsync(Guid patenteId);
    }
}