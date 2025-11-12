using DAL.Implementation.EntityFramework;
using DAL.Infrastructure;
using DomainModel.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Implementation.Repository
{
    /// <summary>
    /// Repositorio para gestión de listas de precios en Entity Framework
    /// Implementa operaciones CRUD sobre las tablas ListaPrecio y ListaPrecio_Detalle
    /// </summary>
    public class ListaPrecioRepository : Repository<ListaPrecio>, IListaPrecioRepository
    {
        private new readonly SistemaPresupuestario _context;

        public ListaPrecioRepository(SistemaPresupuestario context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una lista de precios por ID incluyendo sus detalles y productos
        /// </summary>
        public async Task<object> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ListaPrecio
                .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
                .FirstOrDefaultAsync(lp => lp.ID == id);
        }

        /// <summary>
        /// Obtiene una lista de precios por código
        /// </summary>
        public async Task<object> GetByCodigoAsync(string codigo)
        {
            return await _context.ListaPrecio
                .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
                .FirstOrDefaultAsync(lp => lp.Codigo == codigo);
        }

        /// <summary>
        /// Obtiene todas las listas de precios con sus detalles
        /// </summary>
        public async Task<IEnumerable<object>> GetAllWithDetailsAsync()
        {
            var listas = await _context.ListaPrecio
                .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
                .ToListAsync();
            return listas.Cast<object>();
        }

        /// <summary>
        /// Obtiene solo las listas de precios activas
        /// </summary>
        public async Task<IEnumerable<object>> GetActivasAsync()
        {
            var listas = await _context.ListaPrecio
                .Where(lp => lp.Activo)
                .ToListAsync();
            return listas.Cast<object>();
        }

        /// <summary>
        /// Verifica si existe un código (para validación de duplicados)
        /// </summary>
        public async Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null)
        {
            var query = _context.ListaPrecio.Where(lp => lp.Codigo == codigo);

            if (excludeId.HasValue)
            {
                query = query.Where(lp => lp.ID != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Obtiene el precio de un producto en una lista específica
        /// </summary>
        public async Task<decimal?> ObtenerPrecioProductoAsync(Guid idListaPrecio, Guid idProducto)
        {
            var detalle = await _context.ListaPrecio_Detalle
                .FirstOrDefaultAsync(d => d.IdListaPrecio == idListaPrecio && d.IdProducto == idProducto);

            return detalle?.Precio;
        }

        // Métodos síncronos para compatibilidad
        public object GetByIdWithDetails(Guid id)
        {
            return _context.ListaPrecio
                .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
                .FirstOrDefault(lp => lp.ID == id);
        }

        public IEnumerable<object> GetAllWithDetails()
        {
            return _context.ListaPrecio
                .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
                .ToList()
                .Cast<object>();
        }

        public IEnumerable<object> GetActivas()
        {
            return _context.ListaPrecio
                .Where(lp => lp.Activo)
                .ToList()
                .Cast<object>();
        }

        // Implementación explícita de IRepository<object>
        void IRepository<object>.Add(object entity)
        {
            if (entity is ListaPrecio listaPrecio)
            {
                base.Add(listaPrecio);
            }
            else
            {
                throw new ArgumentException("La entidad debe ser del tipo ListaPrecio");
            }
        }

        void IRepository<object>.Update(object entity)
        {
            if (entity is ListaPrecio listaPrecio)
            {
                base.Update(listaPrecio);
            }
            else
            {
                throw new ArgumentException("La entidad debe ser del tipo ListaPrecio");
            }
        }

        void IRepository<object>.Delete(object entity)
        {
            if (entity is ListaPrecio listaPrecio)
            {
                base.Delete(listaPrecio);
            }
            else
            {
                throw new ArgumentException("La entidad debe ser del tipo ListaPrecio");
            }
        }

        async Task<IEnumerable<object>> IRepository<object>.GetAllAsync()
        {
            var listas = await base.GetAllAsync();
            return listas.Cast<object>();
        }

        async Task<object> IRepository<object>.GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        async Task<object> IRepository<object>.GetByIdAsync(Guid id)
        {
            return await base.GetByIdAsync(id);
        }
    }
}
