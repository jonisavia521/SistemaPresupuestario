using DAL.Implementation.EntityFramework;
using DAL.Infrastructure;
using DomainModel.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
        public object GetByIdWithDetails(Guid id)
        {
            return _context.ListaPrecio
                .Include(lp => lp.ListaPrecio_Detalle)
                .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
                .FirstOrDefault(lp => lp.ID == id);
        }

        /// <summary>
        /// Obtiene una lista de precios por código
        /// </summary>
        public object GetByCodigo(string codigo)
        {
            return _context.ListaPrecio
                .Include(lp => lp.ListaPrecio_Detalle)
                .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
                .FirstOrDefault(lp => lp.Codigo == codigo);
        }

        /// <summary>
        /// Obtiene todas las listas de precios con sus detalles
        /// </summary>
        public IEnumerable<object> GetAllWithDetails()
        {
            return _context.ListaPrecio
                .Include(lp => lp.ListaPrecio_Detalle)
                .Include(lp => lp.ListaPrecio_Detalle.Select(d => d.Producto))
                .ToList()
                .Cast<object>();
        }

        /// <summary>
        /// Obtiene solo las listas de precios activas
        /// </summary>
        public IEnumerable<object> GetActivas()
        {
            return _context.ListaPrecio
                .Where(lp => lp.Activo)
                .ToList()
                .Cast<object>();
        }

        /// <summary>
        /// Verifica si existe un código (para validación de duplicados)
        /// </summary>
        public bool ExisteCodigo(string codigo, Guid? excludeId = null)
        {
            var query = _context.ListaPrecio.Where(lp => lp.Codigo == codigo);

            if (excludeId.HasValue)
            {
                query = query.Where(lp => lp.ID != excludeId.Value);
            }

            return query.Any();
        }

        /// <summary>
        /// Obtiene el precio de un producto en una lista específica
        /// </summary>
        public decimal? ObtenerPrecioProducto(Guid idListaPrecio, Guid idProducto)
        {
            var detalle = _context.ListaPrecio_Detalle
                .FirstOrDefault(d => d.IdListaPrecio == idListaPrecio && d.IdProducto == idProducto);

            return detalle?.Precio;
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
                // 1. Obtener IDs de detalles existentes sin tracking
                var idsDetallesExistentes = _context.ListaPrecio_Detalle
                    .Where(d => d.IdListaPrecio == listaPrecio.ID)
                    .Select(d => d.ID)
                    .ToList(); // Materializar la consulta ANTES de eliminar

                // 2. Eliminar detalles existentes por IDs
                foreach (var id in idsDetallesExistentes)
                {
                    var detalleAEliminar = _context.ListaPrecio_Detalle.Find(id);
                    if (detalleAEliminar != null)
                    {
                        _context.ListaPrecio_Detalle.Remove(detalleAEliminar);
                    }
                }

                // 3. Buscar la entidad principal
                var existing = _context.ListaPrecio.Find(listaPrecio.ID);
                if (existing == null)
                {
                    throw new InvalidOperationException($"No se encontró la lista de precios con ID {listaPrecio.ID}");
                }

                // 4. Actualizar las propiedades escalares
                existing.Codigo = listaPrecio.Codigo;
                existing.Nombre = listaPrecio.Nombre;
                existing.Activo = listaPrecio.Activo;
                existing.IncluyeIva = listaPrecio.IncluyeIva;
                existing.FechaModificacion = DateTime.Now;

                // 5. Agregar todos los detalles nuevos con IDs frescos
                foreach (var detalle in listaPrecio.ListaPrecio_Detalle.ToList())
                {
                    var nuevoDetalle = new ListaPrecio_Detalle
                    {
                        ID = Guid.NewGuid(), // ? Generar un nuevo ID para evitar conflictos
                        IdListaPrecio = listaPrecio.ID,
                        IdProducto = detalle.IdProducto,
                        Precio = detalle.Precio
                    };
                    _context.ListaPrecio_Detalle.Add(nuevoDetalle);
                }

                // 6. Marcar la entidad principal como modificada
                _context.Entry(existing).State = EntityState.Modified;
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

        IEnumerable<object> IRepository<object>.GetAll()
        {
            var listas = base.GetAll();
            return listas.Cast<object>();
        }

        object IRepository<object>.GetById(int id)
        {
            return base.GetById(id);
        }

        object IRepository<object>.GetById(Guid id)
        {
            return base.GetById(id);
        }
    }
}
