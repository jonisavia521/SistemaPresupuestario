using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IPatenteRepository : IRepository<Patente>
    {
        /// <summary>
        /// Obtiene todas las patentes ordenadas por nombre
        /// </summary>
        /// <returns>Lista completa de patentes</returns>
        IEnumerable<Patente> GetAll();

        /// <summary>
        /// Obtiene patentes por lista de IDs
        /// </summary>
        /// <param name="ids">IDs de patentes</param>
        /// <returns>Lista de patentes encontradas</returns>
        IEnumerable<Patente> GetByIds(IEnumerable<Guid> ids);
    }
}