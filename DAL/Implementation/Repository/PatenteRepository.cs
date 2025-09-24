using DAL.Contracts;
using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Implementation.Repository
{
    public interface IPatenteRepository : IRepository<Patente>
    {
        /// <summary>
        /// Obtiene todas las patentes ordenadas por nombre
        /// </summary>
        /// <returns>Lista completa de patentes</returns>
        List<Patente> GetAll();

        /// <summary>
        /// Obtiene patentes por lista de IDs
        /// </summary>
        /// <param name="ids">IDs de patentes</param>
        /// <returns>Lista de patentes encontradas</returns>
        List<Patente> GetByIds(IEnumerable<Guid> ids);

        /// <summary>
        /// Obtiene patentes asignadas directamente a un usuario
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de patentes directas</returns>
        List<Patente> GetDirectPatentesForUser(Guid usuarioId);

        /// <summary>
        /// Obtiene patentes disponibles para asignar (no asignadas a un usuario específico)
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Lista de patentes disponibles</returns>
        List<Patente> GetAvailablePatentesForUser(Guid usuarioId);

        /// <summary>
        /// Verifica si una patente está asignada a un usuario (directa o indirectamente)
        /// </summary>
        /// <param name="patenteId">ID de la patente</param>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>True si el usuario tiene la patente</returns>
        bool IsPatienteAssignedToUser(Guid patenteId, Guid usuarioId);

        /// <summary>
        /// Obtiene estadísticas de uso de una patente
        /// </summary>
        /// <param name="patenteId">ID de la patente</param>
        /// <returns>Información de uso (usuarios directos, familias, usuarios totales)</returns>
        dynamic GetPatienteUsageStats(Guid patenteId);
    }

    public class PatenteRepository : Repository<Patente>, IPatenteRepository
    {
        public PatenteRepository(SistemaPresupuestario context) : base(context) { }

        public List<Patente> GetAll()
        {
            return _dbSet
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        public List<Patente> GetByIds(IEnumerable<Guid> ids)
        {
            return _dbSet
                .Where(p => ids.Contains(p.IdPatente))
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        public List<Patente> GetDirectPatentesForUser(Guid usuarioId)
        {
            return _dbSet
                .Where(p => p.Usuario_Patente.Any(up => up.IdUsuario == usuarioId))
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        public List<Patente> GetAvailablePatentesForUser(Guid usuarioId)
        {
            return _dbSet
                .Where(p => !p.Usuario_Patente.Any(up => up.IdUsuario == usuarioId))
                .OrderBy(p => p.Nombre)
                .ToList();
        }

        public bool IsPatienteAssignedToUser(Guid patenteId, Guid usuarioId)
        {
            // Verificar asignación directa
            var directAssignment = _context.Usuario_Patente
                .Any(up => up.IdPatente == patenteId && up.IdUsuario == usuarioId);

            if (directAssignment)
                return true;

            // Verificar asignación indirecta a través de familias
            var sql = @"
                WITH FamiliasUsuario AS (
                    -- Familias asignadas directamente al usuario
                    SELECT f.IdFamilia
                    FROM Familia f
                    INNER JOIN Usuario_Familia uf ON f.IdFamilia = uf.IdFamilia
                    WHERE uf.IdUsuario = @usuarioId
                    
                    UNION ALL
                    
                    -- Familias hijas recursivamente
                    SELECT f.IdFamilia
                    FROM Familia f
                    INNER JOIN Familia_Familia ff ON f.IdFamilia = ff.IdFamiliaHijo
                    INNER JOIN FamiliasUsuario fu ON ff.IdFamiliaPadre = fu.IdFamilia
                )
                SELECT COUNT(*)
                FROM Familia_Patente fp
                INNER JOIN FamiliasUsuario fu ON fp.IdFamilia = fu.IdFamilia
                WHERE fp.IdPatente = @patenteId";

            var count = _context.Database.SqlQuery<int>(sql,
                new System.Data.SqlClient.SqlParameter("@usuarioId", usuarioId),
                new System.Data.SqlClient.SqlParameter("@patenteId", patenteId))
                .FirstOrDefault();

            return count > 0;
        }

        public dynamic GetPatienteUsageStats(Guid patenteId)
        {
            var sql = @"
                SELECT 
                    COUNT(DISTINCT up.IdUsuario) as UsuariosDirectos,
                    COUNT(DISTINCT fp.IdFamilia) as FamiliasAsignadas,
                    (
                        SELECT COUNT(DISTINCT sub_uf.IdUsuario)
                        FROM Usuario_Familia sub_uf
                        INNER JOIN Familia_Patente sub_fp ON sub_uf.IdFamilia = sub_fp.IdFamilia
                        WHERE sub_fp.IdPatente = @patenteId
                    ) as UsuariosIndirectos
                FROM Patente p
                LEFT JOIN Usuario_Patente up ON p.IdPatente = up.IdPatente
                LEFT JOIN Familia_Patente fp ON p.IdPatente = fp.IdPatente
                WHERE p.IdPatente = @patenteId
                GROUP BY p.IdPatente";

            return _context.Database.SqlQuery<dynamic>(sql,
                new System.Data.SqlClient.SqlParameter("@patenteId", patenteId))
                .FirstOrDefault();
        }
    }
}