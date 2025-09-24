using System;
using System.Collections.Generic;
using SistemaPresupuestario.BLL.DTOs;

namespace SistemaPresupuestario.Service.Seguridad
{
    /// <summary>
    /// Service facade for user management operations
    /// Exposes BLL functionality to the UI layer
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Gets all users with optional filtering
        /// </summary>
        IEnumerable<UsuarioDto> GetUsers(string searchText = null);

        /// <summary>
        /// Gets user by ID
        /// </summary>
        UsuarioDto GetUserById(Guid userId);

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
        /// Gets available families for assignment
        /// </summary>
        IEnumerable<FamiliaDto> GetAvailableFamilies();

        /// <summary>
        /// Gets available patents for assignment
        /// </summary>
        IEnumerable<PatenteDto> GetAvailablePatents();
    }
}