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
    /// Repositorio para familias de permisos con operaciones jerárquicas
    /// </summary>
    public class FamiliaRepository : Repository<Familia>, IFamiliaRepository
    {
        public FamiliaRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public async Task<Familia> GetWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(f => f.RelacionesComoPadre.Select(r => r.FamiliaHijo))
                .Include(f => f.RelacionesComoHijo.Select(r => r.FamiliaPadre))
                .Include(f => f.Familia_Patente.Select(fp => fp.Patente))
                .FirstOrDefaultAsync(f => f.IdFamilia == id);
        }

        public async Task<IEnumerable<Familia>> GetAllWithHierarchyAsync()
        {
            return await _dbSet
                .Include(f => f.RelacionesComoPadre.Select(r => r.FamiliaHijo))
                .Include(f => f.RelacionesComoHijo.Select(r => r.FamiliaPadre))
                .Include(f => f.Familia_Patente.Select(fp => fp.Patente))
                .ToListAsync();
        }

        public async Task<IEnumerable<Familia>> GetFamiliasHijasAsync(Guid familiaId)
        {
            return await _context.Familia_Familia
                .Where(ff => ff.IdFamilia == familiaId)
                .Select(ff => ff.FamiliaHijo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Familia>> GetFamiliasPadreAsync(Guid familiaId)
        {
            return await _context.Familia_Familia
                .Where(ff => ff.IdFamiliaHijo == familiaId)
                .Select(ff => ff.FamiliaPadre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patente>> GetPatentesDirectasAsync(Guid familiaId)
        {
            return await _context.Familia_Patente
                .Where(fp => fp.IdFamilia == familiaId)
                .Select(fp => fp.Patente)
                .ToListAsync();
        }

        public async Task<bool> WouldCreateCycleAsync(Guid familiaId, IEnumerable<Guid> familiasPadreIds)
        {
            if (familiasPadreIds == null || !familiasPadreIds.Any())
                return false;

            // Verificar si alguna de las familias padre es descendiente de la familia actual
            var descendientes = await GetDescendientesAsync(familiaId);
            var descendientesIds = descendientes.Select(d => d.IdFamilia).ToHashSet();

            return familiasPadreIds.Any(padreId => descendientesIds.Contains(padreId) || padreId == familiaId);
        }

        public async Task<IEnumerable<Familia>> GetDescendientesAsync(Guid familiaId)
        {
            var visitadas = new HashSet<Guid>();
            var resultado = new List<Familia>();
            await GetDescendientesRecursiveAsync(familiaId, visitadas, resultado);
            return resultado;
        }

        private async Task GetDescendientesRecursiveAsync(Guid familiaId, HashSet<Guid> visitadas, List<Familia> resultado)
        {
            if (visitadas.Contains(familiaId))
                return;

            visitadas.Add(familiaId);

            var hijas = await _context.Familia_Familia
                .Where(ff => ff.IdFamilia == familiaId)
                .Select(ff => ff.FamiliaHijo)
                .ToListAsync();

            resultado.AddRange(hijas);

            foreach (var hija in hijas)
            {
                await GetDescendientesRecursiveAsync(hija.IdFamilia, visitadas, resultado);
            }
        }

        public async Task<IEnumerable<Familia>> GetAscendientesAsync(Guid familiaId)
        {
            var visitadas = new HashSet<Guid>();
            var resultado = new List<Familia>();
            await GetAscendientesRecursiveAsync(familiaId, visitadas, resultado);
            return resultado;
        }

        private async Task GetAscendientesRecursiveAsync(Guid familiaId, HashSet<Guid> visitadas, List<Familia> resultado)
        {
            if (visitadas.Contains(familiaId))
                return;

            visitadas.Add(familiaId);

            var padres = await _context.Familia_Familia
                .Where(ff => ff.IdFamiliaHijo == familiaId)
                .Select(ff => ff.FamiliaPadre)
                .ToListAsync();

            resultado.AddRange(padres);

            foreach (var padre in padres)
            {
                await GetAscendientesRecursiveAsync(padre.IdFamilia, visitadas, resultado);
            }
        }

        public async Task EstablecerRelacionesAsync(Guid familiaId, IEnumerable<Guid> familiasPadreIds, IEnumerable<Guid> familiasHijasIds)
        {
            // Remover relaciones existentes como hijo
            var relacionesPadreExistentes = await _context.Familia_Familia
                .Where(ff => ff.IdFamiliaHijo == familiaId)
                .ToListAsync();
            _context.Familia_Familia.RemoveRange(relacionesPadreExistentes);

            // Remover relaciones existentes como padre
            var relacionesHijaExistentes = await _context.Familia_Familia
                .Where(ff => ff.IdFamilia == familiaId)
                .ToListAsync();
            _context.Familia_Familia.RemoveRange(relacionesHijaExistentes);

            // Agregar nuevas relaciones como hijo (esta familia es hija)
            if (familiasPadreIds != null && familiasPadreIds.Any())
            {
                var nuevasRelacionesPadre = familiasPadreIds.Select(padreId => new Familia_Familia
                {
                    IdFamilia = padreId,
                    IdFamiliaHijo = familiaId
                });
                _context.Familia_Familia.AddRange(nuevasRelacionesPadre);
            }

            // Agregar nuevas relaciones como padre (esta familia es padre)
            if (familiasHijasIds != null && familiasHijasIds.Any())
            {
                var nuevasRelacionesHijas = familiasHijasIds.Select(hijaId => new Familia_Familia
                {
                    IdFamilia = familiaId,
                    IdFamiliaHijo = hijaId
                });
                _context.Familia_Familia.AddRange(nuevasRelacionesHijas);
            }
        }

        public async Task AsignarPatentesAsync(Guid familiaId, IEnumerable<Guid> patentesIds)
        {
            // Remover asignaciones existentes
            var asignacionesExistentes = await _context.Familia_Patente
                .Where(fp => fp.IdFamilia == familiaId)
                .ToListAsync();
            _context.Familia_Patente.RemoveRange(asignacionesExistentes);

            // Agregar nuevas asignaciones
            if (patentesIds != null && patentesIds.Any())
            {
                var nuevasAsignaciones = patentesIds.Select(patenteId => new Familia_Patente
                {
                    IdFamilia = familiaId,
                    IdPatente = patenteId
                });
                _context.Familia_Patente.AddRange(nuevasAsignaciones);
            }
        }
    }
}