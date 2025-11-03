using DAL.Implementation.EntityFramework;
using DAL.Infraestructure;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Implementation.Repository
{
    /// <summary>
    /// Repositorio de acceso a datos para Producto
    /// Implementa las operaciones de persistencia específicas de Producto
    /// </summary>
    public class ProductoRepository : Repository<ProductoDM>, IProductoRepository
    {
        public ProductoRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public async Task<ProductoDM> GetByCodigoAsync(string codigo)
        {
            // Buscar en la tabla EF Producto
            var productoEF = await _context.Producto
                .FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (productoEF == null)
                return null;

            // Convertir de entidad EF a entidad de dominio
            return MapearADominio(productoEF);
        }

        public async Task<IEnumerable<ProductoDM>> GetActivosAsync()
        {
            var productosEF = await _context.Producto
                .Where(p => !p.Inhabilitado)
                .ToListAsync();

            return productosEF.Select(MapearADominio);
        }

        public async Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Producto
                    .AnyAsync(p => p.Codigo == codigo && p.ID != excludeId.Value);
            }

            return await _context.Producto
                .AnyAsync(p => p.Codigo == codigo);
        }

        public ProductoDM GetById(Guid id)
        {
            var productoEF = _context.Producto.Find(id);
            if (productoEF == null)
                return null;

            return MapearADominio(productoEF);
        }

        // Sobrescribir métodos base para usar el mapeo personalizado
        public new async Task<IEnumerable<ProductoDM>> GetAllAsync()
        {
            var productosEF = await _context.Producto.ToListAsync();
            return productosEF.Select(MapearADominio);
        }

        public new async Task<ProductoDM> GetByIdAsync(Guid id)
        {
            var productoEF = await _context.Producto.FindAsync(id);
            if (productoEF == null)
                return null;

            return MapearADominio(productoEF);
        }

        public new void Add(ProductoDM entidad)
        {
            var productoEF = MapearAEntityFramework(entidad);
            _context.Producto.Add(productoEF);
        }

        public new void Update(ProductoDM entidad)
        {
            var productoEF = MapearAEntityFramework(entidad);
            
            var existente = _context.Producto.Find(entidad.ID);
            if (existente != null)
            {
                // Actualizar propiedades (excepto FechaAlta y UsuarioAlta que son inmutables)
                existente.Codigo = productoEF.Codigo;
                existente.Descripcion = productoEF.Descripcion;
                existente.Inhabilitado = productoEF.Inhabilitado;
                
                _context.Entry(existente).State = EntityState.Modified;
            }
        }

        public new void Delete(ProductoDM entidad)
        {
            var productoEF = _context.Producto.Find(entidad.ID);
            if (productoEF != null)
            {
                // Eliminación lógica: solo marcamos como inhabilitado
                productoEF.Inhabilitado = true;
                _context.Entry(productoEF).State = EntityState.Modified;
            }
        }

        // ==================== MAPEOS ENTRE ENTIDADES ====================

        /// <summary>
        /// Convierte de entidad EF (Producto) a entidad de dominio (ProductoDM)
        /// </summary>
        private ProductoDM MapearADominio(Producto productoEF)
        {
            return new ProductoDM
            {
                ID = productoEF.ID,
                Codigo = productoEF.Codigo,
                Descripcion = productoEF.Descripcion,
                Inhabilitado = productoEF.Inhabilitado,
                FechaAlta = productoEF.FechaAlta,
                UsuarioAlta = productoEF.UsuarioAlta,
                PorcentajeIVA = productoEF.PorcentajeIVA
            };
        }

        /// <summary>
        /// Convierte de entidad de dominio (ProductoDM) a entidad EF (Producto)
        /// </summary>
        private Producto MapearAEntityFramework(ProductoDM dominio)
        {
            return new Producto
            {
                ID = dominio.ID,
                Codigo = dominio.Codigo,
                Descripcion = dominio.Descripcion,
                Inhabilitado = dominio.Inhabilitado,
                FechaAlta = dominio.FechaAlta,
                UsuarioAlta = dominio.UsuarioAlta,
                PorcentajeIVA = dominio.PorcentajeIVA
            };
        }
    }
}
