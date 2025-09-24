using System.Collections.Generic;

namespace BLL.DTOs
{
    /// <summary>
    /// Resultado paginado para consultas
    /// </summary>
    /// <typeparam name="T">Tipo de elemento</typeparam>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
    }
}