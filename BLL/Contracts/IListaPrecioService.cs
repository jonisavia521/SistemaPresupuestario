using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato que define las operaciones de lógica de negocio disponibles para la gestión de listas de precios.
    /// 
    /// Las listas de precios permiten establecer diferentes estrategias de precios para distintos clientes,
    /// segmentos de mercado o períodos. Un cliente puede tener asignada una lista de precios específica,
    /// y cuando se crea una cotización, los precios de los productos se obtienen de la lista asignada.
    /// </summary>
    public interface IListaPrecioService
    {
        /// <summary>
        /// Obtiene una lista de precios específica por su identificador único (ID),
        /// incluyendo todos los precios de productos configurados en esa lista.
        /// </summary>
        /// <param name="id">
        /// El GUID (identificador único global) de la lista de precios que se desea recuperar.
        /// </param>
        /// <returns>
        /// Un objeto ListaPrecioDTO que contiene la información de la lista y todos los detalles
        /// de precios de productos, o null si no existe lista con el ID proporcionado.
        /// </returns>
        ListaPrecioDTO GetById(Guid id);

        /// <summary>
        /// Obtiene una lista de precios específica por su código de identificación.
        /// El código es un identificador de negocio más amigable que el ID interno.
        /// </summary>
        /// <param name="codigo">
        /// El código único de la lista de precios en el sistema (ej: "LP001", "CLIENTE_ESPECIAL").
        /// </param>
        /// <returns>
        /// Un objeto ListaPrecioDTO que contiene la información de la lista solicitada,
        /// o null si no existe lista con el código proporcionado.
        /// </returns>
        ListaPrecioDTO GetByCodigo(string codigo);

        /// <summary>
        /// Obtiene la lista completa de todas las listas de precios registradas en el sistema,
        /// tanto activas como desactivadas.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos ListaPrecioDTO que representan todas las listas.
        /// Si no existen listas de precios, retorna una colección vacía.
        /// </returns>
        IEnumerable<ListaPrecioDTO> GetAll();

        /// <summary>
        /// Obtiene únicamente las listas de precios que se encuentran activas en el sistema.
        /// Las listas desactivadas se excluyen de esta consulta.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos ListaPrecioDTO que representan solo las listas activas.
        /// Si no existen listas activas, retorna una colección vacía.
        /// </returns>
        IEnumerable<ListaPrecioDTO> GetActivas();

        /// <summary>
        /// Crea una nueva lista de precios en el sistema.
        /// 
        /// Esta operación realiza validaciones de negocio necesarias, como verificar que no exista
        /// otra lista con el mismo código y que contenga al menos un producto con su respectivo precio.
        /// </summary>
        /// <param name="listaPrecioDTO">
        /// Objeto ListaPrecioDTO que contiene los datos de la nueva lista de precios,
        /// incluyendo código, nombre y detalles de precios de productos.
        /// </param>
        /// <returns>
        /// true si la lista de precios fue creada exitosamente;
        /// false si ocurrió un error durante la operación (ej: código duplicado, sin productos).
        /// </returns>
        bool Add(ListaPrecioDTO listaPrecioDTO);

        /// <summary>
        /// Actualiza una lista de precios existente en el sistema.
        /// 
        /// Esta operación permite modificar el nombre, código y los precios de los productos
        /// incluidos en la lista. La lista se identifica por su ID.
        /// </summary>
        /// <param name="listaPrecioDTO">
        /// Objeto ListaPrecioDTO que contiene los datos actualizados de la lista de precios.
        /// El ID debe corresponder a una lista existente en el sistema.
        /// </param>
        /// <returns>
        /// true si la lista de precios fue actualizada exitosamente;
        /// false si ocurrió un error durante la operación (ej: lista no encontrada).
        /// </returns>
        bool Update(ListaPrecioDTO listaPrecioDTO);

        /// <summary>
        /// Marca una lista de precios como desactivada (eliminación lógica).
        /// 
        /// En lugar de eliminar físicamente el registro de la base de datos, esta operación
        /// marca la lista como desactivada, preservando la integridad histórica de los datos.
        /// Las listas desactivadas no aparecerán en las listas disponibles para asignar a clientes.
        /// </summary>
        /// <param name="id">
        /// El GUID de la lista de precios que se desea desactivar.
        /// </param>
        /// <returns>
        /// true si la lista de precios fue desactivada exitosamente;
        /// false si ocurrió un error durante la operación (ej: lista no encontrada).
        /// </returns>
        bool Delete(Guid id);

        /// <summary>
        /// Reactiva una lista de precios que se encuentra desactivada.
        /// 
        /// Esta operación invierte el estado de una lista previamente desactivada,
        /// permitiendo que vuelva a ser utilizada en el sistema (ej: asignada a clientes).
        /// </summary>
        /// <param name="id">
        /// El GUID de la lista de precios que se desea reactivar.
        /// </param>
        /// <returns>
        /// true si la lista de precios fue reactivada exitosamente;
        /// false si ocurrió un error (ej: lista no encontrada, ya está activa).
        /// </returns>
        bool Reactivar(Guid id);

        /// <summary>
        /// Verifica si un código de lista de precios ya existe en el sistema.
        /// 
        /// Este método es útil para validaciones en tiempo de entrada, permitiendo detectar
        /// códigos duplicados antes de intentar agregar o actualizar una lista. Puede excluir
        /// un ID específico de la verificación para permitir que una lista mantenga su código actual.
        /// </summary>
        /// <param name="codigo">
        /// El código de la lista de precios que se desea verificar.
        /// </param>
        /// <param name="excludeId">
        /// Identificador (GUID) opcional de una lista a excluir de la búsqueda.
        /// Útil cuando se está actualizando una lista y se desea permitir que mantenga su código actual.
        /// Si es null, no se excluye ninguna lista de la búsqueda.
        /// </param>
        /// <returns>
        /// true si existe al menos una lista de precios con el código proporcionado (excluyendo excludeId);
        /// false si el código está disponible o no existe en el sistema.
        /// </returns>
        bool ExisteCodigo(string codigo, Guid? excludeId = null);

        /// <summary>
        /// Obtiene el precio de un producto específico dentro de una lista de precios determinada.
        /// 
        /// Este método es utilizado durante la creación de cotizaciones para obtener automáticamente
        /// los precios de los productos según la lista de precios asignada al cliente.
        /// </summary>
        /// <param name="idListaPrecio">
        /// El GUID de la lista de precios en la que se desea buscar el precio.
        /// </param>
        /// <param name="idProducto">
        /// El GUID del producto cuyo precio se desea obtener.
        /// </param>
        /// <returns>
        /// Un valor decimal que representa el precio del producto en la lista especificada.
        /// Retorna null si el producto no existe en la lista de precios o si la lista no existe.
        /// </returns>
        decimal? ObtenerPrecioProducto(Guid idListaPrecio, Guid idProducto);
    }
}
