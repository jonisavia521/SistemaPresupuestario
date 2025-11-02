using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainModel.Contract
{
    /// <summary>
    /// Contrato para el repositorio de Productos.
    /// Define las operaciones de acceso a datos sin especificar la implementación.
    /// </summary>
    public interface IProductoRepository : IRepository<ProductoDM>
    {
        /// <summary>
        /// Busca un producto por su código único.
        /// </summary>
        Task<ProductoDM> GetByCodigoAsync(string codigo);

        /// <summary>
        /// Obtiene todos los productos activos (no inhabilitados).
        /// </summary>
        Task<IEnumerable<ProductoDM>> GetActivosAsync();

        /// <summary>
        /// Verifica si un código de producto ya existe en el sistema.
        /// </summary>
        Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null);
    }
}
