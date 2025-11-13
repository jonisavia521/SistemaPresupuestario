using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato del servicio de Productos
    /// Define las operaciones de negocio disponibles
    /// NOTA: Todos los métodos son síncronos para evitar deadlocks en Windows Forms
    /// </summary>
    public interface IProductoService
    {
        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        IEnumerable<ProductoDTO> GetAll();

        /// <summary>
        /// Obtiene todos los productos activos (no inhabilitados)
        /// </summary>
        IEnumerable<ProductoDTO> GetActivos();

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        ProductoDTO GetById(Guid id);

        /// <summary>
        /// Busca un producto por su código
        /// </summary>
        ProductoDTO GetByCodigo(string codigo);

        /// <summary>
        /// Agrega un nuevo producto
        /// </summary>
        bool Add(ProductoDTO productoDto);

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        bool Update(ProductoDTO productoDto);

        /// <summary>
        /// Elimina un producto (eliminación lógica)
        /// </summary>
        bool Delete(Guid id);

        /// <summary>
        /// Verifica si un código de producto ya existe
        /// </summary>
        bool ExisteCodigo(string codigo, Guid? excludeId = null);
    }
}
