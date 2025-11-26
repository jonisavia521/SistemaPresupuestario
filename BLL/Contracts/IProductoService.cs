using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato que define las operaciones de lógica de negocio disponibles para la gestión de productos.
    /// 
    /// Esta interfaz especifica todas las operaciones para consultar, crear, actualizar y eliminar información
    /// de productos o servicios que la empresa ofrece a sus clientes. Los productos son utilizados en las
    /// cotizaciones y facturas para detallar los artículos objeto de venta.
    /// 
    /// NOTA IMPORTANTE: Todos los métodos son síncronos para evitar deadlocks en aplicaciones Windows Forms.
    /// La asincronía puede causar problemas de concurrencia en este tipo de interfaces de usuario.
    /// </summary>
    public interface IProductoService
    {
        /// <summary>
        /// Obtiene la lista completa de todos los productos registrados en el sistema,
        /// tanto activos como inhabilitados.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos ProductoDTO que representan todos los productos.
        /// Si no existen productos, retorna una colección vacía.
        /// </returns>
        IEnumerable<ProductoDTO> GetAll();

        /// <summary>
        /// Obtiene únicamente los productos que se encuentran activos (no inhabilitados) en el sistema.
        /// Los productos inhabilitados se excluyen de esta consulta.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos ProductoDTO que representan solo los productos activos.
        /// Si no existen productos activos, retorna una colección vacía.
        /// </returns>
        IEnumerable<ProductoDTO> GetActivos();

        /// <summary>
        /// Obtiene un producto específico por su identificador único (ID).
        /// </summary>
        /// <param name="id">
        /// El GUID (identificador único global) del producto que se desea recuperar.
        /// </param>
        /// <returns>
        /// Un objeto ProductoDTO que contiene la información del producto solicitado,
        /// o null si no existe un producto con el ID proporcionado.
        /// </returns>
        ProductoDTO GetById(Guid id);

        /// <summary>
        /// Busca un producto específico por su código de producto.
        /// El código es un identificador de negocio más amigable que el ID interno.
        /// </summary>
        /// <param name="codigo">
        /// El código único del producto en el sistema (ej: "PROD001", "PROD002").
        /// </param>
        /// <returns>
        /// Un objeto ProductoDTO que contiene la información del producto solicitado,
        /// o null si no existe un producto con el código proporcionado.
        /// </returns>
        ProductoDTO GetByCodigo(string codigo);

        /// <summary>
        /// Agrega un nuevo producto al sistema.
        /// 
        /// Esta operación realiza validaciones de negocio necesarias, como verificar que no exista
        /// otro producto con el mismo código, antes de persistir el nuevo producto en la base de datos.
        /// </summary>
        /// <param name="productoDto">
        /// Objeto ProductoDTO que contiene los datos del nuevo producto a agregar.
        /// Debe incluir código, descripción, alícuota de IVA y precio base.
        /// </param>
        /// <returns>
        /// true si el producto fue agregado exitosamente a la base de datos;
        /// false si ocurrió un error durante la operación (ej: código duplicado, validación fallida).
        /// </returns>
        bool Add(ProductoDTO productoDto);

        /// <summary>
        /// Actualiza la información de un producto existente en el sistema.
        /// 
        /// Esta operación modifica los datos del producto (código, descripción, precio, IVA, etc.)
        /// en la base de datos. El producto se identifica por su ID.
        /// </summary>
        /// <param name="productoDto">
        /// Objeto ProductoDTO que contiene los datos actualizados del producto.
        /// El ID debe corresponder a un producto existente en el sistema.
        /// </param>
        /// <returns>
        /// true si el producto fue actualizado exitosamente;
        /// false si ocurrió un error durante la operación (ej: producto no encontrado).
        /// </returns>
        bool Update(ProductoDTO productoDto);

        /// <summary>
        /// Marca un producto como inhabilitado (eliminación lógica).
        /// 
        /// En lugar de eliminar físicamente el registro de la base de datos, esta operación
        /// marca el producto como inhabilitado, preservando la integridad histórica de los datos.
        /// Los productos inhabilitados no aparecerán en las listas de productos disponibles para
        /// crear nuevas cotizaciones, pero permanecerán en las cotizaciones históricas.
        /// </summary>
        /// <param name="id">
        /// El GUID del producto que se desea inhabilitar.
        /// </param>
        /// <returns>
        /// true si el producto fue inhabilitado exitosamente;
        /// false si ocurrió un error durante la operación (ej: producto no encontrado).
        /// </returns>
        bool Delete(Guid id);

        /// <summary>
        /// Verifica si un código de producto ya existe en el sistema.
        /// 
        /// Este método es útil para validaciones en tiempo de entrada, permitiendo detectar
        /// códigos duplicados antes de intentar agregar o actualizar un producto. Puede excluir
        /// un ID específico de la verificación para permitir que un producto mantenga su código actual.
        /// </summary>
        /// <param name="codigo">
        /// El código del producto que se desea verificar.
        /// </param>
        /// <param name="excludeId">
        /// Identificador (GUID) opcional de un producto a excluir de la búsqueda.
        /// Útil cuando se está actualizando un producto y se desea permitir que mantenga su código actual.
        /// Si es null, no se excluye ningún producto de la búsqueda.
        /// </param>
        /// <returns>
        /// true si existe al menos un producto con el código proporcionado (excluyendo el excludeId si se proporciona);
        /// false si el código está disponible o no existe en el sistema.
        /// </returns>
        bool ExisteCodigo(string codigo, Guid? excludeId = null);
    }
}
