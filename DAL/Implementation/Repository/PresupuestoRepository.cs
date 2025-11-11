using DAL.Implementation.EntityFramework;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Implementation.Repository
{
    public class PresupuestoRepository : IPresupuestoRepository
    {
        private readonly SistemaPresupuestario _context;

        public PresupuestoRepository(SistemaPresupuestario context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Implementación de IRepository<PresupuestoDM>
        public async Task<IEnumerable<PresupuestoDM>> GetAllAsync()
        {
            var presupuestos = await _context.Presupuesto.ToListAsync();
            return presupuestos.Select(p => MapToDomain(p, false));
        }

        public async Task<PresupuestoDM> GetByIdAsync(Guid id)
        {
            var presupuestoEF = await _context.Presupuesto.FindAsync(id);
            return presupuestoEF != null ? MapToDomain(presupuestoEF, false) : null;
        }

        public async Task<PresupuestoDM> GetByIdAsync(int id)
        {
            // No aplicable para presupuestos (usan Guid), lanzar excepción o devolver null
            throw new NotSupportedException("Presupuesto usa Guid como identificador, no int");
        }

        public void Add(PresupuestoDM entity)
        {
            // Mapear de Domain Model a Entity Framework
            var presupuestoEF = MapToEntity(entity);
            _context.Presupuesto.Add(presupuestoEF);
        }

        public void Delete(PresupuestoDM entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Delete(entity.Id);
        }

        public void Delete(Guid id)
        {
            var presupuesto = _context.Presupuesto.Find(id);
            if (presupuesto != null)
            {
                // Eliminar detalles primero
                var detalles = _context.Presupuesto_Detalle.Where(d => d.IdPresupuesto == id).ToList();
                _context.Presupuesto_Detalle.RemoveRange(detalles);
                
                _context.Presupuesto.Remove(presupuesto);
            }
        }

        public PresupuestoDM GetById(Guid id)
        {
            var presupuestoEF = _context.Presupuesto.Find(id);
            return presupuestoEF != null ? MapToDomain(presupuestoEF, false) : null;
        }

        public PresupuestoDM GetByIdWithDetails(Guid id)
        {
            var presupuestoEF = _context.Presupuesto
                .Include(p => p.Presupuesto_Detalle)
                .FirstOrDefault(p => p.ID == id);

            return presupuestoEF != null ? MapToDomain(presupuestoEF, true) : null;
        }

        public IEnumerable<PresupuestoDM> GetAll()
        {
            return _context.Presupuesto
                .ToList()
                .Select(p => MapToDomain(p, false));
        }

        public IEnumerable<PresupuestoDM> GetAllWithDetails()
        {
            return _context.Presupuesto
                .Include(p => p.Presupuesto_Detalle)
                .ToList()
                .Select(p => MapToDomain(p, true));
        }

        public IEnumerable<PresupuestoDM> GetByCliente(Guid idCliente)
        {
            return _context.Presupuesto
                .Where(p => p.IdCliente == idCliente)
                .ToList()
                .Select(p => MapToDomain(p, false));
        }

        public IEnumerable<PresupuestoDM> GetByVendedor(Guid idVendedor)
        {
            return _context.Presupuesto
                .Where(p => p.IdVendedor == idVendedor)
                .ToList()
                .Select(p => MapToDomain(p, false));
        }

        public IEnumerable<PresupuestoDM> GetByEstado(int estado)
        {
            return _context.Presupuesto
                .Where(p => p.Estado == estado)
                .ToList()
                .Select(p => MapToDomain(p, false));
        }

        public void Update(PresupuestoDM entity)
        {
            var presupuestoEF = _context.Presupuesto.Find(entity.Id);
            if (presupuestoEF == null)
                throw new InvalidOperationException("Presupuesto no encontrado");

            // Actualizar campos del presupuesto
            presupuestoEF.Numero = entity.Numero;
            presupuestoEF.IdCliente = entity.IdCliente;
            presupuestoEF.FechaEmision = entity.FechaEmision;
            presupuestoEF.Estado = entity.Estado;
            presupuestoEF.FechaVencimiento = entity.FechaVencimiento;
            presupuestoEF.IdVendedor = entity.IdVendedor;
            presupuestoEF.IdPresupuestoPadre = entity.IdPresupuestoPadre;

            // Actualizar detalles
            // Eliminar detalles existentes
            var detallesExistentes = _context.Presupuesto_Detalle.Where(d => d.IdPresupuesto == entity.Id).ToList();
            _context.Presupuesto_Detalle.RemoveRange(detallesExistentes);

            // Agregar nuevos detalles
            foreach (var detalleDM in entity.Detalles)
            {
                var detalleEF = new Presupuesto_Detalle
                {
                    ID = detalleDM.Id,
                    Numero = entity.Numero,
                    IdPresupuesto = entity.Id,
                    IdProducto = detalleDM.IdProducto,
                    Cantidad = detalleDM.Cantidad,
                    Precio = detalleDM.Precio,
                    Descuento = detalleDM.Descuento,
                    Renglon = detalleDM.Renglon
                };

                _context.Presupuesto_Detalle.Add(detalleEF);
            }

            _context.Entry(presupuestoEF).State = EntityState.Modified;
        }

        public string GetNextNumero()
        {
            // Obtener el último presupuesto ordenado por número
            var ultimoNumero = _context.Presupuesto
                .OrderByDescending(p => p.Numero)
                .Select(p => p.Numero)
                .FirstOrDefault();

            int siguienteNumero = 1;

            if (!string.IsNullOrEmpty(ultimoNumero))
            {
                // Intentar extraer el número del formato actual (puede ser cualquier formato)
                // Buscar dígitos al final del string
                string digitosExtraidos = new string(ultimoNumero.Where(char.IsDigit).ToArray());
                
                if (!string.IsNullOrEmpty(digitosExtraidos) && int.TryParse(digitosExtraidos, out int numeroActual))
                {
                    siguienteNumero = numeroActual + 1;
                }
            }

            // Formato: 8 dígitos con ceros a la izquierda (ej: 00000001, 00000055)
            return siguienteNumero.ToString("D8");
        }

        public bool ExisteNumero(string numero, Guid? idExcluir = null)
        {
            if (idExcluir.HasValue)
            {
                return _context.Presupuesto.Any(p => p.Numero == numero && p.ID != idExcluir.Value);
            }
            
            return _context.Presupuesto.Any(p => p.Numero == numero);
        }

        // Métodos de mapeo

        private PresupuestoDM MapToDomain(Presupuesto presupuestoEF, bool incluirDetalles)
        {
            List<PresupuestoDetalleDM> detalles = null;

            if (incluirDetalles && presupuestoEF.Presupuesto_Detalle != null)
            {
                detalles = presupuestoEF.Presupuesto_Detalle
                    .OrderBy(d => d.Renglon)
                    .Select(d => MapDetalleToDomain(d, presupuestoEF.ID))
                    .ToList();
            }

            return new PresupuestoDM(
                presupuestoEF.ID,
                presupuestoEF.Numero,
                presupuestoEF.IdCliente,
                presupuestoEF.FechaEmision,
                presupuestoEF.Estado,
                presupuestoEF.FechaVencimiento,
                presupuestoEF.IdPresupuestoPadre,
                presupuestoEF.IdVendedor,
                detalles
            );
        }

        private PresupuestoDetalleDM MapDetalleToDomain(Presupuesto_Detalle detalleEF, Guid idPresupuesto)
        {
            // Obtener el porcentaje de IVA del producto
            decimal porcentajeIVA = 0;
            if (detalleEF.IdProducto.HasValue)
            {
                var producto = _context.Producto.Find(detalleEF.IdProducto.Value);
                if (producto != null)
                {
                    porcentajeIVA = producto.PorcentajeIVA;
                }
            }

            return new PresupuestoDetalleDM(
                detalleEF.ID,
                detalleEF.Numero,
                idPresupuesto,
                detalleEF.IdProducto,
                detalleEF.Cantidad ?? 0,
                detalleEF.Precio ?? 0,
                detalleEF.Descuento ?? 0,
                detalleEF.Renglon ?? 0,
                porcentajeIVA
            );
        }

        private Presupuesto MapToEntity(PresupuestoDM presupuestoDM)
        {
            var presupuestoEF = new Presupuesto
            {
                ID = presupuestoDM.Id,
                Numero = presupuestoDM.Numero,
                IdCliente = presupuestoDM.IdCliente,
                FechaEmision = presupuestoDM.FechaEmision,
                Estado = presupuestoDM.Estado,
                FechaVencimiento = presupuestoDM.FechaVencimiento,
                IdPresupuestoPadre = presupuestoDM.IdPresupuestoPadre,
                IdVendedor = presupuestoDM.IdVendedor
            };

            // Mapear detalles
            foreach (var detalleDM in presupuestoDM.Detalles)
            {
                var detalleEF = new Presupuesto_Detalle
                {
                    ID = detalleDM.Id,
                    Numero = presupuestoDM.Numero,
                    IdPresupuesto = presupuestoDM.Id,
                    IdProducto = detalleDM.IdProducto,
                    Cantidad = detalleDM.Cantidad,
                    Precio = detalleDM.Precio,
                    Descuento = detalleDM.Descuento,
                    Renglon = detalleDM.Renglon
                };

                presupuestoEF.Presupuesto_Detalle.Add(detalleEF);
            }

            return presupuestoEF;
        }
    }
}
