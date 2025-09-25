using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Contracts.Seguridad
{
    /// <summary>
    /// Repositorio para familias de permisos con operaciones jerárquicas
    /// </summary>
    public interface IFamiliaRepository : IRepository<Familia>
    {
        /// <summary>
        /// Obtiene una familia con todas sus relaciones cargadas
        /// </summary>
        /// <param name="id">ID de la familia</param>
        /// <returns>Familia con patentes y relaciones</returns>
        Task<Familia> GetWithRelationsAsync(Guid id);

        /// <summary>
        /// Obtiene todas las familias con sus relaciones jerárquicas
        /// </summary>
        /// <returns>Lista de familias con jerarquía</returns>
        Task<IEnumerable<Familia>> GetAllWithHierarchyAsync();

        /// <summary>
        /// Obtiene las familias hijas de una familia específica
        /// </summary>
        /// <param name="familiaId">ID de la familia padre</param>
        /// <returns>Lista de familias hijas</returns>
        Task<IEnumerable<Familia>> GetFamiliasHijasAsync(Guid familiaId);

        /// <summary>
        /// Obtiene las familias padre de una familia específica
        /// </summary>
        /// <param name="familiaId">ID de la familia hija</param>
        /// <returns>Lista de familias padre</returns>
        Task<IEnumerable<Familia>> GetFamiliasPadreAsync(Guid familiaId);

        /// <summary>
        /// Obtiene las patentes asignadas directamente a una familia
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <returns>Lista de patentes directas</returns>
        Task<IEnumerable<Patente>> GetPatentesDirectasAsync(Guid familiaId);

        /// <summary>
        /// Verifica si una familia tiene ciclos en su jerarquía
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <param name="familiasPadreIds">IDs de las familias que serían padre</param>
        /// <returns>True si crearía un ciclo</returns>
        Task<bool> WouldCreateCycleAsync(Guid familiaId, IEnumerable<Guid> familiasPadreIds);

        /// <summary>
        /// Obtiene todas las familias descendientes de una familia (recursivo)
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <returns>Lista de familias descendientes</returns>
        Task<IEnumerable<Familia>> GetDescendientesAsync(Guid familiaId);

        /// <summary>
        /// Obtiene todas las familias ascendientes de una familia (recursivo)
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <returns>Lista de familias ascendientes</returns>
        Task<IEnumerable<Familia>> GetAscendientesAsync(Guid familiaId);

        /// <summary>
        /// Establece las relaciones padre-hijo de una familia
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <param name="familiasPadreIds">IDs de las familias padre</param>
        /// <param name="familiasHijasIds">IDs de las familias hijas</param>
        Task EstablecerRelacionesAsync(Guid familiaId, IEnumerable<Guid> familiasPadreIds, IEnumerable<Guid> familiasHijasIds);

        /// <summary>
        /// Asigna patentes a una familia (reemplaza asignaciones existentes)
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <param name="patentesIds">IDs de las patentes a asignar</param>
        Task AsignarPatentesAsync(Guid familiaId, IEnumerable<Guid> patentesIds);
    }
}