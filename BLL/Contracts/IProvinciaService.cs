using System;
using System.Collections.Generic;
using BLL.DTOs;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Provincia
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IProvinciaService
    {
        /// <summary>
        /// Obtiene una provincia por su ID
        /// </summary>
        ProvinciaDTO GetById(Guid id);

        /// <summary>
        /// Obtiene una provincia por su código AFIP
        /// </summary>
        ProvinciaDTO GetByCodigoAFIP(string codigoAFIP);

        /// <summary>
        /// Obtiene todas las provincias
        /// </summary>
        IEnumerable<ProvinciaDTO> GetAll();

        /// <summary>
        /// Obtiene todas las provincias ordenadas por código AFIP
        /// </summary>
        IEnumerable<ProvinciaDTO> GetAllOrdenadas();
    }
}
