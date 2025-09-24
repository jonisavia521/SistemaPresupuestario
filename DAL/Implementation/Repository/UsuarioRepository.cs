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
        /// <summary>
        /// Obtiene usuario por nombre de usuario (login)
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <returns>Usuario encontrado o null</returns>
        Usuario GetByEmailAsync(string usuario);

        /// <summary>
        /// Obtiene usuario por ID con todas sus relaciones cargadas
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario con relaciones cargadas</returns>
        Usuario GetByIdWithRelations(Guid id);

        /// <summary>
        /// Obtiene lista paginada de usuarios con filtros
        /// </summary>
        /// <param name="filtroUsuario">Filtro por nombre de usuario (parcial)</param>
        /// <param name="filtroNombre">Filtro por nombre completo (parcial)</param>
        /// <param name="page">Número de página (base 1)</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <param name="total">Total de registros que coinciden con filtros</param>
        /// <returns>Lista paginada de usuarios</returns>
        List<Usuario> GetPaged(string filtroUsuario, string filtroNombre, int page, int pageSize, out int total);

        /// <summary>
        /// Verifica si existe un nombre de usuario (excluyendo opcionalmente un ID)
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <param name="excludeId">ID a excluir de la búsqueda (para edición)</param>
        /// <returns>True si existe</returns>
        bool ExistsUserName(string usuario, Guid? excludeId = null);

        /// <summary>
        /// Agrega usuario con sus relaciones de familias y patentes en una transacción
        /// </summary>
        /// <param name="entity">Entidad usuario</param>
        /// <param name="familiasIds">IDs de familias a asignar</param>
        /// <param name="patentesIds">IDs de patentes a asignar</param>
        void AddUserWithRelations(Usuario entity, IEnumerable<Guid> familiasIds, IEnumerable<Guid> patentesIds);

        /// <summary>
        /// Actualiza usuario con sus relaciones en una transacción
        /// </summary>
        /// <param name="entity">Entidad usuario</param>
        /// <param name="familiasIds">IDs de familias a asignar</param>
        /// <param name="patentesIds">IDs de patentes a asignar</param>
        /// <param name="timestamp">Timestamp para control de concurrencia</param>
        void UpdateUserWithRelations(Usuario entity, IEnumerable<Guid> familiasIds, IEnumerable<Guid> patentesIds, byte[] timestamp);

        /// <summary>
        /// Elimina usuario verificando timestamp de concurrencia
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="timestamp">Timestamp para control de concurrencia</param>
        void Remove(Guid id, byte[] timestamp);

        /// <summary>
        /// Obtiene los permisos efectivos de un usuario (directos + heredados)
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de patentes efectivas con información de origen</returns>
        List<dynamic> GetEffectivePermissions(Guid userId);
    }

    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SistemaPresupuestario context) : base(context) { }

        public Usuario GetByEmailAsync(string usuario)
        {
            return _dbSet.FirstOrDefault(u => u.Usuario1 == usuario);
        }

        public Usuario GetByIdWithRelations(Guid id)
        {
            return _dbSet
                .Include(u => u.Usuario_Familia.Select(uf => uf.Familia))
                .Include(u => u.Usuario_Patente.Select(up => up.Patente))
                .FirstOrDefault(u => u.IdUsuario == id);
        }

        public List<Usuario> GetPaged(string filtroUsuario, string filtroNombre, int page, int pageSize, out int total)
        {
            var query = _dbSet.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filtroUsuario))
            {
                query = query.Where(u => u.Usuario1.Contains(filtroUsuario));
            }

            if (!string.IsNullOrEmpty(filtroNombre))
            {
                query = query.Where(u => u.Nombre.Contains(filtroNombre));
            }

            total = query.Count();

            return query
                .OrderBy(u => u.Usuario1)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.Usuario_Familia)
                .Include(u => u.Usuario_Patente)
                .ToList();
        }

        public bool ExistsUserName(string usuario, Guid? excludeId = null)
        {
            var query = _dbSet.Where(u => u.Usuario1 == usuario);
            
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.IdUsuario != excludeId.Value);
            }

            return query.Any();
        }

        public void AddUserWithRelations(Usuario entity, IEnumerable<Guid> familiasIds, IEnumerable<Guid> patentesIds)
        {
            // Agregar usuario
            _dbSet.Add(entity);

            // Agregar relaciones con familias
            if (familiasIds != null)
            {
                foreach (var familiaId in familiasIds)
                {
                    _context.Usuario_Familia.Add(new Usuario_Familia
                    {
                        IdUsuario = entity.IdUsuario,
                        IdFamilia = familiaId
                    });
                }
            }

            // Agregar relaciones con patentes
            if (patentesIds != null)
            {
                foreach (var patenteId in patentesIds)
                {
                    _context.Usuario_Patente.Add(new Usuario_Patente
                    {
                        IdUsuario = entity.IdUsuario,
                        IdPatente = patenteId
                    });
                }
            }
        }

        public void UpdateUserWithRelations(Usuario entity, IEnumerable<Guid> familiasIds, IEnumerable<Guid> patentesIds, byte[] timestamp)
        {
            var existing = _dbSet.FirstOrDefault(u => u.IdUsuario == entity.IdUsuario);
            if (existing == null)
                throw new InvalidOperationException($"Usuario con ID {entity.IdUsuario} no encontrado");

            // Verificar concurrencia
            if (!timestamp.SequenceEqual(existing.timestamp))
                throw new InvalidOperationException("El usuario ha sido modificado por otro usuario");

            // Actualizar propiedades básicas
            existing.Nombre = entity.Nombre;
            existing.Usuario1 = entity.Usuario1;
            if (!string.IsNullOrEmpty(entity.Clave))
            {
                existing.Clave = entity.Clave;
            }

            // Actualizar relaciones con familias
            var existingFamilias = _context.Usuario_Familia.Where(uf => uf.IdUsuario == entity.IdUsuario).ToList();
            _context.Usuario_Familia.RemoveRange(existingFamilias);

            if (familiasIds != null)
            {
                foreach (var familiaId in familiasIds)
                {
                    _context.Usuario_Familia.Add(new Usuario_Familia
                    {
                        IdUsuario = entity.IdUsuario,
                        IdFamilia = familiaId
                    });
                }
            }

            // Actualizar relaciones con patentes
            var existingPatentes = _context.Usuario_Patente.Where(up => up.IdUsuario == entity.IdUsuario).ToList();
            _context.Usuario_Patente.RemoveRange(existingPatentes);

            if (patentesIds != null)
            {
                foreach (var patenteId in patentesIds)
                {
                    _context.Usuario_Patente.Add(new Usuario_Patente
                    {
                        IdUsuario = entity.IdUsuario,
                        IdPatente = patenteId
                    });
                }
            }
        }

        public void Remove(Guid id, byte[] timestamp)
        {
            var entity = _dbSet.FirstOrDefault(u => u.IdUsuario == id);
            if (entity == null)
                throw new InvalidOperationException($"Usuario con ID {id} no encontrado");

            // Verificar concurrencia
            if (!timestamp.SequenceEqual(entity.timestamp))
                throw new InvalidOperationException("El usuario ha sido modificado por otro usuario");

            // Eliminar relaciones primero
            var familias = _context.Usuario_Familia.Where(uf => uf.IdUsuario == id);
            _context.Usuario_Familia.RemoveRange(familias);

            var patentes = _context.Usuario_Patente.Where(up => up.IdUsuario == id);
            _context.Usuario_Patente.RemoveRange(patentes);

            // Eliminar usuario
            _dbSet.Remove(entity);
        }

        public List<dynamic> GetEffectivePermissions(Guid userId)
        {
            // DECISIÓN: Usar consulta SQL directa para mejor performance en cálculo de permisos efectivos
            // Se podría implementar con LINQ pero sería menos eficiente para jerarquías profundas
            
            var sql = @"
                WITH FamiliasRecursivas AS (
                    -- Familias asignadas directamente al usuario
                    SELECT f.IdFamilia, f.Nombre, 0 as Nivel, 
                           CAST(f.Nombre AS NVARCHAR(MAX)) as Ruta
                    FROM Familia f
                    INNER JOIN Usuario_Familia uf ON f.IdFamilia = uf.IdFamilia
                    WHERE uf.IdUsuario = @userId
                    
                    UNION ALL
                    
                    -- Familias hijas recursivamente
                    SELECT f.IdFamilia, f.Nombre, fr.Nivel + 1,
                           fr.Ruta + ' > ' + f.Nombre
                    FROM Familia f
                    INNER JOIN Familia_Familia ff ON f.IdFamilia = ff.IdFamiliaHijo
                    INNER JOIN FamiliasRecursivas fr ON ff.IdFamiliaPadre = fr.IdFamilia
                    WHERE fr.Nivel < 10 -- Prevenir ciclos infinitos
                ),
                PatentesEfectivas AS (
                    -- Patentes directas del usuario
                    SELECT p.IdPatente, p.Nombre, p.Vista, 'Directo' as Origen, 0 as Nivel
                    FROM Patente p
                    INNER JOIN Usuario_Patente up ON p.IdPatente = up.IdPatente
                    WHERE up.IdUsuario = @userId
                    
                    UNION
                    
                    -- Patentes heredadas de familias
                    SELECT DISTINCT p.IdPatente, p.Nombre, p.Vista, 
                           'Familia: ' + fr.Ruta as Origen, fr.Nivel + 1
                    FROM Patente p
                    INNER JOIN Familia_Patente fp ON p.IdPatente = fp.IdPatente
                    INNER JOIN FamiliasRecursivas fr ON fp.IdFamilia = fr.IdFamilia
                )
                SELECT IdPatente, Nombre, Vista, Origen, Nivel
                FROM PatentesEfectivas
                ORDER BY Nivel, Nombre";

            return _context.Database.SqlQuery<dynamic>(sql, new System.Data.SqlClient.SqlParameter("@userId", userId)).ToList();
        }
    }
}
