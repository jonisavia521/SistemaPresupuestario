using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IFamiliaRepository : IRepository<Familia>
    {
        /// <summary>
        /// Obtiene toda la jerarquía de familias optimizada
        /// </summary>
        /// <returns>Lista completa de familias con relaciones padre-hijo</returns>
        IEnumerable<Familia> GetAllHierarchy();

        /// <summary>
        /// Obtiene familias por lista de IDs con sus patentes
        /// </summary>
        /// <param name="ids">IDs de familias</param>
        /// <returns>Lista de familias con patentes cargadas</returns>
        IEnumerable<Familia> GetByIds(IEnumerable<Guid> ids);

        /// <summary>
        /// Obtiene lista plana de todas las familias (sin jerarquía)
        /// </summary>
        /// <returns>Lista simple de familias</returns>
        IEnumerable<Familia> GetAllFlat();
    }
}