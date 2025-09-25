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
    /// Repositorio para patentes (permisos individuales)
    /// </summary>
    public class PatenteRepository : Repository<Patente>, IPatenteRepository
    {
        public PatenteRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public async Task<Patente> GetWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Familia_Patente.Select(fp => fp.Familia))
                .Include(p => p.Usuario_Patente.Select(up => up.Usuario))
                .FirstOrDefaultAsync(p => p.IdPatente == id);
        }

        public async Task<IEnumerable<Patente>> GetByVistaAsync(string vista)
        {
            if (string.IsNullOrWhiteSpace(vista))
                return new List<Patente>();

            return await _dbSet
                .Where(p => p.Vista.Equals(vista, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<IEnumerable<Patente>> GetAllWithAssignmentInfoAsync(Guid? usuarioId = null)
        {
            // TODO: Esta implementación devuelve todas las patentes
            // En la capa de servicio se agregará la información de asignación
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<(Patente patente, Familia familiaOrigen)>> GetPatentesHeredadasAsync(Guid usuarioId)
        {
            // Obtener patentes a través de las familias asignadas al usuario
            var patentesHeredadas = await _context.Usuario_Familia
                .Where(uf => uf.IdUsuario == usuarioId)
                .SelectMany(uf => uf.Familia.Familia_Patente
                    .Select(fp => new { fp.Patente, uf.Familia }))
                .ToListAsync();

            return patentesHeredadas.Select(ph => (ph.Patente, ph.Familia));
        }

        public async Task<(int total, int conVista, int sinVista)> GetCountsAsync()
        {
            var total = await _dbSet.CountAsync();
            var conVista = await _dbSet.CountAsync(p => !string.IsNullOrEmpty(p.Vista));
            var sinVista = total - conVista;

            return (total, conVista, sinVista);
        }

        public async Task<bool> IsInUseAsync(Guid patenteId)
        {
            var enFamilias = await _context.Familia_Patente
                .AnyAsync(fp => fp.IdPatente == patenteId);

            var enUsuarios = await _context.Usuario_Patente
                .AnyAsync(up => up.IdPatente == patenteId);

            return enFamilias || enUsuarios;
        }

        public async Task<IEnumerable<Familia>> GetFamiliasConPatenteAsync(Guid patenteId)
        {
            return await _context.Familia_Patente
                .Where(fp => fp.IdPatente == patenteId)
                .Select(fp => fp.Familia)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosConPatenteDirectaAsync(Guid patenteId)
        {
            return await _context.Usuario_Patente
                .Where(up => up.IdPatente == patenteId)
                .Select(up => up.Usuario)
                .ToListAsync();
        }
    }
}