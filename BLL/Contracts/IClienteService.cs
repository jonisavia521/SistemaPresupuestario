using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Cliente
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IClienteService
    {
        IEnumerable<ClienteDTO> GetAll();
        IEnumerable<ClienteDTO> GetActivos();
        ClienteDTO GetById(Guid id);
        ClienteDTO GetByCodigo(string codigoCliente);
        ClienteDTO GetByDocumento(string numeroDocumento);
        bool Add(ClienteDTO clienteDTO);
        bool Update(ClienteDTO clienteDTO);
        bool Delete(Guid id);
        bool Reactivar(Guid id);
    }
}
