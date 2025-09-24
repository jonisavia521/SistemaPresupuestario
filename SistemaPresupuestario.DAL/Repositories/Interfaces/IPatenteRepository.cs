using System;
using System.Collections.Generic;
using SistemaPresupuestario.DAL.Repositories.Base;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.DAL.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Patente entity with specific operations
    /// </summary>
    public interface IPatenteRepository : IRepository<Patente>
    {
        /// <summary>
        /// Gets patent by name
        /// </summary>
        Patente GetByName(string nombre);

        /// <summary>
        /// Gets patent by ID including all related data (families and users)
        /// </summary>
        Patente GetByIdWithRelations(Guid patenteId);

        /// <summary>
        /// Gets all patents with filtering by name, with pagination
        /// </summary>
        IEnumerable<Patente> GetPatentsFiltered(
            string searchText,
            int pageNumber,
            int pageSize,
            out int totalCount);

        /// <summary>
        /// Gets patents that are not assigned to any family or user (orphaned patents)
        /// </summary>
        IEnumerable<Patente> GetOrphanedPatents();

        /// <summary>
        /// Gets patents available for assignment to a specific user
        /// (excluding patents already assigned directly or through families)
        /// </summary>
        IEnumerable<Patente> GetAvailablePatentsForUser(Guid userId);

        /// <summary>
        /// Gets patents available for assignment to a specific family
        /// (excluding patents already assigned to this family)
        /// </summary>
        IEnumerable<Patente> GetAvailablePatentsForFamily(Guid familiaId);

        /// <summary>
        /// Gets all families that have this patent assigned
        /// </summary>
        IEnumerable<Familia> GetFamiliesWithPatent(Guid patenteId);

        /// <summary>
        /// Gets all users that have this patent (directly or through families)
        /// </summary>
        IEnumerable<Usuario> GetUsersWithPatent(Guid patenteId);

        /// <summary>
        /// Gets usage statistics for a patent
        /// </summary>
        PatenteUsageDto GetPatenteUsage(Guid patenteId);

        /// <summary>
        /// Checks if patent name already exists (for validation)
        /// </summary>
        bool ExistsPatenteNombre(string nombre, Guid? excludePatenteId = null);

        /// <summary>
        /// Gets patents by view/form name
        /// </summary>
        IEnumerable<Patente> GetPatentesByVista(string vista);
    }

    /// <summary>
    /// DTO for patent usage statistics
    /// </summary>
    public class PatenteUsageDto
    {
        public Guid PatenteId { get; set; }
        public string Nombre { get; set; }
        public int DirectUsersCount { get; set; }
        public int FamiliesCount { get; set; }
        public int TotalEffectiveUsersCount { get; set; }
        public IEnumerable<Usuario> DirectUsers { get; set; }
        public IEnumerable<Familia> Familias { get; set; }
    }
}