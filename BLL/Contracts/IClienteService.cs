using BLL.DTOs;
using System;
using System.Collections.Generic;

using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato que define las operaciones de lógica de negocio disponibles para la gestión de clientes.
    /// 
    /// Esta interfaz actúa como contrato entre la capa de presentación (UI) y la capa de lógica de negocio (BLL),
    /// especificando todas las operaciones disponibles para consultar, crear, actualizar y eliminar información
    /// de clientes. Implementa el patrón Repository y utiliza DTOs (Data Transfer Objects) para la comunicación
    /// entre capas, asegurando desacoplamiento entre la lógica de negocio y las entidades de dominio.
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Obtiene la lista completa de todos los clientes registrados en el sistema,
        /// tanto activos como inactivos.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos ClienteDTO que representan todos los clientes.
        /// Si no existen clientes, retorna una colección vacía.
        /// </returns>
        IEnumerable<ClienteDTO> GetAll();

        /// <summary>
        /// Obtiene únicamente los clientes que se encuentran activos en el sistema.
        /// Los clientes inactivos no son incluidos en esta consulta.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos ClienteDTO que representan solo los clientes activos.
        /// Si no existen clientes activos, retorna una colección vacía.
        /// </returns>
        IEnumerable<ClienteDTO> GetActivos();

        /// <summary>
        /// Obtiene un cliente específico por su identificador único (ID).
        /// </summary>
        /// <param name="id">
        /// El GUID (identificador único global) del cliente que se desea recuperar.
        /// </param>
        /// <returns>
        /// Un objeto ClienteDTO que contiene la información del cliente solicitado,
        /// o null si no existe un cliente con el ID proporcionado.
        /// </returns>
        ClienteDTO GetById(Guid id);

        /// <summary>
        /// Obtiene un cliente específico por su código de cliente (identificador de negocio).
        /// Este código es diferente del ID y es más amigable para el usuario final.
        /// </summary>
        /// <param name="codigoCliente">
        /// El código único del cliente en el sistema (ej: "CLI001", "CLI002").
        /// </param>
        /// <returns>
        /// Un objeto ClienteDTO que contiene la información del cliente solicitado,
        /// o null si no existe un cliente con el código proporcionado.
        /// </returns>
        ClienteDTO GetByCodigo(string codigoCliente);

        /// <summary>
        /// Obtiene un cliente específico por su número de documento fiscal (CUIT o DNI).
        /// </summary>
        /// <param name="numeroDocumento">
        /// El número de documento fiscal del cliente (CUIT de 11 dígitos o DNI de 8 dígitos).
        /// </param>
        /// <returns>
        /// Un objeto ClienteDTO que contiene la información del cliente solicitado,
        /// o null si no existe un cliente con el documento proporcionado.
        /// </returns>
        ClienteDTO GetByDocumento(string numeroDocumento);

        /// <summary>
        /// Agrega un nuevo cliente al sistema.
        /// 
        /// Esta operación realiza las validaciones de negocio necesarias, como verificar
        /// que no exista otro cliente con el mismo código o documento, antes de persistir
        /// el nuevo cliente en la base de datos.
        /// </summary>
        /// <param name="clienteDTO">
        /// Objeto ClienteDTO que contiene los datos del nuevo cliente a agregar.
        /// </param>
        /// <returns>
        /// true si el cliente fue agregado exitosamente a la base de datos;
        /// false si ocurrió un error durante la operación (ej: validación fallida).
        /// </returns>
        bool Add(ClienteDTO clienteDTO);

        /// <summary>
        /// Actualiza la información de un cliente existente en el sistema.
        /// 
        /// Esta operación modifica todos los datos del cliente proporcionado (nombre, dirección,
        /// teléfono, etc.) en la base de datos. El cliente se identifica por su ID.
        /// </summary>
        /// <param name="clienteDTO">
        /// Objeto ClienteDTO que contiene los datos actualizados del cliente.
        /// El ID debe corresponder a un cliente existente en el sistema.
        /// </param>
        /// <returns>
        /// true si el cliente fue actualizado exitosamente;
        /// false si ocurrió un error durante la operación (ej: cliente no encontrado).
        /// </returns>
        bool Update(ClienteDTO clienteDTO);

        /// <summary>
        /// Marca un cliente como inactivo (eliminación lógica).
        /// 
        /// En lugar de eliminar físicamente el registro de la base de datos, esta operación
        /// marca el cliente como inactivo, preservando la integridad histórica de los datos.
        /// Los datos del cliente permanecen en la base de datos y pueden ser reactivados.
        /// </summary>
        /// <param name="id">
        /// El GUID del cliente que se desea desactivar.
        /// </param>
        /// <returns>
        /// true si el cliente fue desactivado exitosamente;
        /// false si ocurrió un error durante la operación (ej: cliente no encontrado).
        /// </returns>
        bool Delete(Guid id);

        /// <summary>
        /// Reactiva un cliente que se encuentra inactivo.
        /// 
        /// Esta operación invierte el estado de un cliente previamente desactivado,
        /// permitiendo que vuelva a ser utilizado en operaciones del sistema.
        /// </summary>
        /// <param name="id">
        /// El GUID del cliente que se desea reactivar.
        /// </param>
        /// <returns>
        /// true si el cliente fue reactivado exitosamente;
        /// false si ocurrió un error durante la operación (ej: cliente no encontrado, ya está activo).
        /// </returns>
        bool Reactivar(Guid id);
        
        /// <summary>
        /// Actualiza las alícuotas ARBA (impuesto de Ingresos Brutos) de todos los clientes
        /// con tipo de IVA "Responsable Inscripto".
        /// 
        /// Esta operación es típicamente ejecutada periódicamente para sincronizar las alícuotas
        /// fiscales de los clientes con los datos actuales del padrón ARBA (Agencia de Recaudación
        /// de Buenos Aires). Los clientes con otros tipos de IVA (Monotributista, Exento, etc.)
        /// no son afectados por esta operación.
        /// </summary>
        /// <returns>
        /// Un entero que representa la cantidad de clientes cuyas alícuotas ARBA fueron
        /// actualizadas exitosamente en la base de datos.
        /// </returns>
        int ActualizarAlicuotasArba();
    }
}
