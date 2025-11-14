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
    /// Repositorio de acceso a datos para Cliente
    /// Implementa las operaciones de persistencia específicas de Cliente
    /// </summary>
    public class ClienteRepository : Repository<ClienteDM>, IClienteRepository
    {
        public ClienteRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public ClienteDM GetByCodigo(string codigoCliente)
        {
            // Buscar en la tabla EF Cliente
            var clienteEF = _context.Cliente
                .FirstOrDefault(c => c.CodigoCliente == codigoCliente);

            if (clienteEF == null)
                return null;

            // Convertir de entidad EF a entidad de dominio
            return MapearADominio(clienteEF);
        }

        public ClienteDM GetByDocumento(string numeroDocumento)
        {
            // Buscar en la tabla EF Cliente por CUIT (que almacena el número de documento)
            var clienteEF = _context.Cliente
                .FirstOrDefault(c => c.CUIT == numeroDocumento);

            if (clienteEF == null)
                return null;

            return MapearADominio(clienteEF);
        }

        public IEnumerable<ClienteDM> GetActivos()
        {
            var clientesEF = _context.Cliente
                .Where(c => c.Activo)
                .ToList();

            return clientesEF.Select(MapearADominio);
        }

        public bool ExisteCodigo(string codigoCliente, Guid? excluyendoId = null)
        {
            if (excluyendoId.HasValue)
            {
                return _context.Cliente
                    .Any(c => c.CodigoCliente == codigoCliente && c.ID != excluyendoId.Value);
            }

            return _context.Cliente
                .Any(c => c.CodigoCliente == codigoCliente);
        }

        public bool ExisteDocumento(string numeroDocumento, Guid? excluyendoId = null)
        {
            if (excluyendoId.HasValue)
            {
                return _context.Cliente
                    .Any(c => c.CUIT == numeroDocumento && c.ID != excluyendoId.Value);
            }

            return _context.Cliente
                .Any(c => c.CUIT == numeroDocumento);
        }

        public new ClienteDM GetById(Guid id)
        {
            var clienteEF = _context.Cliente.Find(id);
            if (clienteEF == null)
                return null;

            return MapearADominio(clienteEF);
        }

        // Sobrescribir métodos base para usar el mapeo personalizado
        public new IEnumerable<ClienteDM> GetAll()
        {
            var clientesEF = _context.Cliente.ToList();
            return clientesEF.Select(MapearADominio);
        }

        public new void Add(ClienteDM entidad)
        {
            var clienteEF = MapearAEntityFramework(entidad);
            _context.Cliente.Add(clienteEF);
        }

        public new void Update(ClienteDM entidad)
        {
            var clienteEF = MapearAEntityFramework(entidad);
            
            var existente = _context.Cliente.Find(entidad.Id);
            if (existente != null)
            {
                // Actualizar propiedades
                _context.Entry(existente).CurrentValues.SetValues(clienteEF);
            }
        }

        public new void Delete(ClienteDM entidad)
        {
            var clienteEF = _context.Cliente.Find(entidad.Id);
            if (clienteEF != null)
            {
                // Eliminación lógica: solo marcamos como inactivo
                clienteEF.Activo = false;
                clienteEF.FechaModificacion = DateTime.Now;
                _context.Entry(clienteEF).State = EntityState.Modified;
            }
        }

        // ==================== MAPEOS ENTRE ENTIDADES ====================

        /// <summary>
        /// Convierte de entidad EF (Cliente) a entidad de dominio (ClienteDM)
        /// </summary>
        private ClienteDM MapearADominio(Cliente clienteEF)
        {
            // El tipo de documento ahora está en un campo separado
            string tipoDocumento = clienteEF.TipoDocumento ?? "CUIT";
            string numeroDocumento = clienteEF.CUIT ?? string.Empty;

            // IdVendedor ahora es Guid? (FK)
            Guid? idVendedor = clienteEF.IdVendedor;

            // IdProvincia ahora es Guid? (FK)
            Guid? idProvincia = clienteEF.IdProvincia;

            // Usar los campos de condición de pago
            string condicionPago = clienteEF.CondicionPago ?? "01";
            string tipoIva = clienteEF.TipoIva ?? "RESPONSABLE INSCRIPTO";
            
            // AlicuotaArba
            decimal alicuotaArba = clienteEF.AlicuotaArba;

            // Usar los campos de contacto reales
            string email = clienteEF.Email;
            string telefono = clienteEF.Telefono;
            string direccion = clienteEF.DireccionLegal;
            string localidad = clienteEF.Localidad;

            // Usar los campos de auditoría reales
            bool activo = clienteEF.Activo;
            DateTime fechaAlta = clienteEF.FechaAlta != default(DateTime) ? clienteEF.FechaAlta : DateTime.Now;
            DateTime? fechaModificacion = clienteEF.FechaModificacion;

            // Usar el constructor de carga desde BD
            return new ClienteDM(
                clienteEF.ID,
                clienteEF.CodigoCliente,
                clienteEF.RazonSocial,
                tipoDocumento,
                numeroDocumento,
                idVendedor,
                tipoIva,
                condicionPago,
                activo,
                fechaAlta,
                fechaModificacion,
                alicuotaArba, // NUEVO
                idProvincia,
                email,
                telefono,
                direccion,
                localidad
            );
        }

        /// <summary>
        /// Convierte de entidad de dominio (ClienteDM) a entidad EF (Cliente)
        /// </summary>
        private Cliente MapearAEntityFramework(ClienteDM dominio)
        {
            return new Cliente
            {
                ID = dominio.Id,
                CodigoCliente = dominio.CodigoCliente,
                RazonSocial = dominio.RazonSocial,
                TipoDocumento = dominio.TipoDocumento,
                CUIT = dominio.NumeroDocumento,
                IdVendedor = dominio.IdVendedor,
                IdProvincia = dominio.IdProvincia,
                TipoIva = dominio.TipoIva,
                CondicionPago = dominio.CondicionPago,
                AlicuotaArba = dominio.AlicuotaArba, // NUEVO
                Email = dominio.Email,
                Telefono = dominio.Telefono,
                DireccionLegal = dominio.Direccion,
                DireccionComercial = dominio.Direccion,
                Localidad = dominio.Localidad,
                Activo = dominio.Activo,
                FechaAlta = dominio.FechaAlta,
                FechaModificacion = dominio.FechaModificacion
            };
        }
    }
}
