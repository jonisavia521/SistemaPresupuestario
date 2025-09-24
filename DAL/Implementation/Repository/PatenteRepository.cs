using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Implementation.Repository
{
    public interface IPatenteRepository : IRepository<Patente>
    {
        Task<IEnumerable<Patente>> GetAllWithRelationsAsync();
        Task<Patente> GetByIdWithRelationsAsync(Guid id);
        Task<List<Patente>> GetPatentesDisponiblesAsync();
        Task<List<Patente>> GetPatentesByFamiliaAsync(Guid familiaId);
    }

    public class PatenteRepository : Repository<Patente>, IPatenteRepository
    {
        public PatenteRepository(SistemaPresupuestario context) : base(context) { }

        public async Task<IEnumerable<Patente>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(p => p.Familia_Patente.Select(fp => fp.Familia))
                .Include(p => p.Usuario_Patente.Select(up => up.Usuario))
                .ToListAsync();
        }

        public async Task<Patente> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Familia_Patente.Select(fp => fp.Familia))
                .Include(p => p.Usuario_Patente.Select(up => up.Usuario))
                .FirstOrDefaultAsync(p => p.IdPatente == id);
        }

        public async Task<List<Patente>> GetPatentesDisponiblesAsync()
        {
            return await _dbSet
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<List<Patente>> GetPatentesByFamiliaAsync(Guid familiaId)
        {
            return await _dbSet
                .Where(p => p.Familia_Patente.Any(fp => fp.IdFamilia == familiaId))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }
    }
}