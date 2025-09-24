using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Implementation.Repository
{
    public interface IFamiliaRepository : IRepository<Familia>
    {
        Task<IEnumerable<Familia>> GetAllWithRelationsAsync();
        Task<Familia> GetByIdWithRelationsAsync(Guid id);
        Task<List<Familia>> GetFamiliasJerarquicasAsync();
        Task<List<Guid>> GetPatentesFromFamiliaAsync(Guid familiaId);
        Task<bool> HasCircularReferenceAsync(Guid familiaId, Guid potentialParentId);
    }

    public class FamiliaRepository : Repository<Familia>, IFamiliaRepository
    {
        public FamiliaRepository(SistemaPresupuestario context) : base(context) { }

        public async Task<IEnumerable<Familia>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(f => f.RelacionesComoPadre)
                .Include(f => f.RelacionesComoHijo)
                .Include(f => f.Familia_Patente.Select(fp => fp.Patente))
                .ToListAsync();
        }

        public async Task<Familia> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(f => f.RelacionesComoPadre)
                .Include(f => f.RelacionesComoHijo)
                .Include(f => f.Familia_Patente.Select(fp => fp.Patente))
                .FirstOrDefaultAsync(f => f.IdFamilia == id);
        }

        public async Task<List<Familia>> GetFamiliasJerarquicasAsync()
        {
            return await _dbSet
                .Include(f => f.RelacionesComoPadre)
                .Include(f => f.Familia_Patente.Select(fp => fp.Patente))
                .OrderBy(f => f.Nombre)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetPatentesFromFamiliaAsync(Guid familiaId)
        {
            var patentes = new List<Guid>();
            var visitedFamilias = new HashSet<Guid>();
            await GetPatentesRecursive(familiaId, patentes, visitedFamilias);
            return patentes.Distinct().ToList();
        }

        private async Task GetPatentesRecursive(Guid familiaId, List<Guid> patentes, HashSet<Guid> visitedFamilias)
        {
            if (visitedFamilias.Contains(familiaId)) return; // Evitar ciclos
            visitedFamilias.Add(familiaId);

            // Obtener patentes directas
            var patentesDirectas = await _context.Familia_Patente
                .Where(fp => fp.IdFamilia == familiaId)
                .Select(fp => fp.IdPatente)
                .ToListAsync();
            patentes.AddRange(patentesDirectas);

            // Obtener familias hijas y procesar recursivamente
            var familiasHijas = await _context.Familia_Familia
                .Where(ff => ff.IdFamiliaPadre == familiaId)
                .Select(ff => ff.IdFamiliaHijo)
                .ToListAsync();

            foreach (var familiaHija in familiasHijas)
            {
                await GetPatentesRecursive(familiaHija, patentes, visitedFamilias);
            }
        }

        public async Task<bool> HasCircularReferenceAsync(Guid familiaId, Guid potentialParentId)
        {
            var visitedFamilias = new HashSet<Guid>();
            return await CheckCircularReferenceRecursive(familiaId, potentialParentId, visitedFamilias);
        }

        private async Task<bool> CheckCircularReferenceRecursive(Guid currentFamiliaId, Guid targetParentId, HashSet<Guid> visitedFamilias)
        {
            if (visitedFamilias.Contains(currentFamiliaId)) return false;
            if (currentFamiliaId == targetParentId) return true;

            visitedFamilias.Add(currentFamiliaId);

            var padres = await _context.Familia_Familia
                .Where(ff => ff.IdFamiliaHijo == currentFamiliaId)
                .Select(ff => ff.IdFamiliaPadre)
                .ToListAsync();

            foreach (var padre in padres)
            {
                if (await CheckCircularReferenceRecursive(padre, targetParentId, visitedFamilias))
                    return true;
            }

            return false;
        }
    }
}