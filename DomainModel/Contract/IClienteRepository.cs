using DomainModel.Domain;
using System;
using System.Collections.Generic;

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
        ClienteDM GetByCodigo(string codigoCliente);

        /// <summary>
        /// Busca un cliente por su número de documento
        /// </summary>
        ClienteDM GetByDocumento(string numeroDocumento);

        /// <summary>
        /// Obtiene todos los clientes activos
        /// </summary>
        IEnumerable<ClienteDM> GetActivos();

        /// <summary>
        /// Verifica si existe un cliente con el código especificado
        /// </summary>
        bool ExisteCodigo(string codigoCliente, Guid? excluyendoId = null);

        /// <summary>
        /// Verifica si existe un cliente con el número de documento especificado
        /// </summary>
        bool ExisteDocumento(string numeroDocumento, Guid? excluyendoId = null);
    }
}
