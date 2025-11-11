using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato del servicio de Productos
    /// Define las operaciones de negocio disponibles
    /// </summary>
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDTO>> GetAllAsync();
        Task<IEnumerable<ProductoDTO>> GetActivosAsync();
        Task<ProductoDTO> GetByIdAsync(Guid id);
        Task<ProductoDTO> GetByCodigoAsync(string codigo);
        Task<bool> AddAsync(ProductoDTO productoDto);
        Task<bool> UpdateAsync(ProductoDTO productoDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null);
        
        // Métodos síncronos para compatibilidad
        ProductoDTO GetByCodigo(string codigo);
    }
}
