using DomainModel.Domain;
using System;
using System.Collections.Generic;

namespace DomainModel.Contract
{
    /// <summary>
    /// Contrato para el repositorio de Productos.
    /// Define las operaciones de acceso a datos sin especificar la implementación.
    /// NOTA: Todos los métodos son síncronos para evitar deadlocks en Windows Forms.
    /// </summary>
    public interface IProductoRepository : IRepository<ProductoDM>
    {
        /// <summary>
        /// Busca un producto por su código único (versión síncrona)
        /// </summary>
        ProductoDM GetByCodigo(string codigo);

        /// <summary>
        /// Obtiene todos los productos activos (no inhabilitados)
        /// </summary>
        IEnumerable<ProductoDM> GetActivos();

        /// <summary>
        /// Verifica si un código de producto ya existe en el sistema
        /// </summary>
        bool ExisteCodigo(string codigo, Guid? excludeId = null);

        /// <summary>
        /// Obtiene un producto por ID (versión síncrona)
        /// </summary>
        ProductoDM GetById(Guid id);

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        IEnumerable<ProductoDM> GetAll();
    }
}
