using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Contracts
{
    public interface IUsuarioService
    {
        Task AddAsync(UsuarioDTO obj);

        void Update(UsuarioDTO obj);

        void Delete(UsuarioDTO obj);

        Task<IEnumerable<UsuarioDTO>> GetAllAsync();

       Task<UsuarioDTO> GetByIdAsync(Guid id);

    }
}