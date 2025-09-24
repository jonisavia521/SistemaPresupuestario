using BLL.DTOs;
using BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Seguridad
{
    /// <summary>
    /// Servicio de lógica de negocio para gestión de usuarios
    /// DECISIÓN: Separar lógica de negocio de acceso a datos para mejor testabilidad
    /// </summary>
    public interface IUsuarioBusinessService
    {
        /// <summary>
        /// Obtiene lista paginada de usuarios con filtros aplicados
        /// </summary>
        /// <param name="filtroUsuario">Filtro por nombre de usuario</param>
        /// <param name="filtroNombre">Filtro por nombre completo</param>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Lista de UserDto con información resumida</returns>
        Task<(List<UserDto> usuarios, int total)> GetUsuariosPaginatedAsync(
            string filtroUsuario, string filtroNombre, int page, int pageSize);

        /// <summary>
        /// Obtiene usuario por ID con toda la información para edición
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>UserEditDto con información completa</returns>
        /// <exception cref="NotFoundException">Si el usuario no existe</exception>
        Task<UserEditDto> GetUsuarioForEditAsync(Guid id);

        /// <summary>
        /// Crea nuevo usuario con validaciones de negocio
        /// </summary>
        /// <param name="userEditDto">Información del usuario</param>
        /// <returns>ID del usuario creado</returns>
        /// <exception cref="DomainValidationException">Si hay errores de validación</exception>
        /// <exception cref="PermissionIntegrityException">Si no tiene permisos efectivos</exception>
        Task<Guid> CreateUsuarioAsync(UserEditDto userEditDto);

        /// <summary>
        /// Actualiza usuario existente con validaciones de negocio
        /// </summary>
        /// <param name="userEditDto">Información actualizada del usuario</param>
        /// <exception cref="NotFoundException">Si el usuario no existe</exception>
        /// <exception cref="ConcurrencyException">Si hay conflicto de concurrencia</exception>
        /// <exception cref="DomainValidationException">Si hay errores de validación</exception>
        /// <exception cref="PermissionIntegrityException">Si no tiene permisos efectivos</exception>
        Task UpdateUsuarioAsync(UserEditDto userEditDto);

        /// <summary>
        /// Elimina usuario con validaciones de integridad
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="timestampBase64">Timestamp para control de concurrencia</param>
        /// <exception cref="NotFoundException">Si el usuario no existe</exception>
        /// <exception cref="ConcurrencyException">Si hay conflicto de concurrencia</exception>
        Task DeleteUsuarioAsync(Guid id, string timestampBase64);

        /// <summary>
        /// Valida que un nombre de usuario sea único
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="excludeId">ID a excluir (para edición)</param>
        /// <returns>True si es único</returns>
        Task<bool> IsUsuarioUniqueAsync(string usuario, Guid? excludeId = null);

        /// <summary>
        /// Valida fortaleza de contraseña según reglas de negocio
        /// </summary>
        /// <param name="password">Contraseña a validar</param>
        /// <returns>True si cumple los requisitos</returns>
        bool ValidatePasswordStrength(string password);

        /// <summary>
        /// Obtiene los requisitos de contraseña para mostrar al usuario
        /// </summary>
        /// <returns>Descripción de requisitos</returns>
        string GetPasswordRequirements();

        /// <summary>
        /// Autentica usuario por nombre de usuario y contraseña
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>UserDto si es válido, null si no</returns>
        Task<UserDto> AuthenticateAsync(string usuario, string password);

        /// <summary>
        /// Cambia contraseña de usuario con validaciones
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="currentPassword">Contraseña actual</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <exception cref="DomainValidationException">Si la contraseña actual es incorrecta o la nueva no cumple requisitos</exception>
        Task ChangePasswordAsync(Guid usuarioId, string currentPassword, string newPassword);
    }
}