using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Cliente
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDTO>> GetAllAsync();
        Task<IEnumerable<ClienteDTO>> GetActivosAsync();
        Task<ClienteDTO> GetByIdAsync(Guid id);
        Task<ClienteDTO> GetByCodigoAsync(string codigoCliente);
        Task<ClienteDTO> GetByDocumentoAsync(string numeroDocumento);
        Task<bool> AddAsync(ClienteDTO clienteDTO);
        Task<bool> UpdateAsync(ClienteDTO clienteDTO);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ReactivarAsync(Guid id);
    }
}
