using System;
using System.Collections.Generic;
using BLL.DTOs;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato que define las operaciones de lógica de negocio disponibles para la gestión de provincias.
    /// 
    /// Las provincias son datos de referencia utilizados para completar información de dirección
    /// de clientes y para operaciones fiscales. Este servicio proporciona acceso de solo lectura
    /// a la base de datos de provincias argentinas, ya que son datos maestros que rara vez cambian.
    /// </summary>
    public interface IProvinciaService
    {
        /// <summary>
        /// Obtiene una provincia específica por su identificador único (ID).
        /// </summary>
        /// <param name="id">
        /// El GUID (identificador único global) de la provincia que se desea recuperar.
        /// </param>
        /// <returns>
        /// Un objeto ProvinciaDTO que contiene la información de la provincia solicitada,
        /// o null si no existe una provincia con el ID proporcionado.
        /// </returns>
        ProvinciaDTO GetById(Guid id);

        /// <summary>
        /// Obtiene una provincia específica por su código AFIP (Administración Federal de Ingresos Públicos).
        /// 
        /// AFIP es el organismo fiscal argentino que asigna códigos únicos a cada provincia.
        /// Estos códigos son estándares en documentación fiscal y sistemas integrados.
        /// </summary>
        /// <param name="codigoAFIP">
        /// El código AFIP de la provincia (ej: "01" para Buenos Aires, "05" para Catamarca, etc.).
        /// </param>
        /// <returns>
        /// Un objeto ProvinciaDTO que contiene la información de la provincia solicitada,
        /// o null si no existe una provincia con el código AFIP proporcionado.
        /// </returns>
        ProvinciaDTO GetByCodigoAFIP(string codigoAFIP);

        /// <summary>
        /// Obtiene la lista completa de todas las provincias registradas en el sistema.
        /// El orden de la colección no está garantizado.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos ProvinciaDTO que representan todas las provincias.
        /// Si no existen provincias, retorna una colección vacía (aunque esto es poco probable).
        /// </returns>
        IEnumerable<ProvinciaDTO> GetAll();

        /// <summary>
        /// Obtiene la lista completa de todas las provincias ordenadas alfabéticamente por código AFIP.
        /// Este método es útil para mostrar las provincias en listas desplegables de formularios.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos ProvinciaDTO ordenados por código AFIP de forma ascendente.
        /// Si no existen provincias, retorna una colección vacía.
        /// </returns>
        IEnumerable<ProvinciaDTO> GetAllOrdenadas();
    }
}
