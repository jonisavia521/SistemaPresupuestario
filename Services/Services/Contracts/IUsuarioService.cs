using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Contracts
{
    /// <summary>
    /// Contrato para operaciones CRUD de usuarios
    /// Separa responsabilidades: ILogin para autenticación, IUsuarioService para gestión
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Obtiene todos los usuarios del sistema (solo datos básicos, sin permisos)
        /// </summary>
        IEnumerable<Usuario> GetAll();

        /// <summary>
        /// Obtiene un usuario por ID (incluye permisos completos)
        /// </summary>
        Usuario GetById(Guid id);

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        bool Add(Usuario usuario);

        /// <summary>
        /// Actualiza datos de un usuario existente
        /// </summary>
        bool Update(Usuario usuario);

        /// <summary>
        /// Elimina un usuario del sistema
        /// </summary>
        bool Delete(Guid id);
    }
}