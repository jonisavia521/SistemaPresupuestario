using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SistemaPresupuestario.DAL.Context;
using SistemaPresupuestario.DAL.Repositories.Base;
using SistemaPresupuestario.DAL.Repositories.Interfaces;
using SistemaPresupuestario.DomainModel.Seguridad;

namespace SistemaPresupuestario.DAL.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Familia entity
    /// Provides optimized queries for hierarchical family operations
    /// </summary>
    public class FamiliaRepository : Repository<Familia>, IFamiliaRepository
    {
        public FamiliaRepository(SistemaPresupuestarioContext context) : base(context)
        {
        }

        public Familia GetByIdWithRelations(Guid familiaId)
        {
            return _dbSet
                .Include(f => f.FamiliaPadre)
                .Include(f => f.FamiliasHijas.Select(fh => fh.Patentes))
                .Include(f => f.Patentes)
                .Include(f => f.Usuarios)
                .FirstOrDefault(f => f.Id == familiaId);
        }

        public IEnumerable<Familia> GetRootFamilies()
        {
            return _dbSet
                .Where(f => f.FamiliaPadreId == null)
                .Include(f => f.FamiliasHijas)
                .Include(f => f.Patentes)
                .OrderBy(f => f.Nombre)
                .ToList();
        }

        public IEnumerable<FamiliaHierarchyDto> GetFamilyHierarchy()
        {
            var rootFamilies = GetRootFamilies();
            return rootFamilies.Select(f => BuildHierarchyDto(f, 0)).ToList();
        }

        private FamiliaHierarchyDto BuildHierarchyDto(Familia familia, int level)
        {
            var dto = new FamiliaHierarchyDto
            {
                Id = familia.Id,
                Nombre = familia.Nombre,
                Descripcion = familia.Descripcion,
                Level = level,
                ParentId = familia.FamiliaPadreId,
                HasChildren = familia.FamiliasHijas?.Any() == true,
                DirectPatentsCount = familia.Patentes?.Count ?? 0,
                EffectivePatentsCount = familia.ObtenerPatentesEfectivas().Count(),
                UsersCount = familia.Usuarios?.Count ?? 0,
                DirectPatents = familia.Patentes?.ToList() ?? new List<Patente>()
            };

            if (familia.FamiliasHijas?.Any() == true)
            {
                dto.Children = familia.FamiliasHijas
                    .OrderBy(fh => fh.Nombre)
                    .Select(fh => BuildHierarchyDto(fh, level + 1))
                    .ToList();
            }
            else
            {
                dto.Children = new List<FamiliaHierarchyDto>();
            }

            return dto;
        }

        public IEnumerable<Familia> GetChildFamilies(Guid familiaId)
        {
            return _dbSet
                .Where(f => f.FamiliaPadreId == familiaId)
                .Include(f => f.Patentes)
                .OrderBy(f => f.Nombre)
                .ToList();
        }

        public IEnumerable<Familia> GetDescendantFamilies(Guid familiaId)
        {
            var descendants = new List<Familia>();
            var visited = new HashSet<Guid>();
            
            GetDescendantsRecursive(familiaId, descendants, visited);
            
            return descendants;
        }

        private void GetDescendantsRecursive(Guid familiaId, List<Familia> descendants, HashSet<Guid> visited)
        {
            if (visited.Contains(familiaId))
                return;

            visited.Add(familiaId);

            var children = _dbSet
                .Where(f => f.FamiliaPadreId == familiaId)
                .Include(f => f.Patentes)
                .ToList();

            foreach (var child in children)
            {
                descendants.Add(child);
                GetDescendantsRecursive(child.Id, descendants, visited);
            }
        }

        public IEnumerable<Familia> GetAncestorFamilies(Guid familiaId)
        {
            var ancestors = new List<Familia>();
            var visited = new HashSet<Guid>();
            
            GetAncestorsRecursive(familiaId, ancestors, visited);
            
            return ancestors;
        }

        private void GetAncestorsRecursive(Guid familiaId, List<Familia> ancestors, HashSet<Guid> visited)
        {
            if (visited.Contains(familiaId))
                return;

            visited.Add(familiaId);

            var familia = _dbSet
                .Include(f => f.FamiliaPadre)
                .FirstOrDefault(f => f.Id == familiaId);

            if (familia?.FamiliaPadre != null)
            {
                ancestors.Add(familia.FamiliaPadre);
                GetAncestorsRecursive(familia.FamiliaPadre.Id, ancestors, visited);
            }
        }

        public IEnumerable<Patente> GetDirectPatents(Guid familiaId)
        {
            var familia = _dbSet
                .Include(f => f.Patentes)
                .FirstOrDefault(f => f.Id == familiaId);

            return familia?.Patentes?.ToList() ?? new List<Patente>();
        }

        public IEnumerable<Patente> GetEffectivePatents(Guid familiaId)
        {
            var familia = GetByIdWithRelations(familiaId);
            if (familia == null)
                return Enumerable.Empty<Patente>();

            return familia.ObtenerPatentesEfectivas();
        }

        public bool WouldCreateCycle(Guid childId, Guid potentialParentId)
        {
            // A cycle would be created if the potential parent is actually a descendant of the child
            var descendants = GetDescendantFamilies(childId);
            return descendants.Any(d => d.Id == potentialParentId);
        }

        public IEnumerable<Familia> GetFamilyPath(Guid familiaId)
        {
            var path = new List<Familia>();
            var visited = new HashSet<Guid>();
            
            GetFamilyPathRecursive(familiaId, path, visited);
            
            // Reverse to get path from root to target
            path.Reverse();
            return path;
        }

        private void GetFamilyPathRecursive(Guid familiaId, List<Familia> path, HashSet<Guid> visited)
        {
            if (visited.Contains(familiaId))
                return;

            visited.Add(familiaId);

            var familia = _dbSet
                .Include(f => f.FamiliaPadre)
                .FirstOrDefault(f => f.Id == familiaId);

            if (familia != null)
            {
                path.Add(familia);
                
                if (familia.FamiliaPadre != null)
                {
                    GetFamilyPathRecursive(familia.FamiliaPadre.Id, path, visited);
                }
            }
        }

        public IEnumerable<Familia> GetFamiliesByLevel(int level)
        {
            if (level == 0)
            {
                return GetRootFamilies();
            }

            // For higher levels, we need to traverse the hierarchy
            var currentLevel = GetRootFamilies();
            
            for (int i = 0; i < level; i++)
            {
                var nextLevel = new List<Familia>();
                foreach (var familia in currentLevel)
                {
                    nextLevel.AddRange(GetChildFamilies(familia.Id));
                }
                currentLevel = nextLevel;
            }

            return currentLevel;
        }

        public void UpdateFamilyWithPatents(Familia familia, IEnumerable<Guid> patenteIds)
        {
            if (familia == null)
                throw new ArgumentNullException(nameof(familia));

            // Load existing family with relations
            var existingFamily = GetByIdWithRelations(familia.Id);
            if (existingFamily == null)
                throw new InvalidOperationException($"Familia with ID {familia.Id} not found");

            // Update basic properties
            existingFamily.Nombre = familia.Nombre;
            existingFamily.Descripcion = familia.Descripcion;

            // Update patent assignments
            existingFamily.Patentes.Clear();
            if (patenteIds != null)
            {
                var patentes = _context.Patentes.Where(p => patenteIds.Contains(p.Id)).ToList();
                foreach (var patente in patentes)
                {
                    existingFamily.Patentes.Add(patente);
                }
            }

            existingFamily.ModifiedAt = DateTime.Now;
        }

        public IEnumerable<Familia> GetFamiliesByPatent(Guid patenteId)
        {
            return _dbSet
                .Where(f => f.Patentes.Any(p => p.Id == patenteId))
                .Include(f => f.Patentes)
                .Include(f => f.Usuarios)
                .OrderBy(f => f.Nombre)
                .ToList();
        }
    }
}