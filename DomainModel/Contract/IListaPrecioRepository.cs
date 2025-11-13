using System;
using System.Collections.Generic;
using DomainModel.Contract;

namespace DomainModel.Contract
{
    /// <summary>
    /// Interfaz del repositorio de Lista de Precios
    /// Define las operaciones de persistencia específicas para listas de precios
    /// </summary>
    public interface IListaPrecioRepository : IRepository<object>
    {
        IEnumerable<object> GetAllWithDetails();
        object GetByIdWithDetails(Guid id);
        object GetByCodigo(string codigo);
        IEnumerable<object> GetActivas();
        bool ExisteCodigo(string codigo, Guid? excludeId = null);
        decimal? ObtenerPrecioProducto(Guid idListaPrecio, Guid idProducto);
    }
}
