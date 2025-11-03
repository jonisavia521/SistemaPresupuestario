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
    /// Repositorio de acceso a datos para Cliente
    /// Implementa las operaciones de persistencia específicas de Cliente
    /// </summary>
    public class ClienteRepository : Repository<ClienteDM>, IClienteRepository
    {
        public ClienteRepository(SistemaPresupuestario context) : base(context)
        {
        }

        public async Task<ClienteDM> GetByCodigoAsync(string codigoCliente)
        {
            // Buscar en la tabla EF Cliente
            var clienteEF = await _context.Cliente
                .FirstOrDefaultAsync(c => c.CodigoCliente == codigoCliente);

            if (clienteEF == null)
                return null;

            // Convertir de entidad EF a entidad de dominio
            return MapearADominio(clienteEF);
        }

        public async Task<ClienteDM> GetByDocumentoAsync(string numeroDocumento)
        {
            // Buscar en la tabla EF Cliente por CUIT (que almacena el número de documento)
            var clienteEF = await _context.Cliente
                .FirstOrDefaultAsync(c => c.CUIT == numeroDocumento);

            if (clienteEF == null)
                return null;

            return MapearADominio(clienteEF);
        }

        public async Task<IEnumerable<ClienteDM>> GetActivosAsync()
        {
            var clientesEF = await _context.Cliente
                .Where(c => c.Activo)
                .ToListAsync();

            return clientesEF.Select(MapearADominio);
        }

        public async Task<bool> ExisteCodigoAsync(string codigoCliente, Guid? excluyendoId = null)
        {
            if (excluyendoId.HasValue)
            {
                return await _context.Cliente
                    .AnyAsync(c => c.CodigoCliente == codigoCliente && c.ID != excluyendoId.Value);
            }

            return await _context.Cliente
                .AnyAsync(c => c.CodigoCliente == codigoCliente);
        }

        public async Task<bool> ExisteDocumentoAsync(string numeroDocumento, Guid? excluyendoId = null)
        {
            if (excluyendoId.HasValue)
            {
                return await _context.Cliente
                    .AnyAsync(c => c.CUIT == numeroDocumento && c.ID != excluyendoId.Value);
            }

            return await _context.Cliente
                .AnyAsync(c => c.CUIT == numeroDocumento);
        }

        public ClienteDM GetById(Guid id)
        {
            var clienteEF = _context.Cliente.Find(id);
            if (clienteEF == null)
                return null;

            return MapearADominio(clienteEF);
        }

        // Sobrescribir métodos base para usar el mapeo personalizado
        public new async Task<IEnumerable<ClienteDM>> GetAllAsync()
        {
            var clientesEF = await _context.Cliente.ToListAsync();
            return clientesEF.Select(MapearADominio);
        }

        public new async Task<ClienteDM> GetByIdAsync(Guid id)
        {
            var clienteEF = await _context.Cliente.FindAsync(id);
            if (clienteEF == null)
                return null;

            return MapearADominio(clienteEF);
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

            // Usar los campos de condición de pago
            string condicionPago = clienteEF.CondicionPago ?? "01";
            string tipoIva = clienteEF.TipoIva ?? "RESPONSABLE INSCRIPTO";

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
                idVendedor, // MODIFICADO
                tipoIva,
                condicionPago,
                activo,
                fechaAlta,
                fechaModificacion,
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
                IdVendedor = dominio.IdVendedor, // MODIFICADO
                TipoIva = dominio.TipoIva,
                CondicionPago = dominio.CondicionPago,
                Email = dominio.Email,
                Telefono = dominio.Telefono,
                DireccionLegal = dominio.Direccion,
                DireccionComercial = dominio.Direccion,
                Localidad = dominio.Localidad,
                Activo = dominio.Activo,
                FechaAlta = dominio.FechaAlta,
                FechaModificacion = dominio.FechaModificacion,
                IdProvincia = null
            };
        }
    }
}
