using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Implementation.Repository
{
    public interface IFamiliaRepository : IRepository<Familia>
    {
        /// <summary>
        /// Obtiene toda la jerarquía de familias optimizada
        /// </summary>
        /// <returns>Lista completa de familias con relaciones padre-hijo</returns>
        List<Familia> GetAllHierarchy();

        /// <summary>
        /// Obtiene familias por lista de IDs con sus patentes
        /// </summary>
        /// <param name="ids">IDs de familias</param>
        /// <returns>Lista de familias con patentes cargadas</returns>
        List<Familia> GetByIds(IEnumerable<Guid> ids);

        /// <summary>
        /// Obtiene lista plana de todas las familias (sin jerarquía)
        /// </summary>
        /// <returns>Lista simple de familias</returns>
        List<Familia> GetFlat();

        /// <summary>
        /// Verifica si crear una relación padre-hijo generaría un ciclo
        /// </summary>
        /// <param name="padreId">ID de la familia padre</param>
        /// <param name="hijoId">ID de la familia hijo</param>
        /// <returns>True si generaría un ciclo</returns>
        bool WouldCreateCycle(Guid padreId, Guid hijoId);

        /// <summary>
        /// Obtiene todos los descendientes de una familia
        /// </summary>
        /// <param name="familiaId">ID de la familia padre</param>
        /// <returns>Lista de IDs de descendientes</returns>
        List<Guid> GetAllDescendants(Guid familiaId);

        /// <summary>
        /// Obtiene todas las patentes efectivas de una familia (directas + heredadas)
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <returns>Lista de patentes únicas</returns>
        List<Patente> GetEffectivePatentes(Guid familiaId);
    }

    public class FamiliaRepository : Repository<Familia>, IFamiliaRepository
    {
        public FamiliaRepository(SistemaPresupuestario context) : base(context) { }

        public List<Familia> GetAllHierarchy()
        {
            return _dbSet
                .Include(f => f.RelacionesComoPadre.Select(r => r.FamiliaHijo))
                .Include(f => f.RelacionesComoHijo.Select(r => r.FamiliaPadre))
                .Include(f => f.Familia_Patente.Select(fp => fp.Patente))
                .ToList();
        }

        public List<Familia> GetByIds(IEnumerable<Guid> ids)
        {
            return _dbSet
                .Where(f => ids.Contains(f.IdFamilia))
                .Include(f => f.Familia_Patente.Select(fp => fp.Patente))
                .ToList();
        }

        public List<Familia> GetFlat()
        {
            return _dbSet
                .OrderBy(f => f.Nombre)
                .ToList();
        }

        public bool WouldCreateCycle(Guid padreId, Guid hijoId)
        {
            // DECISIÓN: Usar DFS para detectar ciclos antes de crear relación
            if (padreId == hijoId)
                return true;

            var visited = new HashSet<Guid>();
            return HasCycleDFS(hijoId, padreId, visited);
        }

        private bool HasCycleDFS(Guid currentId, Guid targetId, HashSet<Guid> visited)
        {
            if (visited.Contains(currentId))
                return false; // Ya visitado en esta ruta, no hay ciclo

            visited.Add(currentId);

            // Obtener hijos del nodo actual
            var childIds = _context.Familia_Familia
                .Where(ff => ff.IdFamiliaPadre == currentId)
                .Select(ff => ff.IdFamiliaHijo)
                .ToList();

            foreach (var childId in childIds)
            {
                if (childId == targetId)
                    return true; // Encontramos el objetivo, hay ciclo

                if (HasCycleDFS(childId, targetId, visited))
                    return true;
            }

            return false;
        }

        public List<Guid> GetAllDescendants(Guid familiaId)
        {
            var descendants = new List<Guid>();
            var toProcess = new Queue<Guid>();
            var processed = new HashSet<Guid>();

            toProcess.Enqueue(familiaId);
            processed.Add(familiaId);

            while (toProcess.Count > 0)
            {
                var currentId = toProcess.Dequeue();
                
                var children = _context.Familia_Familia
                    .Where(ff => ff.IdFamiliaPadre == currentId)
                    .Select(ff => ff.IdFamiliaHijo)
                    .ToList();

                foreach (var childId in children)
                {
                    if (!processed.Contains(childId))
                    {
                        descendants.Add(childId);
                        toProcess.Enqueue(childId);
                        processed.Add(childId);
                    }
                }
            }

            return descendants;
        }

        public List<Patente> GetEffectivePatentes(Guid familiaId)
        {
            // DECISIÓN: Usar SQL recursivo para mejor performance con jerarquías profundas
            var sql = @"
                WITH FamiliasRecursivas AS (
                    -- Familia raíz
                    SELECT IdFamilia, 0 as Nivel
                    FROM Familia
                    WHERE IdFamilia = @familiaId
                    
                    UNION ALL
                    
                    -- Familias hijas recursivamente
                    SELECT f.IdFamilia, fr.Nivel + 1
                    FROM Familia f
                    INNER JOIN Familia_Familia ff ON f.IdFamilia = ff.IdFamiliaHijo
                    INNER JOIN FamiliasRecursivas fr ON ff.IdFamiliaPadre = fr.IdFamilia
                    WHERE fr.Nivel < 10 -- Prevenir ciclos infinitos
                )
                SELECT DISTINCT p.IdPatente, p.Nombre, p.Vista, p.timestamp
                FROM Patente p
                INNER JOIN Familia_Patente fp ON p.IdPatente = fp.IdPatente
                INNER JOIN FamiliasRecursivas fr ON fp.IdFamilia = fr.IdFamilia
                ORDER BY p.Nombre";

            var patentesData = _context.Database.SqlQuery<dynamic>(sql, 
                new System.Data.SqlClient.SqlParameter("@familiaId", familiaId)).ToList();

            // Convertir resultado dinámico a entidades Patente
            var patentes = new List<Patente>();
            foreach (dynamic row in patentesData)
            {
                patentes.Add(new Patente
                {
                    IdPatente = row.IdPatente,
                    Nombre = row.Nombre,
                    Vista = row.Vista,
                    timestamp = row.timestamp
                });
            }

            return patentes;
        }
    }
}