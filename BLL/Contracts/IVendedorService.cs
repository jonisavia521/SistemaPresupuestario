using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Vendedor
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IVendedorService
    {
        IEnumerable<VendedorDTO> GetAll();
        IEnumerable<VendedorDTO> GetActivos();
        VendedorDTO GetById(Guid id);
        VendedorDTO GetByCodigo(string codigoVendedor);
        VendedorDTO GetByCUIT(string cuit);
        bool Add(VendedorDTO vendedorDTO);
        bool Update(VendedorDTO vendedorDTO);
        bool Delete(Guid id);
        bool Reactivar(Guid id);
    }
}
