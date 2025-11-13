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
    /// Repositorio de acceso a datos para Vendedor
    /// Implementa las operaciones de persistencia específicas de Vendedor
    /// </summary>
    public class VendedorRepository : Repository<VendedorDM>, IVendedorRepository
    {
        public VendedorRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public VendedorDM GetByCodigo(string codigoVendedor)
        {
            var vendedorEF = _context.Vendedor
                .FirstOrDefault(v => v.CodigoVendedor == codigoVendedor);

            if (vendedorEF == null)
                return null;

            return MapearADominio(vendedorEF);
        }

        public VendedorDM GetByCUIT(string cuit)
        {
            var vendedorEF = _context.Vendedor
                .FirstOrDefault(v => v.CUIT == cuit);

            if (vendedorEF == null)
                return null;

            return MapearADominio(vendedorEF);
        }

        public IEnumerable<VendedorDM> GetActivos()
        {
            var vendedoresEF = _context.Vendedor
                .Where(v => v.Activo)
                .ToList();

            return vendedoresEF.Select(MapearADominio);
        }

        public bool ExisteCodigo(string codigoVendedor, Guid? excluyendoId = null)
        {
            if (excluyendoId.HasValue)
            {
                return _context.Vendedor
                    .Any(v => v.CodigoVendedor == codigoVendedor && v.ID != excluyendoId.Value);
            }

            return _context.Vendedor
                .Any(v => v.CodigoVendedor == codigoVendedor);
        }

        public bool ExisteCUIT(string cuit, Guid? excluyendoId = null)
        {
            if (excluyendoId.HasValue)
            {
                return _context.Vendedor
                    .Any(v => v.CUIT == cuit && v.ID != excluyendoId.Value);
            }

            return _context.Vendedor
                .Any(v => v.CUIT == cuit);
        }

        public new VendedorDM GetById(Guid id)
        {
            var vendedorEF = _context.Vendedor.Find(id);
            if (vendedorEF == null)
                return null;

            return MapearADominio(vendedorEF);
        }

        // Sobrescribir métodos base para usar el mapeo personalizado
        public new IEnumerable<VendedorDM> GetAll()
        {
            var vendedoresEF = _context.Vendedor.ToList();
            return vendedoresEF.Select(MapearADominio);
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
