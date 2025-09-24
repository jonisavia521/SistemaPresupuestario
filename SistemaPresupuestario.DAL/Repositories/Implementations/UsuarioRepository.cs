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
    /// Repository implementation for Usuario entity
    /// Provides optimized queries for user management operations
    /// </summary>
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SistemaPresupuestarioContext context) : base(context)
        {
        }

        public Usuario GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return _dbSet.FirstOrDefault(u => u.NombreUsuario == username);
        }

        public Usuario GetByUsernameWithRelations(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            return _dbSet
                .Include(u => u.Familias.Select(f => f.Patentes))
                .Include(u => u.Familias.Select(f => f.FamiliasHijas.Select(fh => fh.Patentes)))
                .Include(u => u.Patentes)
                .FirstOrDefault(u => u.NombreUsuario == username);
        }

        public Usuario GetByIdWithRelations(Guid userId)
        {
            return _dbSet
                .Include(u => u.Familias.Select(f => f.Patentes))
                .Include(u => u.Familias.Select(f => f.FamiliasHijas.Select(fh => fh.Patentes)))
                .Include(u => u.Patentes)
                .FirstOrDefault(u => u.Id == userId);
        }

        public bool ExistsUsername(string username, Guid? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var query = _dbSet.Where(u => u.NombreUsuario == username);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return query.Any();
        }

        public IEnumerable<Usuario> GetUsersFiltered(string searchText, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _dbSet.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim().ToLower();
                query = query.Where(u => 
                    u.Nombre.ToLower().Contains(searchText) || 
                    u.NombreUsuario.ToLower().Contains(searchText) ||
                    u.Email.ToLower().Contains(searchText));
            }

            // Get total count
            totalCount = query.Count();

            // Apply pagination and ordering
            return query
                .OrderBy(u => u.Nombre)
                .ThenBy(u => u.NombreUsuario)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.Familias)
                .Include(u => u.Patentes)
                .ToList();
        }

        public IEnumerable<Patente> GetEffectivePatents(Guid userId)
        {
            var usuario = GetByIdWithRelations(userId);
            if (usuario == null)
                return Enumerable.Empty<Patente>();

            return usuario.ObtenerPatentesEfectivas();
        }

        public PermissionsummaryDto GetPermissionSummary(Guid userId)
        {
            var usuario = GetByIdWithRelations(userId);
            if (usuario == null)
                return null;

            var directPatents = usuario.ObtenerPatentesDirectas().ToList();
            var inheritedPatents = usuario.ObtenerPatentesHeredadas().ToList();
            var effectivePatents = usuario.ObtenerPatentesEfectivas().ToList();

            return new PermissionsummaryDto
            {
                UsuarioId = userId,
                DirectPatentsCount = directPatents.Count,
                InheritedPatentsCount = inheritedPatents.Count,
                TotalEffectivePatentsCount = effectivePatents.Count,
                FamiliasCount = usuario.Familias?.Count ?? 0,
                DirectPatents = directPatents,
                InheritedPatents = inheritedPatents,
                Familias = usuario.Familias?.ToList() ?? new List<Familia>()
            };
        }

        public void UpdateUserWithRelations(Usuario usuario, IEnumerable<Guid> familiaIds, IEnumerable<Guid> patenteIds)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // Load existing user with relations
            var existingUser = GetByIdWithRelations(usuario.Id);
            if (existingUser == null)
                throw new InvalidOperationException($"Usuario with ID {usuario.Id} not found");

            // Update basic properties
            existingUser.Nombre = usuario.Nombre;
            existingUser.NombreUsuario = usuario.NombreUsuario;
            existingUser.Email = usuario.Email;
            existingUser.Activo = usuario.Activo;
            existingUser.DebeRenovarClave = usuario.DebeRenovarClave;
            
            // Update password if provided
            if (!string.IsNullOrEmpty(usuario.ClaveHash))
            {
                existingUser.ClaveHash = usuario.ClaveHash;
                existingUser.Salt = usuario.Salt;
            }

            // Update family assignments
            existingUser.Familias.Clear();
            if (familiaIds != null)
            {
                var familias = _context.Familias.Where(f => familiaIds.Contains(f.Id)).ToList();
                foreach (var familia in familias)
                {
                    existingUser.Familias.Add(familia);
                }
            }

            // Update patent assignments
            existingUser.Patentes.Clear();
            if (patenteIds != null)
            {
                var patentes = _context.Patentes.Where(p => patenteIds.Contains(p.Id)).ToList();
                foreach (var patente in patentes)
                {
                    existingUser.Patentes.Add(patente);
                }
            }

            existingUser.ModifiedAt = DateTime.Now;
        }

        public void AddUserWithRelations(Usuario usuario, IEnumerable<Guid> familiaIds, IEnumerable<Guid> patenteIds)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // Assign families
            if (familiaIds != null)
            {
                var familias = _context.Familias.Where(f => familiaIds.Contains(f.Id)).ToList();
                foreach (var familia in familias)
                {
                    usuario.Familias.Add(familia);
                }
            }

            // Assign patents
            if (patenteIds != null)
            {
                var patentes = _context.Patentes.Where(p => patenteIds.Contains(p.Id)).ToList();
                foreach (var patente in patentes)
                {
                    usuario.Patentes.Add(patente);
                }
            }

            Add(usuario);
        }

        public IEnumerable<Usuario> GetUsersByFamily(Guid familiaId)
        {
            return _dbSet
                .Where(u => u.Familias.Any(f => f.Id == familiaId))
                .Include(u => u.Familias)
                .Include(u => u.Patentes)
                .ToList();
        }

        public IEnumerable<Usuario> GetUsersByPatent(Guid patenteId)
        {
            // Users with direct patent assignment
            var directUsers = _dbSet
                .Where(u => u.Patentes.Any(p => p.Id == patenteId))
                .ToList();

            // Users with patent through families (more complex query)
            var familyUsers = _dbSet
                .Where(u => u.Familias.Any(f => 
                    f.Patentes.Any(p => p.Id == patenteId) ||
                    f.FamiliasHijas.Any(fh => fh.Patentes.Any(p => p.Id == patenteId))))
                .ToList();

            // Combine and deduplicate
            return directUsers.Union(familyUsers).Distinct().ToList();
        }
    }
}