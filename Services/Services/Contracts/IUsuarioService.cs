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
        IEnumerable<Usuario> GetAll();
        Usuario GetById(Guid id);
        bool Add(Usuario usuario);
        bool Update(Usuario usuario);
        bool Delete(Guid id);

        IEnumerable<Familia> GetAllFamilias();

        IEnumerable<Patente> GetAllPatentes();

        bool SavePermisos(Usuario usuario);
    }
}