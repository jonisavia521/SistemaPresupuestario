using DAL.Implementation.EntityFramework;
using DAL.Infrastructure;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Implementation.Repository
{
    /// <summary>
    /// Repositorio de acceso a datos para Producto
    /// Implementa las operaciones de persistencia específicas de Producto
    /// TODOS LOS MÉTODOS SON SÍNCRONOS para evitar deadlocks en Windows Forms
    /// </summary>
    public class ProductoRepository : Repository<ProductoDM>, IProductoRepository
    {
        public ProductoRepository(SistemaPresupuestario context) : base(context)
        {
        }

        /// <summary>
        /// Busca un producto por su código único
        /// </summary>
        public ProductoDM GetByCodigo(string codigo)
        {
            // Buscar con AsNoTracking para evitar lazy loading y mejorar rendimiento
            var productoEF = _context.Producto
                .AsNoTracking()
                .FirstOrDefault(p => p.Codigo == codigo);

            if (productoEF == null)
                return null;

            return MapearADominio(productoEF);
        }

        /// <summary>
        /// Obtiene todos los productos activos (no inhabilitados)
        /// </summary>
        public IEnumerable<ProductoDM> GetActivos()
        {
            var productosEF = _context.Producto
                .AsNoTracking()
                .Where(p => !p.Inhabilitado)
                .ToList();

            return productosEF.Select(MapearADominio);
        }

        /// <summary>
        /// Verifica si un código de producto ya existe
        /// </summary>
        public bool ExisteCodigo(string codigo, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return _context.Producto
                    .AsNoTracking()
                    .Any(p => p.Codigo == codigo && p.ID != excludeId.Value);
            }

            return _context.Producto
                .AsNoTracking()
                .Any(p => p.Codigo == codigo);
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        public ProductoDM GetById(Guid id)
        {
            var productoEF = _context.Producto
                .AsNoTracking()
                .FirstOrDefault(p => p.ID == id);
            
            if (productoEF == null)
                return null;

            return MapearADominio(productoEF);
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        public IEnumerable<ProductoDM> GetAll()
        {
            var productosEF = _context.Producto
                .AsNoTracking()
                .ToList();
            
            return productosEF.Select(MapearADominio);
        }

        // Sobrescribir métodos del base para que no se usen
        [Obsolete("Use GetAll() instead", true)]
        public new System.Threading.Tasks.Task<IEnumerable<ProductoDM>> GetAllAsync()
        {
            throw new NotSupportedException("Use el método síncrono GetAll()");
        }

        [Obsolete("Use GetById() instead", true)]
        public new System.Threading.Tasks.Task<ProductoDM> GetByIdAsync(Guid id)
        {
            throw new NotSupportedException("Use el método síncrono GetById()");
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
                existente.PorcentajeIVA = productoEF.PorcentajeIVA;
                
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
