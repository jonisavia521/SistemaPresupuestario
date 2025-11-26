using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato que define las operaciones de lógica de negocio disponibles para la gestión de vendedores.
    /// 
    /// Esta interfaz especifica todas las operaciones para consultar, crear, actualizar y eliminar información
    /// de vendedores en el sistema. Los vendedores son los responsables de realizar cotizaciones y ventas,
    /// y pueden tener asociadas comisiones sobre las operaciones que realicen.
    /// </summary>
    public interface IVendedorService
    {
        /// <summary>
        /// Obtiene la lista completa de todos los vendedores registrados en el sistema,
        /// tanto activos como inactivos.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos VendedorDTO que representan todos los vendedores.
        /// Si no existen vendedores, retorna una colección vacía.
        /// </returns>
        IEnumerable<VendedorDTO> GetAll();

        /// <summary>
        /// Obtiene únicamente los vendedores que se encuentran activos en el sistema.
        /// Los vendedores inactivos no son incluidos en esta consulta.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos VendedorDTO que representan solo los vendedores activos.
        /// Si no existen vendedores activos, retorna una colección vacía.
        /// </returns>
        IEnumerable<VendedorDTO> GetActivos();

        /// <summary>
        /// Obtiene un vendedor específico por su identificador único (ID).
        /// </summary>
        /// <param name="id">
        /// El GUID (identificador único global) del vendedor que se desea recuperar.
        /// </param>
        /// <returns>
        /// Un objeto VendedorDTO que contiene la información del vendedor solicitado,
        /// o null si no existe un vendedor con el ID proporcionado.
        /// </returns>
        VendedorDTO GetById(Guid id);

        /// <summary>
        /// Obtiene un vendedor específico por su código de vendedor (identificador de negocio).
        /// El código es típicamente un identificador corto y amigable (ej: "01", "02", etc.).
        /// </summary>
        /// <param name="codigoVendedor">
        /// El código único del vendedor en el sistema.
        /// </param>
        /// <returns>
        /// Un objeto VendedorDTO que contiene la información del vendedor solicitado,
        /// o null si no existe un vendedor con el código proporcionado.
        /// </returns>
        VendedorDTO GetByCodigo(string codigoVendedor);

        /// <summary>
        /// Obtiene un vendedor específico por su número de CUIT (Código Único de Identificación Tributaria).
        /// </summary>
        /// <param name="cuit">
        /// El número de CUIT del vendedor (11 dígitos).
        /// </param>
        /// <returns>
        /// Un objeto VendedorDTO que contiene la información del vendedor solicitado,
        /// o null si no existe un vendedor con el CUIT proporcionado.
        /// </returns>
        VendedorDTO GetByCUIT(string cuit);

        /// <summary>
        /// Agrega un nuevo vendedor al sistema.
        /// 
        /// Esta operación realiza validaciones de negocio necesarias, como verificar que no exista
        /// otro vendedor con el mismo código o CUIT, antes de persistir el nuevo vendedor en la base de datos.
        /// </summary>
        /// <param name="vendedorDTO">
        /// Objeto VendedorDTO que contiene los datos del nuevo vendedor a agregar.
        /// </param>
        /// <returns>
        /// true si el vendedor fue agregado exitosamente a la base de datos;
        /// false si ocurrió un error durante la operación (ej: validación fallida).
        /// </returns>
        bool Add(VendedorDTO vendedorDTO);

        /// <summary>
        /// Actualiza la información de un vendedor existente en el sistema.
        /// 
        /// Esta operación modifica los datos del vendedor (nombre, comisión, datos de contacto, etc.)
        /// en la base de datos. El vendedor se identifica por su ID.
        /// </summary>
        /// <param name="vendedorDTO">
        /// Objeto VendedorDTO que contiene los datos actualizados del vendedor.
        /// El ID debe corresponder a un vendedor existente en el sistema.
        /// </param>
        /// <returns>
        /// true si el vendedor fue actualizado exitosamente;
        /// false si ocurrió un error durante la operación (ej: vendedor no encontrado).
        /// </returns>
        bool Update(VendedorDTO vendedorDTO);

        /// <summary>
        /// Marca un vendedor como inactivo (eliminación lógica).
        /// 
        /// En lugar de eliminar físicamente el registro de la base de datos, esta operación
        /// marca el vendedor como inactivo, preservando la integridad histórica de los datos.
        /// Los datos del vendedor permanecen en la base de datos y pueden ser reactivados.
        /// </summary>
        /// <param name="id">
        /// El GUID del vendedor que se desea desactivar.
        /// </param>
        /// <returns>
        /// true si el vendedor fue desactivado exitosamente;
        /// false si ocurrió un error durante la operación (ej: vendedor no encontrado).
        /// </returns>
        bool Delete(Guid id);

        /// <summary>
        /// Reactiva un vendedor que se encuentra inactivo.
        /// 
        /// Esta operación invierte el estado de un vendedor previamente desactivado,
        /// permitiendo que vuelva a realizar operaciones en el sistema.
        /// </summary>
        /// <param name="id">
        /// El GUID del vendedor que se desea reactivar.
        /// </param>
        /// <returns>
        /// true si el vendedor fue reactivado exitosamente;
        /// false si ocurrió un error durante la operación (ej: vendedor no encontrado, ya está activo).
        /// </returns>
        bool Reactivar(Guid id);
    }
}
