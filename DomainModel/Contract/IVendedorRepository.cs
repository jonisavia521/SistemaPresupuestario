using DomainModel.Domain;
using System;
using System.Collections.Generic;

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
        VendedorDM GetByCodigo(string codigoVendedor);

        /// <summary>
        /// Busca un vendedor por su CUIT
        /// </summary>
        VendedorDM GetByCUIT(string cuit);

        /// <summary>
        /// Obtiene todos los vendedores activos
        /// </summary>
        IEnumerable<VendedorDM> GetActivos();

        /// <summary>
        /// Verifica si existe un vendedor con el código especificado
        /// </summary>
        bool ExisteCodigo(string codigoVendedor, Guid? excluyendoId = null);

        /// <summary>
        /// Verifica si existe un vendedor con el CUIT especificado
        /// </summary>
        bool ExisteCUIT(string cuit, Guid? excluyendoId = null);
    }
}
