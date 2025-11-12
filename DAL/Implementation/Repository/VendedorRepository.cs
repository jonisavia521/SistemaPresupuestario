using DAL.Implementation.EntityFramework;
using DAL.Infrastructure;
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
    /// Repositorio de acceso a datos para Vendedor
    /// Implementa las operaciones de persistencia específicas de Vendedor
    /// </summary>
    public class VendedorRepository : Repository<VendedorDM>, IVendedorRepository
    {
        public VendedorRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public async Task<VendedorDM> GetByCodigoAsync(string codigoVendedor)
        {
            var vendedorEF = await _context.Vendedor
                .FirstOrDefaultAsync(v => v.CodigoVendedor == codigoVendedor);

            if (vendedorEF == null)
                return null;

            return MapearADominio(vendedorEF);
        }

        public async Task<VendedorDM> GetByCUITAsync(string cuit)
        {
            var vendedorEF = await _context.Vendedor
                .FirstOrDefaultAsync(v => v.CUIT == cuit);

            if (vendedorEF == null)
                return null;

            return MapearADominio(vendedorEF);
        }

        public async Task<IEnumerable<VendedorDM>> GetActivosAsync()
        {
            var vendedoresEF = await _context.Vendedor
                .Where(v => v.Activo)
                .ToListAsync();

            return vendedoresEF.Select(MapearADominio);
        }

        public async Task<bool> ExisteCodigoAsync(string codigoVendedor, Guid? excluyendoId = null)
        {
            if (excluyendoId.HasValue)
            {
                return await _context.Vendedor
                    .AnyAsync(v => v.CodigoVendedor == codigoVendedor && v.ID != excluyendoId.Value);
            }

            return await _context.Vendedor
                .AnyAsync(v => v.CodigoVendedor == codigoVendedor);
        }

        public async Task<bool> ExisteCUITAsync(string cuit, Guid? excluyendoId = null)
        {
            if (excluyendoId.HasValue)
            {
                return await _context.Vendedor
                    .AnyAsync(v => v.CUIT == cuit && v.ID != excluyendoId.Value);
            }

            return await _context.Vendedor
                .AnyAsync(v => v.CUIT == cuit);
        }

        public VendedorDM GetById(Guid id)
        {
            var vendedorEF = _context.Vendedor.Find(id);
            if (vendedorEF == null)
                return null;

            return MapearADominio(vendedorEF);
        }

        // Sobrescribir métodos base para usar el mapeo personalizado
        public new async Task<IEnumerable<VendedorDM>> GetAllAsync()
        {
            var vendedoresEF = await _context.Vendedor.ToListAsync();
            return vendedoresEF.Select(MapearADominio);
        }

        public new async Task<VendedorDM> GetByIdAsync(Guid id)
        {
            var vendedorEF = await _context.Vendedor.FindAsync(id);
            if (vendedorEF == null)
                return null;

            return MapearADominio(vendedorEF);
        }

        public new void Add(VendedorDM entidad)
        {
            var vendedorEF = MapearAEntityFramework(entidad);
            _context.Vendedor.Add(vendedorEF);
        }

        public new void Update(VendedorDM entidad)
        {
            var vendedorEF = MapearAEntityFramework(entidad);
            
            var existente = _context.Vendedor.Find(entidad.Id);
            if (existente != null)
            {
                // Actualizar propiedades
                _context.Entry(existente).CurrentValues.SetValues(vendedorEF);
            }
        }

        public new void Delete(VendedorDM entidad)
        {
            var vendedorEF = _context.Vendedor.Find(entidad.Id);
            if (vendedorEF != null)
            {
                // Eliminación lógica
                vendedorEF.Activo = false;
                vendedorEF.FechaModificacion = DateTime.Now;
                _context.Entry(vendedorEF).State = EntityState.Modified;
            }
        }

        // ==================== MAPEOS ENTRE ENTIDADES ====================

        /// <summary>
        /// Convierte de entidad EF (Vendedor) a entidad de dominio (VendedorDM)
        /// </summary>
        private VendedorDM MapearADominio(Vendedor vendedorEF)
        {
            // Ahora todos los campos son NOT NULL según el script SQL
            string codigoVendedor = vendedorEF.CodigoVendedor;
            string cuit = vendedorEF.CUIT;
            decimal porcentajeComision = vendedorEF.PorcentajeComision;
            
            bool activo = vendedorEF.Activo;
            DateTime fechaAlta = vendedorEF.FechaAlta;
            DateTime? fechaModificacion = vendedorEF.FechaModificacion;

            return new VendedorDM(
                vendedorEF.ID,
                codigoVendedor,
                vendedorEF.Nombre,
                cuit,
                porcentajeComision,
                activo,
                fechaAlta,
                fechaModificacion,
                vendedorEF.Mail,
                vendedorEF.Telefono,
                vendedorEF.Direccion
            );
        }

        /// <summary>
        /// Convierte de entidad de dominio (VendedorDM) a entidad EF (Vendedor)
        /// </summary>
        private Vendedor MapearAEntityFramework(VendedorDM dominio)
        {
            return new Vendedor
            {
                ID = dominio.Id,
                CodigoVendedor = dominio.CodigoVendedor,
                Nombre = dominio.Nombre,
                CUIT = dominio.CUIT,
                PorcentajeComision = dominio.PorcentajeComision,
                Mail = dominio.Email,
                Telefono = dominio.Telefono,
                Direccion = dominio.Direccion,
                Activo = dominio.Activo,
                FechaAlta = dominio.FechaAlta,
                FechaModificacion = dominio.FechaModificacion
            };
        }
    }
}
