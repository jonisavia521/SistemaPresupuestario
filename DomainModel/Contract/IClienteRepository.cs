using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainModel.Contract
{
    /// <summary>
    /// Contrato para operaciones de acceso a datos de Cliente
    /// Define la abstracción que la DAL debe implementar
    /// </summary>
    public interface IClienteRepository : IRepository<ClienteDM>
    {
        /// <summary>
        /// Busca un cliente por su código único
        /// </summary>
        Task<ClienteDM> GetByCodigoAsync(string codigoCliente);

        /// <summary>
        /// Busca un cliente por su número de documento
        /// </summary>
        Task<ClienteDM> GetByDocumentoAsync(string numeroDocumento);

        /// <summary>
        /// Obtiene todos los clientes activos
        /// </summary>
        Task<IEnumerable<ClienteDM>> GetActivosAsync();

        /// <summary>
        /// Verifica si existe un cliente con el código especificado
        /// </summary>
        Task<bool> ExisteCodigoAsync(string codigoCliente, Guid? excluyendoId = null);

        /// <summary>
        /// Verifica si existe un cliente con el número de documento especificado
        /// </summary>
        Task<bool> ExisteDocumentoAsync(string numeroDocumento, Guid? excluyendoId = null);

        /// <summary>
        /// Obtiene un cliente por ID (versión síncrona)
        /// </summary>
        ClienteDM GetById(Guid id);
    }
}
