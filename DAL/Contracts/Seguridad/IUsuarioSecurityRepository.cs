using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Contracts.Seguridad
{
    /// <summary>
    /// Repositorio extendido de usuarios con operaciones específicas de seguridad
    /// </summary>
    public interface IUsuarioSecurityRepository : IRepository<Usuario>
    {
        /// <summary>
        /// Obtiene usuario por nombre de usuario
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        /// <returns>Usuario o null si no existe</returns>
        Task<Usuario> GetByUsernameAsync(string nombreUsuario);

        /// <summary>
        /// Verifica si un nombre de usuario ya existe
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        /// <param name="excludeId">ID a excluir de la búsqueda</param>
        /// <returns>True si existe</returns>
        Task<bool> ExistsUsernameAsync(string nombreUsuario, Guid? excludeId = null);

        /// <summary>
        /// Obtiene usuario con todas sus relaciones de permisos cargadas
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario con familias y patentes</returns>
        Task<Usuario> GetWithPermissionsAsync(Guid id);

        /// <summary>
        /// Obtiene usuarios con filtros y paginación
        /// </summary>
        /// <param name="filtroNombre">Filtro por nombre</param>
        /// <param name="filtroUsuario">Filtro por nombre de usuario</param>
        /// <param name="soloActivos">Solo usuarios activos</param>
        /// <param name="skip">Registros a saltar</param>
        /// <param name="take">Registros a tomar</param>
        /// <returns>Lista de usuarios y total</returns>
        Task<(IEnumerable<Usuario> usuarios, int total)> GetPagedAsync(
            string filtroNombre = null, string filtroUsuario = null, 
            bool soloActivos = true, int skip = 0, int take = 50);

        /// <summary>
        /// Obtiene las familias asignadas directamente a un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de familias asignadas</returns>
        Task<IEnumerable<Familia>> GetFamiliasAsignadasAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene las patentes asignadas directamente a un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de patentes asignadas</returns>
        Task<IEnumerable<Patente>> GetPatentesAsignadasAsync(Guid usuarioId);

        /// <summary>
        /// Asigna familias a un usuario (reemplaza asignaciones existentes)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="familiasIds">IDs de familias a asignar</param>
        Task AsignarFamiliasAsync(Guid usuarioId, IEnumerable<Guid> familiasIds);

        /// <summary>
        /// Asigna patentes a un usuario (reemplaza asignaciones existentes)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="patentesIds">IDs de patentes a asignar</param>
        Task AsignarPatentesAsync(Guid usuarioId, IEnumerable<Guid> patentesIds);

        /// <summary>
        /// Cuenta permisos de un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Información de conteos</returns>
        Task<(int familiasDirectas, int patentesDirectas)> GetPermissionCountsAsync(Guid usuarioId);
    }
}