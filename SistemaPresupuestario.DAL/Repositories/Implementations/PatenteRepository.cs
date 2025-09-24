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
    /// Repository implementation for Patente entity
    /// Provides optimized queries for patent management operations
    /// </summary>
    public class PatenteRepository : Repository<Patente>, IPatenteRepository
    {
        public PatenteRepository(SistemaPresupuestarioContext context) : base(context)
        {
        }

        public Patente GetByName(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return null;

            return _dbSet.FirstOrDefault(p => p.Nombre == nombre);
        }

        public Patente GetByIdWithRelations(Guid patenteId)
        {
            return _dbSet
                .Include(p => p.Familias)
                .Include(p => p.Usuarios)
                .FirstOrDefault(p => p.Id == patenteId);
        }

        public IEnumerable<Patente> GetPatentsFiltered(string searchText, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _dbSet.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();
                query = query.Where(p => 
                    p.Nombre.ToLower().Contains(searchText) || 
                    p.Vista.ToLower().Contains(searchText) ||
                    p.Descripcion.ToLower().Contains(searchText));
            }

            // Get total count
            totalCount = query.Count();

            // Apply pagination and ordering
            return query
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.Vista)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Familias)
                .Include(p => p.Usuarios)
                .ToList();
        }

        public IEnumerable<Patente> GetOrphanedPatents()
        {
            return _dbSet
                .Where(p => !p.Familias.Any() && !p.Usuarios.Any())
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        public IEnumerable<Patente> GetAvailablePatentsForUser(Guid userId)
        {
            // Get user's current patents (direct and through families)
            var usuario = _context.Usuarios
                .Include(u => u.Patentes)
                .Include(u => u.Familias.Select(f => f.Patentes))
                .FirstOrDefault(u => u.Id == userId);

            if (usuario == null)
                return _dbSet.OrderBy(p => p.Nombre).ToList();

            var currentPatentIds = usuario.ObtenerPatentesEfectivas().Select(p => p.Id).ToList();

            return _dbSet
                .Where(p => !currentPatentIds.Contains(p.Id))
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        public IEnumerable<Patente> GetAvailablePatentsForFamily(Guid familiaId)
        {
            var familia = _context.Familias
                .Include(f => f.Patentes)
                .FirstOrDefault(f => f.Id == familiaId);

            if (familia == null)
                return _dbSet.OrderBy(p => p.Nombre).ToList();

            var currentPatentIds = familia.Patentes.Select(p => p.Id).ToList();

            return _dbSet
                .Where(p => !currentPatentIds.Contains(p.Id))
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        public IEnumerable<Familia> GetFamiliesWithPatent(Guid patenteId)
        {
            return _context.Familias
                .Where(f => f.Patentes.Any(p => p.Id == patenteId))
                .Include(f => f.Usuarios)
                .OrderBy(f => f.Nombre)
                .ToList();
        }

        public IEnumerable<Usuario> GetUsersWithPatent(Guid patenteId)
        {
            // Users with direct patent assignment
            var directUsers = _context.Usuarios
                .Where(u => u.Patentes.Any(p => p.Id == patenteId))
                .ToList();

            // Users with patent through families
            var familyUsers = _context.Usuarios
                .Where(u => u.Familias.Any(f => f.Patentes.Any(p => p.Id == patenteId)))
                .ToList();

            // For users through family hierarchies, we need to check recursively
            var allFamiliesWithPatent = GetFamiliesWithPatent(patenteId);
            var hierarchicalUsers = new List<Usuario>();

            foreach (var familia in allFamiliesWithPatent)
            {
                // Get all parent families that would inherit this patent
                var parentFamilies = GetParentFamiliesRecursive(familia.Id);
                foreach (var parentFamily in parentFamilies)
                {
                    var usersInParent = _context.Usuarios
                        .Where(u => u.Familias.Any(f => f.Id == parentFamily.Id))
                        .ToList();
                    hierarchicalUsers.AddRange(usersInParent);
                }
            }

            // Combine and deduplicate
            return directUsers
                .Union(familyUsers)
                .Union(hierarchicalUsers)
                .Distinct()
                .OrderBy(u => u.Nombre)
                .ToList();
        }

        private IEnumerable<Familia> GetParentFamiliesRecursive(Guid familiaId)
        {
            var parents = new List<Familia>();
            var visited = new HashSet<Guid>();
            
            GetParentsRecursive(familiaId, parents, visited);
            
            return parents;
        }

        private void GetParentsRecursive(Guid familiaId, List<Familia> parents, HashSet<Guid> visited)
        {
            if (visited.Contains(familiaId))
                return;

            visited.Add(familiaId);

            var parentFamilies = _context.Familias
                .Where(f => f.FamiliasHijas.Any(fh => fh.Id == familiaId))
                .ToList();

            foreach (var parent in parentFamilies)
            {
                parents.Add(parent);
                GetParentsRecursive(parent.Id, parents, visited);
            }
        }

        public PatenteUsageDto GetPatenteUsage(Guid patenteId)
        {
            var patente = GetByIdWithRelations(patenteId);
            if (patente == null)
                return null;

            var directUsers = patente.Usuarios?.ToList() ?? new List<Usuario>();
            var familias = patente.Familias?.ToList() ?? new List<Familia>();
            var allUsersWithPatent = GetUsersWithPatent(patenteId).ToList();

            return new PatenteUsageDto
            {
                PatenteId = patenteId,
                Nombre = patente.Nombre,
                DirectUsersCount = directUsers.Count,
                FamiliesCount = familias.Count,
                TotalEffectiveUsersCount = allUsersWithPatent.Count,
                DirectUsers = directUsers,
                Familias = familias
            };
        }

        public bool ExistsPatenteNombre(string nombre, Guid? excludePatenteId = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            var query = _dbSet.Where(p => p.Nombre == nombre);
            
            if (excludePatenteId.HasValue)
            {
                query = query.Where(p => p.Id != excludePatenteId.Value);
            }

            return query.Any();
        }

        public IEnumerable<Patente> GetPatentesByVista(string vista)
        {
            if (string.IsNullOrWhiteSpace(vista))
                return Enumerable.Empty<Patente>();

            return _dbSet
                .Where(p => p.Vista == vista)
                .OrderBy(p => p.Nombre)
                .ToList();
        }
    }
}