using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Lista de Precios
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IListaPrecioService
    {
        /// <summary>
        /// Obtiene una lista de precios por su ID con sus detalles
        /// </summary>
        ListaPrecioDTO GetById(Guid id);

        /// <summary>
        /// Obtiene una lista de precios por su código
        /// </summary>
        ListaPrecioDTO GetByCodigo(string codigo);

        /// <summary>
        /// Obtiene todas las listas de precios
        /// </summary>
        IEnumerable<ListaPrecioDTO> GetAll();

        /// <summary>
        /// Obtiene todas las listas de precios activas
        /// </summary>
        IEnumerable<ListaPrecioDTO> GetActivas();

        /// <summary>
        /// Crea una nueva lista de precios
        /// </summary>
        bool Add(ListaPrecioDTO listaPrecioDTO);

        /// <summary>
        /// Actualiza una lista de precios existente
        /// </summary>
        bool Update(ListaPrecioDTO listaPrecioDTO);

        /// <summary>
        /// Elimina una lista de precios (desactivación lógica)
        /// </summary>
        bool Delete(Guid id);

        /// <summary>
        /// Reactiva una lista de precios desactivada
        /// </summary>
        bool Reactivar(Guid id);

        /// <summary>
        /// Verifica si un código ya existe (para validación)
        /// </summary>
        bool ExisteCodigo(string codigo, Guid? excludeId = null);

        /// <summary>
        /// Obtiene el precio de un producto en una lista específica
        /// </summary>
        decimal? ObtenerPrecioProducto(Guid idListaPrecio, Guid idProducto);
    }
}
