using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Lista de Precios
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IListaPrecioService
    {
        /// <summary>
        /// Obtiene una lista de precios por su ID con sus detalles
        /// </summary>
        Task<ListaPrecioDTO> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene una lista de precios por su código
        /// </summary>
        Task<ListaPrecioDTO> GetByCodigoAsync(string codigo);

        /// <summary>
        /// Obtiene todas las listas de precios
        /// </summary>
        Task<IEnumerable<ListaPrecioDTO>> GetAllAsync();

        /// <summary>
        /// Obtiene todas las listas de precios activas
        /// </summary>
        Task<IEnumerable<ListaPrecioDTO>> GetActivasAsync();

        /// <summary>
        /// Crea una nueva lista de precios
        /// </summary>
        Task<bool> AddAsync(ListaPrecioDTO listaPrecioDTO);

        /// <summary>
        /// Actualiza una lista de precios existente
        /// </summary>
        Task<bool> UpdateAsync(ListaPrecioDTO listaPrecioDTO);

        /// <summary>
        /// Elimina una lista de precios (desactivación lógica)
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Reactiva una lista de precios desactivada
        /// </summary>
        Task<bool> ReactivarAsync(Guid id);

        /// <summary>
        /// Verifica si un código ya existe (para validación)
        /// </summary>
        Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null);

        /// <summary>
        /// Obtiene el precio de un producto en una lista específica
        /// </summary>
        Task<decimal?> ObtenerPrecioProductoAsync(Guid idListaPrecio, Guid idProducto);

        // Métodos síncronos para compatibilidad
        ListaPrecioDTO GetById(Guid id);
        IEnumerable<ListaPrecioDTO> GetAll();
        IEnumerable<ListaPrecioDTO> GetActivas();
    }
}
