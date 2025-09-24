using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Seguridad
{
    /// <summary>
    /// Servicio de capa de presentación para gestión de usuarios
    /// DECISIÓN: Capa service separada para orquestar BLL y proveer interface simple a UI
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Obtiene lista paginada de usuarios con filtros
        /// </summary>
        Task<(List<UserDto> usuarios, int total)> GetUsuariosPaginatedAsync(
            string filtroUsuario = null, string filtroNombre = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Obtiene usuario para edición
        /// </summary>
        Task<UserEditDto> GetUsuarioForEditAsync(Guid id);

        /// <summary>
        /// Crea nuevo usuario
        /// </summary>
        Task<Guid> CreateUsuarioAsync(UserEditDto userEditDto);

        /// <summary>
        /// Actualiza usuario existente
        /// </summary>
        Task UpdateUsuarioAsync(UserEditDto userEditDto);

        /// <summary>
        /// Elimina usuario
        /// </summary>
        Task DeleteUsuarioAsync(Guid id, string timestampBase64);

        /// <summary>
        /// Verifica si un nombre de usuario es único
        /// </summary>
        Task<bool> IsUsuarioUniqueAsync(string usuario, Guid? excludeId = null);

        /// <summary>
        /// Obtiene jerarquía de familias para asignación
        /// </summary>
        Task<List<FamiliaDto>> GetJerarquiaFamiliasAsync(Guid? usuarioId = null);

        /// <summary>
        /// Obtiene patentes disponibles para asignación
        /// </summary>
        Task<List<PatenteDto>> GetPatentesDisponiblesAsync(Guid? usuarioId = null);

        /// <summary>
        /// Obtiene permisos efectivos de un usuario
        /// </summary>
        Task<List<PermisoEfectivoDto>> GetPermisosEfectivosAsync(Guid usuarioId);

        /// <summary>
        /// Valida fortaleza de contraseña
        /// </summary>
        bool ValidatePasswordStrength(string password);

        /// <summary>
        /// Obtiene requisitos de contraseña
        /// </summary>
        string GetPasswordRequirements();

        /// <summary>
        /// Autentica usuario
        /// </summary>
        Task<UserDto> AuthenticateAsync(string usuario, string password);

        /// <summary>
        /// Cambia contraseña de usuario
        /// </summary>
        Task ChangePasswordAsync(Guid usuarioId, string currentPassword, string newPassword);
    }
}