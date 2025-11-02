using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Vendedor
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IVendedorService
    {
        Task<IEnumerable<VendedorDTO>> GetAllAsync();
        Task<IEnumerable<VendedorDTO>> GetActivosAsync();
        Task<VendedorDTO> GetByIdAsync(Guid id);
        Task<VendedorDTO> GetByCodigoAsync(string codigoVendedor);
        Task<VendedorDTO> GetByCUITAsync(string cuit);
        Task<bool> AddAsync(VendedorDTO vendedorDTO);
        Task<bool> UpdateAsync(VendedorDTO vendedorDTO);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ReactivarAsync(Guid id);
    }
}
