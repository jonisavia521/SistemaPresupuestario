using System;
using System.Collections.Generic;
using SistemaPresupuestario.BLL.DTOs;

namespace SistemaPresupuestario.BLL.Services.Interfaces
{
    /// <summary>
    /// Service interface for user management operations
    /// Handles business logic for user CRUD operations and permission management
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Gets all users with filtering and pagination
        /// </summary>
        IEnumerable<UsuarioDto> GetUsers(string searchText = null, int pageNumber = 1, int pageSize = 50, out int totalCount);

        /// <summary>
        /// Gets user by ID with all related data
        /// </summary>
        UsuarioDto GetUserById(Guid userId);

        /// <summary>
        /// Gets user by username
        /// </summary>
        UsuarioDto GetUserByUsername(string username);

        /// <summary>
        /// Creates a new user
        /// </summary>
        UsuarioDto CreateUser(UsuarioEditDto userDto);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        UsuarioDto UpdateUser(UsuarioEditDto userDto);

        /// <summary>
        /// Deletes a user
        /// </summary>
        void DeleteUser(Guid userId);

        /// <summary>
        /// Gets effective permissions for a user
        /// </summary>
        PermisoEfectivoDto GetUserEffectivePermissions(Guid userId);

        /// <summary>
        /// Validates username uniqueness
        /// </summary>
        bool IsUsernameAvailable(string username, Guid? excludeUserId = null);

        /// <summary>
        /// Authenticates a user
        /// </summary>
        UsuarioDto AuthenticateUser(string username, string password);

        /// <summary>
        /// Changes user password
        /// </summary>
        void ChangePassword(Guid userId, string currentPassword, string newPassword);

        /// <summary>
        /// Resets user password (admin operation)
        /// </summary>
        void ResetPassword(Guid userId, string newPassword, bool forceChange = true);

        /// <summary>
        /// Gets available families for assignment
        /// </summary>
        IEnumerable<FamiliaDto> GetAvailableFamilies();

        /// <summary>
        /// Gets available patents for direct assignment
        /// </summary>
        IEnumerable<PatenteDto> GetAvailablePatents(Guid? userId = null);
    }
}