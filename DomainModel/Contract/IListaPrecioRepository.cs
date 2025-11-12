using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainModel.Contract;

namespace DomainModel.Contract
{
    /// <summary>
    /// Interfaz del repositorio de Lista de Precios
    /// Define las operaciones de persistencia específicas para listas de precios
    /// </summary>
    public interface IListaPrecioRepository : IRepository<object>
    {
        Task<IEnumerable<object>> GetAllWithDetailsAsync();
        Task<object> GetByIdWithDetailsAsync(Guid id);
        Task<object> GetByCodigoAsync(string codigo);
        Task<IEnumerable<object>> GetActivasAsync();
        Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null);
        Task<decimal?> ObtenerPrecioProductoAsync(Guid idListaPrecio, Guid idProducto);
        
        // Métodos síncronos
        object GetByIdWithDetails(Guid id);
        IEnumerable<object> GetAllWithDetails();
        IEnumerable<object> GetActivas();
    }
}
