using BLL.DTOs.Seguridad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Contracts.Seguridad
{
    /// <summary>
    /// Interfaz para servicios de negocio de familias de permisos
    /// </summary>
    public interface IFamiliaBusinessService
    {
        /// <summary>
        /// Obtiene todas las familias organizadas jerárquicamente
        /// </summary>
        /// <returns>Lista de familias con su jerarquía</returns>
        Task<IEnumerable<FamiliaDto>> GetAllFamiliasHierarchicalAsync();

        /// <summary>
        /// Obtiene una familia específica con sus relaciones
        /// </summary>
        /// <param name="id">ID de la familia</param>
        /// <returns>Familia con sus datos completos</returns>
        Task<FamiliaDto> GetFamiliaAsync(Guid id);

        /// <summary>
        /// Obtiene las familias asignadas a un usuario específico
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de familias asignadas</returns>
        Task<IEnumerable<FamiliaDto>> GetFamiliasByUsuarioAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene todas las familias disponibles marcando cuáles están asignadas a un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de todas las familias con flag de asignación</returns>
        Task<IEnumerable<FamiliaDto>> GetAllFamiliasWithAssignmentStatusAsync(Guid usuarioId);

        /// <summary>
        /// Valida que no se creen ciclos al establecer relaciones entre familias
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <param name="familiasPadre">IDs de las familias que serían padre</param>
        /// <returns>True si no crea ciclos</returns>
        Task<bool> ValidateNoCyclesAsync(Guid familiaId, IEnumerable<Guid> familiasPadre);

        /// <summary>
        /// Calcula todas las patentes efectivas de una familia (directas + heredadas)
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <returns>Lista de patentes efectivas</returns>
        Task<IEnumerable<PatenteDto>> CalcularPatentesEfectivasAsync(Guid familiaId);
    }
}