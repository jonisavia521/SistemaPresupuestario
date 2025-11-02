using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainModel.Contract
{
    /// <summary>
    /// Contrato para operaciones de acceso a datos de Vendedor
    /// Define la abstracción que la DAL debe implementar
    /// </summary>
    public interface IVendedorRepository : IRepository<VendedorDM>
    {
        /// <summary>
        /// Busca un vendedor por su código único
        /// </summary>
        Task<VendedorDM> GetByCodigoAsync(string codigoVendedor);

        /// <summary>
        /// Busca un vendedor por su CUIT
        /// </summary>
        Task<VendedorDM> GetByCUITAsync(string cuit);

        /// <summary>
        /// Obtiene todos los vendedores activos
        /// </summary>
        Task<IEnumerable<VendedorDM>> GetActivosAsync();

        /// <summary>
        /// Verifica si existe un vendedor con el código especificado
        /// </summary>
        Task<bool> ExisteCodigoAsync(string codigoVendedor, Guid? excluyendoId = null);

        /// <summary>
        /// Verifica si existe un vendedor con el CUIT especificado
        /// </summary>
        Task<bool> ExisteCUITAsync(string cuit, Guid? excluyendoId = null);
    }
}
