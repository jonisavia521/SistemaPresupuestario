using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementation.Repository
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario GetByEmailAsync(string email);
        Task<Usuario> GetByIdWithRelationsAsync(Guid id);
        Task<bool> ExistsUserNameAsync(string userName, Guid? excludeId = null);
        Task<PagedUsuarioResult> GetPagedAsync(string filter, int page, int pageSize);
        Task<List<Guid>> GetEffectivePatentesAsync(Guid userId);
        void UpdateUserWithRelations(Usuario usuario, List<Guid> familiaIds, List<Guid> patenteIds);
    }

    public class PagedUsuarioResult
    {
        public IEnumerable<Usuario> Items { get; set; }
        public int TotalItems { get; set; }
    }

    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SistemaPresupuestario context) : base(context) { }

        public Usuario GetByEmailAsync(string usuario)
        {
            return _dbSet.FirstOrDefault(u => u.Usuario1 == usuario);
        }

        public async Task<Usuario> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(u => u.Usuario_Familia.Select(uf => uf.Familia))
                .Include(u => u.Usuario_Patente.Select(up => up.Patente))
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        public async Task<bool> ExistsUserNameAsync(string userName, Guid? excludeId = null)
        {
            var query = _dbSet.Where(u => u.Usuario1 == userName);
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.IdUsuario != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<PagedUsuarioResult> GetPagedAsync(string filter, int page, int pageSize)
        {
            var query = _dbSet.AsQueryable();

            // Aplicar filtro si se proporciona
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(u => u.Nombre.Contains(filter) || u.Usuario1.Contains(filter));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedUsuarioResult
            {
                Items = items,
                TotalItems = total
            };
        }

        public async Task<List<Guid>> GetEffectivePatentesAsync(Guid userId)
        {
            var patentesDirectas = await _context.Usuario_Patente
                .Where(up => up.IdUsuario == userId)
                .Select(up => up.IdPatente)
                .ToListAsync();

            var familiasUsuario = await _context.Usuario_Familia
                .Where(uf => uf.IdUsuario == userId)
                .Select(uf => uf.IdFamilia)
                .ToListAsync();

            var patentesHeredadas = await GetPatentesFromFamilias(familiasUsuario);

            return patentesDirectas.Union(patentesHeredadas).Distinct().ToList();
        }

        private async Task<List<Guid>> GetPatentesFromFamilias(List<Guid> familiaIds)
        {
            if (!familiaIds.Any()) return new List<Guid>();

            var patentes = new List<Guid>();
            var visitedFamilias = new HashSet<Guid>();

            foreach (var familiaId in familiaIds)
            {
                await GetPatentesFromFamiliaRecursive(familiaId, patentes, visitedFamilias);
            }

            return patentes;
        }

        private async Task GetPatentesFromFamiliaRecursive(Guid familiaId, List<Guid> patentes, HashSet<Guid> visitedFamilias)
        {
            if (visitedFamilias.Contains(familiaId)) return; // Evitar ciclos
            visitedFamilias.Add(familiaId);

            // Obtener patentes directas de la familia
            var patentesDirectas = await _context.Familia_Patente
                .Where(fp => fp.IdFamilia == familiaId)
                .Select(fp => fp.IdPatente)
                .ToListAsync();
            patentes.AddRange(patentesDirectas);

            // Obtener familias hijas
            var familiasHijas = await _context.Familia_Familia
                .Where(ff => ff.IdFamiliaPadre == familiaId)
                .Select(ff => ff.IdFamiliaHijo)
                .ToListAsync();

            // Recursión para familias hijas
            foreach (var familiaHija in familiasHijas)
            {
                await GetPatentesFromFamiliaRecursive(familiaHija, patentes, visitedFamilias);
            }
        }

        public void UpdateUserWithRelations(Usuario usuario, List<Guid> familiaIds, List<Guid> patenteIds)
        {
            var existingUser = _dbSet
                .Include(u => u.Usuario_Familia)
                .Include(u => u.Usuario_Patente)
                .FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);

            if (existingUser == null) return;

            // Actualizar datos básicos
            existingUser.Nombre = usuario.Nombre;
            existingUser.Usuario1 = usuario.Usuario1;
            if (!string.IsNullOrEmpty(usuario.Clave))
                existingUser.Clave = usuario.Clave;

            // Actualizar relaciones con familias
            existingUser.Usuario_Familia.Clear();
            foreach (var familiaId in familiaIds)
            {
                existingUser.Usuario_Familia.Add(new Usuario_Familia
                {
                    IdUsuario = usuario.IdUsuario,
                    IdFamilia = familiaId
                });
            }

            // Actualizar relaciones con patentes
            existingUser.Usuario_Patente.Clear();
            foreach (var patenteId in patenteIds)
            {
                existingUser.Usuario_Patente.Add(new Usuario_Patente
                {
                    IdUsuario = usuario.IdUsuario,
                    IdPatente = patenteId
                });
            }
        }
    }
}
