using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        /// <summary>
        /// Obtiene usuario por nombre de usuario (login)
        /// </summary>
        /// <param name="usuario">Nombre de usuario</param>
        /// <returns>Usuario encontrado o null</returns>
        Usuario GetByEmailAsync(string usuario);

        /// <summary>
        /// Obtiene usuario por ID con todas sus relaciones cargadas
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario con relaciones cargadas</returns>
        Usuario GetByIdWithRelations(Guid id);
    }
}