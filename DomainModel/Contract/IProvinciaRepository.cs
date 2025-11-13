using DomainModel.Domain;
using System;
using System.Collections.Generic;

namespace DomainModel.Contract
{
    /// <summary>
    /// Contrato del repositorio de Provincia
    /// Define las operaciones de acceso a datos para provincias
    /// </summary>
    public interface IProvinciaRepository : IRepository<ProvinciaDM>
    {
        /// <summary>
        /// Obtiene una provincia por su código AFIP
        /// </summary>
        ProvinciaDM GetByCodigoAFIP(string codigoAFIP);

        /// <summary>
        /// Obtiene todas las provincias ordenadas por código AFIP
        /// </summary>
        IEnumerable<ProvinciaDM> GetAllOrdenadas();
    }
}
