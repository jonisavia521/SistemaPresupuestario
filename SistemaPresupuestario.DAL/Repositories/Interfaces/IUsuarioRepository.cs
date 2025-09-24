using System;
using System.Collections.Generic;
using SistemaPresupuestario.DAL.Repositories.Base;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.DAL.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Usuario entity with specific operations
    /// </summary>
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        /// <summary>
        /// Gets user by username
        /// </summary>
        Usuario GetByUsername(string username);

        /// <summary>
        /// Gets user by username including all related data (families and patents)
        /// </summary>
        Usuario GetByUsernameWithRelations(string username);

        /// <summary>
        /// Gets user by ID including all related data (families and patents)
        /// </summary>
        Usuario GetByIdWithRelations(Guid userId);

        /// <summary>
        /// Checks if username already exists (for validation)
        /// </summary>
        bool ExistsUsername(string username, Guid? excludeUserId = null);

        /// <summary>
        /// Gets users with filtering by name or username, with pagination
        /// </summary>
        IEnumerable<Usuario> GetUsersFiltered(
            string searchText,
            int pageNumber,
            int pageSize,
            out int totalCount);

        /// <summary>
        /// Gets all effective patents for a user (direct + inherited from families)
        /// Optimized version that calculates permissions at database level when possible
        /// </summary>
        IEnumerable<Patente> GetEffectivePatents(Guid userId);

        /// <summary>
        /// Gets effective permissions summary for a user
        /// Returns information about direct vs inherited permissions
        /// </summary>
        PermissionsummaryDto GetPermissionSummary(Guid userId);

        /// <summary>
        /// Updates user with their family and patent assignments in a single operation
        /// Handles the many-to-many relationship updates efficiently
        /// </summary>
        void UpdateUserWithRelations(Usuario usuario, IEnumerable<Guid> familiaIds, IEnumerable<Guid> patenteIds);

        /// <summary>
        /// Adds user with their initial family and patent assignments
        /// </summary>
        void AddUserWithRelations(Usuario usuario, IEnumerable<Guid> familiaIds, IEnumerable<Guid> patenteIds);

        /// <summary>
        /// Gets users for a specific family
        /// </summary>
        IEnumerable<Usuario> GetUsersByFamily(Guid familiaId);

        /// <summary>
        /// Gets users who have a specific patent (direct or inherited)
        /// </summary>
        IEnumerable<Usuario> GetUsersByPatent(Guid patenteId);
    }

    /// <summary>
    /// DTO for permission summary information
    /// </summary>
    public class PermissionsummaryDto
    {
        public Guid UsuarioId { get; set; }
        public int DirectPatentsCount { get; set; }
        public int InheritedPatentsCount { get; set; }
        public int TotalEffectivePatentsCount { get; set; }
        public int FamiliasCount { get; set; }
        public IEnumerable<Patente> DirectPatents { get; set; }
        public IEnumerable<Patente> InheritedPatents { get; set; }
        public IEnumerable<Familia> Familias { get; set; }
    }
}