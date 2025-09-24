using System;
using System.Collections.Generic;
using SistemaPresupuestario.DAL.Repositories.Base;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.DAL.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Familia entity with hierarchical operations
    /// </summary>
    public interface IFamiliaRepository : IRepository<Familia>
    {
        /// <summary>
        /// Gets family by ID including all related data (patents, parent, children)
        /// </summary>
        Familia GetByIdWithRelations(Guid familiaId);

        /// <summary>
        /// Gets all root families (families without parent)
        /// </summary>
        IEnumerable<Familia> GetRootFamilies();

        /// <summary>
        /// Gets complete family hierarchy starting from roots
        /// Useful for building tree views
        /// </summary>
        IEnumerable<FamiliaHierarchyDto> GetFamilyHierarchy();

        /// <summary>
        /// Gets all child families for a given family (direct children only)
        /// </summary>
        IEnumerable<Familia> GetChildFamilies(Guid familiaId);

        /// <summary>
        /// Gets all descendant families recursively (all children at all levels)
        /// </summary>
        IEnumerable<Familia> GetDescendantFamilies(Guid familiaId);

        /// <summary>
        /// Gets all ancestor families going up to the root
        /// </summary>
        IEnumerable<Familia> GetAncestorFamilies(Guid familiaId);

        /// <summary>
        /// Gets all patents directly assigned to a family
        /// </summary>
        IEnumerable<Patente> GetDirectPatents(Guid familiaId);

        /// <summary>
        /// Gets all effective patents for a family (direct + inherited from children)
        /// Uses optimized query to avoid N+1 problems
        /// </summary>
        IEnumerable<Patente> GetEffectivePatents(Guid familiaId);

        /// <summary>
        /// Checks if creating a parent-child relationship would create a cycle
        /// </summary>
        bool WouldCreateCycle(Guid childId, Guid potentialParentId);

        /// <summary>
        /// Gets the complete path from root to the specified family
        /// </summary>
        IEnumerable<Familia> GetFamilyPath(Guid familiaId);

        /// <summary>
        /// Gets families by level in hierarchy (0 = root level)
        /// </summary>
        IEnumerable<Familia> GetFamiliesByLevel(int level);

        /// <summary>
        /// Updates family with its patent assignments
        /// </summary>
        void UpdateFamilyWithPatents(Familia familia, IEnumerable<Guid> patenteIds);

        /// <summary>
        /// Gets all families that contain a specific patent (directly assigned)
        /// </summary>
        IEnumerable<Familia> GetFamiliesByPatent(Guid patenteId);
    }

    /// <summary>
    /// DTO for representing family hierarchy
    /// </summary>
    public class FamiliaHierarchyDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
        public bool HasChildren { get; set; }
        public int DirectPatentsCount { get; set; }
        public int EffectivePatentsCount { get; set; }
        public int UsersCount { get; set; }
        public IEnumerable<FamiliaHierarchyDto> Children { get; set; }
        public IEnumerable<Patente> DirectPatents { get; set; }
    }
}