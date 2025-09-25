using BLL.DTOs.Seguridad;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Contracts.Seguridad
{
    /// <summary>
    /// Interfaz para servicios de negocio de usuarios
    /// DECISIÓN: Bypass de Service Layer - UI llamará directamente a BLL
    /// debido a restricción de no modificar capa Service existente
    /// </summary>
    public interface IUsuarioBusinessService
    {
        /// <summary>
        /// Obtiene todos los usuarios con información resumida
        /// </summary>
        /// <returns>Lista de usuarios para mostrar en grilla</returns>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// Obtiene un usuario específico para edición
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario con todos sus datos para editar</returns>
        Task<UserEditDto> GetUserForEditAsync(Guid id);

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="userDto">Datos del usuario a crear</param>
        /// <returns>ID del usuario creado</returns>
        Task<Guid> CreateUserAsync(UserEditDto userDto);

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="userDto">Datos del usuario a actualizar</param>
        Task UpdateUserAsync(UserEditDto userDto);

        /// <summary>
        /// Elimina un usuario (lógica o física según configuración)
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <param name="versionConcurrencia">Versión para control de concurrencia</param>
        Task DeleteUserAsync(Guid id, string versionConcurrencia);

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="nuevaClave">Nueva contraseña</param>
        /// <param name="versionConcurrencia">Versión para control de concurrencia</param>
        Task ChangePasswordAsync(Guid id, string nuevaClave, string versionConcurrencia);

        /// <summary>
        /// Valida que un nombre de usuario no esté duplicado
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a validar</param>
        /// <param name="excludeId">ID a excluir de la validación (para edición)</param>
        /// <returns>True si está disponible</returns>
        Task<bool> IsUsernameAvailableAsync(string nombreUsuario, Guid? excludeId = null);

        /// <summary>
        /// Obtiene usuarios con filtros y paginación
        /// </summary>
        /// <param name="filtroNombre">Filtro por nombre</param>
        /// <param name="filtroUsuario">Filtro por nombre de usuario</param>
        /// <param name="soloActivos">Solo usuarios activos</param>
        /// <param name="pagina">Número de página (base 1)</param>
        /// <param name="tamanoPagina">Tamaño de página</param>
        /// <returns>Resultado paginado</returns>
        Task<PagedResult<UserDto>> GetUsersPagedAsync(string filtroNombre = null, 
            string filtroUsuario = null, bool soloActivos = true, int pagina = 1, int tamanoPagina = 50);
    }

    /// <summary>
    /// Resultado paginado genérico
    /// </summary>
    /// <typeparam name="T">Tipo de datos</typeparam>
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}