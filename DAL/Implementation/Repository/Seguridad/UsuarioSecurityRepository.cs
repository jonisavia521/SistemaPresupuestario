using DAL.Contracts;
using DAL.Contracts.Seguridad;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Implementation.Repository.Seguridad
{
    /// <summary>
    /// Repositorio extendido de usuarios con operaciones específicas de seguridad
    /// </summary>
    public class UsuarioSecurityRepository : Repository<Usuario>, IUsuarioSecurityRepository
    {
        public UsuarioSecurityRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public async Task<Usuario> GetByUsernameAsync(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return null;

            return await _dbSet
                .FirstOrDefaultAsync(u => u.Usuario1.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> ExistsUsernameAsync(string nombreUsuario, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return false;

            var query = _dbSet.Where(u => u.Usuario1.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase));
            
            if (excludeId.HasValue)
                query = query.Where(u => u.IdUsuario != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<Usuario> GetWithPermissionsAsync(Guid id)
        {
            return await _dbSet
                .Include(u => u.Usuario_Familia.Select(uf => uf.Familia))
                .Include(u => u.Usuario_Patente.Select(up => up.Patente))
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<(IEnumerable<Usuario> usuarios, int total)> GetPagedAsync(
            string filtroNombre = null, string filtroUsuario = null, 
            bool soloActivos = true, int skip = 0, int take = 50)
        {
            var query = _dbSet.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(filtroNombre))
                query = query.Where(u => u.Nombre.Contains(filtroNombre));

            if (!string.IsNullOrWhiteSpace(filtroUsuario))
                query = query.Where(u => u.Usuario1.Contains(filtroUsuario));

            // TODO: Agregar filtro por activos cuando se implemente el campo

            // Obtener total antes de paginar
            var total = await query.CountAsync();

            // Aplicar paginación y ordenamiento
            var usuarios = await query
                .OrderBy(u => u.Nombre)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (usuarios, total);
        }

        public async Task<IEnumerable<Familia>> GetFamiliasAsignadasAsync(Guid usuarioId)
        {
            return await _context.Usuario_Familia
                .Where(uf => uf.IdUsuario == usuarioId)
                .Select(uf => uf.Familia)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patente>> GetPatentesAsignadasAsync(Guid usuarioId)
        {
            return await _context.Usuario_Patente
                .Where(up => up.IdUsuario == usuarioId)
                .Select(up => up.Patente)
                .ToListAsync();
        }

        public async Task AsignarFamiliasAsync(Guid usuarioId, IEnumerable<Guid> familiasIds)
        {
            // Remover asignaciones existentes
            var asignacionesExistentes = await _context.Usuario_Familia
                .Where(uf => uf.IdUsuario == usuarioId)
                .ToListAsync();

            _context.Usuario_Familia.RemoveRange(asignacionesExistentes);

            // Agregar nuevas asignaciones
            if (familiasIds != null && familiasIds.Any())
            {
                var nuevasAsignaciones = familiasIds.Select(familiaId => new Usuario_Familia
                {
                    IdUsuario = usuarioId,
                    IdFamilia = familiaId
                });

                _context.Usuario_Familia.AddRange(nuevasAsignaciones);
            }
        }

        public async Task AsignarPatentesAsync(Guid usuarioId, IEnumerable<Guid> patentesIds)
        {
            // Remover asignaciones existentes
            var asignacionesExistentes = await _context.Usuario_Patente
                .Where(up => up.IdUsuario == usuarioId)
                .ToListAsync();

            _context.Usuario_Patente.RemoveRange(asignacionesExistentes);

            // Agregar nuevas asignaciones
            if (patentesIds != null && patentesIds.Any())
            {
                var nuevasAsignaciones = patentesIds.Select(patenteId => new Usuario_Patente
                {
                    IdUsuario = usuarioId,
                    IdPatente = patenteId
                });

                _context.Usuario_Patente.AddRange(nuevasAsignaciones);
            }
        }

        public async Task<(int familiasDirectas, int patentesDirectas)> GetPermissionCountsAsync(Guid usuarioId)
        {
            var familiasCount = await _context.Usuario_Familia
                .CountAsync(uf => uf.IdUsuario == usuarioId);

            var patentesCount = await _context.Usuario_Patente
                .CountAsync(up => up.IdUsuario == usuarioId);

            return (familiasCount, patentesCount);
        }
    }
}